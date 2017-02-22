/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Behaviac.Design.Exporters
{
    public class BsonDeserializer
    {
        internal static class ListHelper
        {
            public static Type GetListItemType(Type enumerableType)
            {
                if (enumerableType.IsArray)
                {
                    return enumerableType.GetElementType();
                }

                return enumerableType.IsGenericType ? enumerableType.GetGenericArguments()[0] : typeof(object);
            }

            public static Type GetDictionarKeyType(Type kType)
            {
                return kType.IsGenericType ? kType.GetGenericArguments()[0] : typeof(object);
            }

            public static Type GetDictionarValueType(Type vType)
            {
                return vType.IsGenericType ? vType.GetGenericArguments()[1] : typeof(object);
            }

            public static IDictionary CreateDictionary(Type mapType, Type kType, Type vType)
            {
                if (mapType.IsInterface)
                {
                    return (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(kType, vType));
                }

                if (mapType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null) != null)
                {
                    return (IDictionary)Activator.CreateInstance(mapType);
                }

                return new Dictionary<object, object>();
            }
        }

        internal abstract class BaseWrapper
        {
            public static BaseWrapper Create(Type type, Type itemType, object existingContainer)
            {
                var instance = CreateWrapperFromType(existingContainer == null ? type : existingContainer.GetType(), itemType);
                instance.SetContainer(existingContainer ?? instance.CreateContainer(type, itemType));
                return instance;
            }

            private static BaseWrapper CreateWrapperFromType(Type type, Type itemType)
            {
                if (type.IsArray)
                {
                    return (BaseWrapper)Activator.CreateInstance(typeof(ArrayWrapper<>).MakeGenericType(itemType));
                }

                var isCollection = false;
                var types = new List<Type>();
                types.Insert(0, type.IsGenericType ? type.GetGenericTypeDefinition() : type);

                foreach (var @interface in types)
                {
                    if (typeof(IList<>).IsAssignableFrom(@interface) || typeof(IList).IsAssignableFrom(@interface))
                    {
                        return new ListWrapper();
                    }

                    if (typeof(ICollection<>).IsAssignableFrom(@interface))
                    {
                        isCollection = true;
                    }
                }

                if (isCollection)
                {
                    return (BaseWrapper)Activator.CreateInstance(typeof(CollectionWrapper<>).MakeGenericType(itemType));
                }

                //a last-ditch pass
                foreach (var @interface in types)
                {
                    if (typeof(IEnumerable<>).IsAssignableFrom(@interface) || typeof(IEnumerable).IsAssignableFrom(@interface))
                    {
                        return new ListWrapper();
                    }
                }

                throw new BsonException(string.Format("Collection of type {0} cannot be deserialized", type.FullName));
            }

            public abstract void Add(object value);
            public abstract object Collection
            {
                get;
            }

            protected abstract object CreateContainer(Type type, Type itemType);
            protected abstract void SetContainer(object container);
        }

        internal class ArrayWrapper<T> : BaseWrapper
        {
            private readonly List<T> _list = new List<T>();

            public override void Add(object value)
            {
                _list.Add((T)value);
            }

            protected override object CreateContainer(Type type, Type itemType)
            {
                return null;
            }

            protected override void SetContainer(object container)
            {
                if (container != null)
                {
                    throw new BsonException("An container cannot exist when trying to deserialize an array");
                }
            }

            public override object Collection
            {
                get
                {
                    return _list.ToArray();
                }
            }
        }

        internal class ListWrapper : BaseWrapper
        {
            private IList _list;

            public override object Collection
            {
                get
                {
                    return _list;
                }
            }
            public override void Add(object value)
            {
                _list.Add(value);
            }

            protected override object CreateContainer(Type type, Type itemType)
            {
                if (type.IsInterface)
                {
                    return Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
                }

                if (type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null) != null)
                {
                    return Activator.CreateInstance(type);
                }

                return null;
            }
            protected override void SetContainer(object container)
            {
                _list = container == null ? new ArrayList() : (IList)container;
            }
        }

        internal class CollectionWrapper<T> : BaseWrapper
        {
            private ICollection<T> _list;

            public override object Collection
            {
                get
                {
                    return _list;
                }
            }

            public override void Add(object value)
            {
                _list.Add((T)value);
            }

            protected override object CreateContainer(Type type, Type itemType)
            {
                return Activator.CreateInstance(type);
            }
            protected override void SetContainer(object container)
            {
                _list = (ICollection<T>)container;
            }
        }

        private readonly static IDictionary<Types, Type> _typeMap = new Dictionary<Types, Type>
        {
            {Types.Int32, typeof(int)}, {Types.Int64, typeof(long)}, {Types.Boolean, typeof(bool)}, {Types.String, typeof(string)},
            {Types.Double, typeof(double)}, {Types.Binary, typeof(byte[])}, {Types.Regex, typeof(Regex)}, {Types.DateTime, typeof(DateTime)}
        };
        private readonly BinaryReader _reader;
        private Document _current;

        private BsonDeserializer(BinaryReader reader)
        {
            _reader = reader;
        }

        public static T Deserializer<T>(BinaryReader reader)
        {
            BsonDeserializer ds = new BsonDeserializer(reader);

            return ds.Read<T>();
        }

        private T Read<T>()
        {
            NewDocument(_reader.ReadInt32());
            var @object = (T)DeserializeValue(typeof(T), Types.Object);
            return @object;
        }

        private void Read(int read)
        {
            _current.Digested += read;
        }

        private bool IsDone()
        {
            var isDone = _current.Digested + 1 == _current.Length;

            if (isDone)
            {
                _reader.ReadByte(); // EOO
                var old = _current;
                _current = old.Parent;

                if (_current != null)
                {
                    Read(old.Length);
                }
            }

            return isDone;
        }

        private void NewDocument(int length)
        {
            var old = _current;
            _current = new Document { Length = length, Parent = old, Digested = 4 };
        }

        private object DeserializeValue(Type type, Types storedType)
        {
            return DeserializeValue(type, storedType, null);
        }

        private object DeserializeValue(Type type, Types storedType, object container)
        {
            if (storedType == Types.Null)
            {
                return null;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (type == typeof(string))
            {
                return ReadString();
            }

            if (type == typeof(int))
            {
                return ReadInt(storedType);
            }

            if (type.IsEnum)
            {
                return ReadEnum(type, storedType);
            }

            if (type == typeof(float))
            {
                Read(8);
                return (float)_reader.ReadDouble();
            }

            if (storedType == Types.Binary)
            {
                return ReadBinary();
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return ReadList(type, container);
            }

            if (type == typeof(bool))
            {
                Read(1);
                return _reader.ReadBoolean();
            }

            if (type == typeof(DateTime))
            {
                return BsonSerializer.Epoch.AddMilliseconds(ReadLong(Types.Int64));
            }

            if (type == typeof(long))
            {
                return ReadLong(storedType);
            }

            if (type == typeof(double))
            {
                Read(8);
                return _reader.ReadDouble();
            }

            if (type == typeof(Regex))
            {
                return ReadRegularExpression();
            }

            return ReadObject(type);
        }

        private object ReadObject(Type type)
        {
            var instance = Activator.CreateInstance(type, true);
            var typeHelper = TypeHelper.GetHelperForType(type);

            while (true)
            {
                var storageType = ReadType();
                var name = ReadName();
                var isNull = false;

                if (storageType == Types.Object)
                {
                    var length = _reader.ReadInt32();

                    if (length == 5)
                    {
                        _reader.ReadByte(); //eoo
                        Read(5);
                        isNull = true;

                    }
                    else
                    {
                        NewDocument(length);
                    }
                }

                object container = null;
                var property = typeHelper.FindProperty(name);
                var propertyType = property != null ? property.PropertyType : _typeMap.ContainsKey(storageType) ? _typeMap[storageType] : typeof(object);

                if (property == null)
                {
                    throw new BsonException(string.Format("Deserialization failed: type {0} does not have a property named {1}", type.FullName, name));
                }

                var value = isNull ? null : DeserializeValue(propertyType, storageType, container);

                if (IsDone())
                {
                    break;
                }
            }

            return instance;
        }

        private object ReadList(Type listType, object existingContainer)
        {
            if (IsDictionary(listType))
            {
                return ReadDictionary(listType, existingContainer);
            }

            NewDocument(_reader.ReadInt32());
            var itemType = ListHelper.GetListItemType(listType);
            var isObject = typeof(object) == itemType;
            var wrapper = BaseWrapper.Create(listType, itemType, existingContainer);

            while (!IsDone())
            {
                var storageType = ReadType();
                ReadName();

                if (storageType == Types.Object)
                {
                    NewDocument(_reader.ReadInt32());
                }

                var specificItemType = isObject ? _typeMap[storageType] : itemType;
                var value = DeserializeValue(specificItemType, storageType);
                wrapper.Add(value);
            }

            return wrapper.Collection;
        }

        private static bool IsDictionary(Type type)
        {
            var types = new List<Type>(type.GetInterfaces());
            types.Insert(0, type);

            foreach (var interfaceType in types)
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    return true;
                }
            }

            return false;
        }

        private object ReadDictionary(Type listType, object existingContainer)
        {
            var valueType = ListHelper.GetDictionarValueType(listType);
            var isObject = typeof(object) == valueType;
            var container = existingContainer == null ? ListHelper.CreateDictionary(listType, ListHelper.GetDictionarKeyType(listType), valueType) : (IDictionary)existingContainer;

            while (!IsDone())
            {
                var storageType = ReadType();

                var key = ReadName();

                if (storageType == Types.Object)
                {
                    NewDocument(_reader.ReadInt32());
                }

                var specificItemType = isObject ? _typeMap[storageType] : valueType;
                var value = DeserializeValue(specificItemType, storageType);
                container.Add(key, value);
            }

            return container;
        }

        private object ReadBinary()
        {
            var length = _reader.ReadInt32();
            var subType = _reader.ReadByte();
            Read(5 + length);

            if (subType == 2)
            {
                return _reader.ReadBytes(_reader.ReadInt32());
            }

            if (subType == 3)
            {
                return new Guid(_reader.ReadBytes(length));
            }

            throw new BsonException("No support for binary type: " + subType);
        }

        private string ReadName()
        {
            var buffer = new List<byte>(128);
            byte b;

            while ((b = _reader.ReadByte()) > 0)
            {
                buffer.Add(b);
            }

            Read(buffer.Count + 1);
            return Encoding.UTF8.GetString(buffer.ToArray());
        }

        private string ReadString()
        {
            var length = _reader.ReadInt32();
            var buffer = _reader.ReadBytes(length - 1);
            _reader.ReadByte(); //null;
            Read(4 + length);

            return Encoding.UTF8.GetString(buffer);
        }

        private int ReadInt(Types storedType)
        {
            switch (storedType)
            {
                case Types.Int32:
                    Read(4);
                    return _reader.ReadInt32();

                case Types.Int64:
                    Read(8);
                    return (int)_reader.ReadInt64();

                case Types.Double:
                    Read(8);
                    return (int)_reader.ReadDouble();

                default:
                    throw new BsonException("Could not create an int from " + storedType);
            }
        }

        private long ReadLong(Types storedType)
        {
            switch (storedType)
            {
                case Types.Int32:
                    Read(4);
                    return _reader.ReadInt32();

                case Types.Int64:
                    Read(8);
                    return _reader.ReadInt64();

                case Types.Double:
                    Read(8);
                    return (long)_reader.ReadDouble();

                default:
                    throw new BsonException("Could not create an int64 from " + storedType);
            }
        }

        private object ReadEnum(Type type, Types storedType)
        {
            if (storedType == Types.Int64)
            {
                return Enum.Parse(type, ReadLong(storedType).ToString(), false);
            }

            return Enum.Parse(type, ReadInt(storedType).ToString(), false);
        }

        private object ReadRegularExpression()
        {
            var pattern = ReadName();
            var optionsString = ReadName();

            var options = RegexOptions.None;

            if (optionsString.Contains("i"))
            {
                options = options | RegexOptions.IgnoreCase;
            }

            if (optionsString.Contains("e"))
            {
                options = options | RegexOptions.ECMAScript;
            }

            if (optionsString.Contains("l"))
            {
                options = options | RegexOptions.CultureInvariant;
            }


            if (optionsString.Contains("s"))
            {
                options = options | RegexOptions.Singleline;
            }

            if (optionsString.Contains("m"))
            {
                options = options | RegexOptions.Multiline;
            }

            if (optionsString.Contains("w"))
            {
                options = options | RegexOptions.IgnorePatternWhitespace;
            }

            if (optionsString.Contains("x"))
            {
                options = options | RegexOptions.ExplicitCapture;
            }

            return new Regex(pattern, options);
        }

        private Types ReadType()
        {
            Read(1);
            return (Types)_reader.ReadByte();
        }
    }
}