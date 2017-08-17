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
using System.Reflection;

namespace behaviac
{
    public class AgentMetaVisitor
    {
        private static Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();
        private static Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();
        private static Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();

        public static object GetProperty(behaviac.Agent agent, string property)
        {
            Type type = agent.GetType();
            string propertyName = type.FullName + property;

            if (_fields.ContainsKey(propertyName))
            {
                return _fields[propertyName].GetValue(agent);
            }

            if (_properties.ContainsKey(propertyName))
            {
                return _properties[propertyName].GetValue(agent, null);
            }

            while (type != typeof(object))
            {
                FieldInfo field = type.GetField(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                if (field != null)
                {
                    _fields[propertyName] = field;
                    return field.GetValue(agent);
                }

                PropertyInfo prop = type.GetProperty(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                if (prop != null)
                {
                    _properties[propertyName] = prop;
                    return prop.GetValue(agent, null);
                }

                type = type.BaseType;
            }

            Debug.Check(false, "No property can be found!");
            return null;
        }

        public static void SetProperty(behaviac.Agent agent, string property, object value)
        {
            Type type = agent.GetType();
            string propertyName = type.FullName + property;

            if (_fields.ContainsKey(propertyName))
            {
                _fields[propertyName].SetValue(agent, value);
                return;
            }

            if (_properties.ContainsKey(propertyName))
            {
                _properties[propertyName].SetValue(agent, value, null);
                return;
            }

            while (type != typeof(object))
            {
                FieldInfo field = type.GetField(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                if (field != null)
                {
                    _fields[propertyName] = field;
                    field.SetValue(agent, value);
                    return;
                }

                PropertyInfo prop = type.GetProperty(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                if (prop != null)
                {
                    _properties[propertyName] = prop;
                    prop.SetValue(agent, value, null);
                    return;
                }

                type = type.BaseType;
            }

            Debug.Check(false, "No property can be found!");
        }

        public static object ExecuteMethod(behaviac.Agent agent, string method, object[] args)
        {
            Type type = agent.GetType();
            string methodName = type.FullName + method;

            if (_methods.ContainsKey(methodName))
            {
                return _methods[methodName].Invoke(agent, args); ;
            }

            while (type != typeof(object))
            {
                MethodInfo m = type.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                if (m != null)
                {
                    _methods[methodName] = m;
                    return m.Invoke(agent, args);
                }

                type = type.BaseType;
            }

            Debug.Check(false, "No method can be found!");
            return null;
        }
    }

    //public static class ExpressionUtil
    //{
    //    public static Func<TArg1, TResult> CreateExpression<TArg1, TResult>(
    //        Func<Expression, UnaryExpression> body)
    //    {
    //        ParameterExpression inp = Expression.Parameter(typeof(TArg1), "inp");
    //        try
    //        {
    //            return Expression.Lambda<Func<TArg1, TResult>>(body(inp), inp).Compile();
    //        }
    //        catch (Exception ex)
    //        {
    //            string msg = ex.Message;
    //            return delegate { throw new InvalidOperationException(msg); };
    //        }
    //    }

    //    public static Func<TArg1, TArg2, TResult> CreateExpression<TArg1, TArg2, TResult>(
    //        Func<Expression, Expression, BinaryExpression> body)
    //    {
    //        return CreateExpression<TArg1, TArg2, TResult>(body, false);
    //    }

    //    public static Func<TArg1, TArg2, TResult> CreateExpression<TArg1, TArg2, TResult>(
    //        Func<Expression, Expression, BinaryExpression> body, bool castArgsToResultOnFailure)
    //    {
    //        ParameterExpression lhs = Expression.Parameter(typeof(TArg1), "lhs");
    //        ParameterExpression rhs = Expression.Parameter(typeof(TArg2), "rhs");
    //        try
    //        {
    //            try
    //            {
    //                return Expression.Lambda<Func<TArg1, TArg2, TResult>>(body(lhs, rhs), lhs, rhs).Compile();
    //            }
    //            catch (InvalidOperationException)
    //            {
    //                if (castArgsToResultOnFailure && !(typeof(TArg1) == typeof(TResult) && typeof(TArg2) == typeof(TResult)))
    //                {
    //                    Expression castLhs = typeof(TArg1) == typeof(TResult) ?
    //                            (Expression)lhs :
    //                            (Expression)Expression.Convert(lhs, typeof(TResult));
    //                    Expression castRhs = typeof(TArg2) == typeof(TResult) ?
    //                            (Expression)rhs :
    //                            (Expression)Expression.Convert(rhs, typeof(TResult));

    //                    return Expression.Lambda<Func<TArg1, TArg2, TResult>>(
    //                        body(castLhs, castRhs), lhs, rhs).Compile();
    //                }
    //                else
    //                {
    //                    throw;
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            string msg = ex.Message;
    //            return delegate { throw new InvalidOperationException(msg); };
    //        }
    //    }

    //    public static Func<TArg1, TArg2, bool> MakeMemberEqualsMethod<TArg1, TArg2>()
    //    {
    //        try
    //        {
    //            Type type1 = typeof(TArg1);
    //            Type type2 = typeof(TArg1);
    //            Debug.Check(Utils.IsCustomStructType(type1));

    //            ParameterExpression pThis = Expression.Parameter(type1, "lhs");
    //            ParameterExpression pThat = Expression.Parameter(type2, "rhs");

    //            // cast to the subclass type1
    //            UnaryExpression pCastThis = Expression.Convert(pThis, type1);
    //            UnaryExpression pCastThat = Expression.Convert(pThat, type2);

    //            // compound AND expression using short-circuit evaluation
    //            Expression last = null;
    //            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
    //            foreach (FieldInfo field in type1.GetFields(bindingFlags))
    //            {
    //                BinaryExpression equals = null;

    //                if (field.FieldType == typeof(System.Char))
    //                {
    //                    //Char needs to be checked before primitive
    //                    MethodInfo CharEquals = typeof(Extensions).GetMethod("CharEquals");
    //                    equals = Expression.Equal(
    //                        Expression.Field(pCastThis, field),
    //                        Expression.Field(pCastThat, field),
    //                        false,
    //                        CharEquals
    //                    );
    //                }
    //                else if (field.FieldType.IsPrimitive || field.FieldType == typeof (string) || field.FieldType.IsEnum)
    //                {
    //                    equals = Expression.Equal(
    //                        Expression.Field(pCastThis, field),
    //                        Expression.Field(pCastThat, field)
    //                    );
    //                }
    //                else
    //                {
    //                    MethodInfo TypesEquals = typeof(Extensions).GetMethod("TypesEquals").MakeGenericMethod(field.FieldType); ;
    //                    equals = Expression.Equal(
    //                        Expression.Field(pCastThis, field),
    //                        Expression.Field(pCastThat, field),
    //                        false,
    //                        TypesEquals
    //                    );
    //                }

    //                if (last == null)
    //                    last = equals;
    //                else
    //                    last = Expression.AndAlso(last, equals);
    //            }

    //            //Expression<Func<bool>> falsePredicate = () => false;
    //            Expression falsePredicate = Expression.Constant(false);
    //            // call Object.Equals if second parameter doesn't match type1
    //            last = Expression.Condition(
    //                Expression.TypeIs(pThat, type1),
    //                last,
    //                falsePredicate
    //            );

    //            // compile method
    //            return Expression.Lambda<Func<TArg1, TArg2, bool>>(last, pThis, pThat).Compile();
    //        }
    //        catch (Exception ex)
    //        {
    //            string msg = ex.Message;
    //            return delegate { throw new InvalidOperationException(msg); };
    //        }
    //    }

    //    class Extensions
    //    {
    //        /// more than ten times faster than SequenceEquals
    //        public static bool ListsAreEqual<T>(List<T> a, List<T> b)
    //        {
    //            // if both are null, ReferenceEquals returns true
    //            if (Object.ReferenceEquals(a, b))
    //            {
    //                return true;
    //            }

    //            if (a == null || b == null)
    //            {
    //                return false;
    //            }

    //            if (a.Count != b.Count)
    //            {
    //                return false;
    //            }

    //            //EqualityComparer<T> comparer = EqualityComparer<T>.Default;
    //            //return !b1.Where((t, i) => !comparer.Equals(t, b2[i])).Any();

    //            for (int i = 0; i < a.Count; ++i)
    //            {
    //                T ai = a[i];
    //                T bi = b[i];

    //                bool bEqual = OperationUtils.Compare(ai, bi, EOperatorType.E_EQUAL);
    //                if (!bEqual)
    //                {
    //                    return false;
    //                }
    //            }

    //            return true;
    //        }

    //        public static bool CharEquals(System.Char a, System.Char b)
    //        {
    //            return a == b;
    //        }

    //        public static bool TypesEquals<T>(T a, T b)
    //        {
    //            return OperationUtils.Compare(a, b, EOperatorType.E_EQUAL);
    //        }
    //    }

    //    //Debug.Log(System.Enviroment.Version);
    //    public static Func<TArg1, TArg2, bool> MakeItemEqualsMethod<TArg1, TArg2>()
    //    {
    //        try
    //        {
    //            Type type1 = typeof(TArg1);
    //            Type type2 = typeof(TArg1);
    //            Debug.Check(Utils.IsArrayType(type1));

    //            ParameterExpression pThis = Expression.Parameter(type1, "lhs");
    //            ParameterExpression pThat = Expression.Parameter(type2, "rhs");

    //            // cast to the subclass type1
    //            UnaryExpression pCastThis = Expression.Convert(pThis, type1);
    //            UnaryExpression pCastThat = Expression.Convert(pThat, type2);

    //            Type elementType = type1.GetGenericArguments()[0];
    //            MethodInfo enEqualsMethod = typeof(Extensions).GetMethod("ListsAreEqual").MakeGenericMethod(elementType);
    //            Expression equals = Expression.Call(enEqualsMethod, pCastThis, pCastThat);

    //            Expression last = Expression.Condition(
    //                Expression.TypeIs(pThat, type1),
    //                equals,
    //                Expression.Constant(false)
    //            );

    //            // compile method
    //            return Expression.Lambda<Func<TArg1, TArg2, bool>>(last, pThis, pThat).Compile();
    //        }
    //        catch (Exception ex)
    //        {
    //            string msg = ex.Message;
    //            return delegate { throw new InvalidOperationException(msg); };
    //        }
    //    }

    //}

    //public static class Operator<T>
    //{
    //    static readonly Func<T, T, T> add, subtract, multiply, divide;

    //    public static Func<T, T, T> Add { get { return add; } }
    //    public static Func<T, T, T> Subtract { get { return subtract; } }
    //    public static Func<T, T, T> Multiply { get { return multiply; } }
    //    public static Func<T, T, T> Divide { get { return divide; } }

    //    static readonly Func<T, T, bool> memberEqual, equal, notEqual, greaterThan, lessThan, greaterThanOrEqual, lessThanOrEqual;
    //    static readonly Func<T, T, bool> itemEqual;

    //    public static Func<T, T, bool> MemberEqual { get { return memberEqual; } }

    //    public static Func<T, T, bool> ItemEqual { get { return itemEqual; } }

    //    public static Func<T, T, bool> Equal { get { return equal; } }
    //    public static Func<T, T, bool> NotEqual { get { return notEqual; } }
    //    public static Func<T, T, bool> GreaterThan { get { return greaterThan; } }
    //    public static Func<T, T, bool> GreaterThanOrEqual { get { return greaterThanOrEqual; } }
    //    public static Func<T, T, bool> LessThan { get { return lessThan; } }
    //    public static Func<T, T, bool> LessThanOrEqual { get { return lessThanOrEqual; } }

    //    static Operator()
    //    {
    //        Type type = typeof(T);

    //        if (!Utils.IsAgentType(type) && !Utils.IsCustomClassType(type) && !Utils.IsCustomStructType(type) && !Utils.IsArrayType(type) && !Utils.IsEnumType(type) &&
    //            !Utils.IsStringType(type) && type != typeof(bool) && type != typeof(char))
    //        {
    //            add = ExpressionUtil.CreateExpression<T, T, T>(Expression.Add);
    //            subtract = ExpressionUtil.CreateExpression<T, T, T>(Expression.Subtract);
    //            divide = ExpressionUtil.CreateExpression<T, T, T>(Expression.Divide);
    //            multiply = ExpressionUtil.CreateExpression<T, T, T>(Expression.Multiply);

    //            greaterThan = ExpressionUtil.CreateExpression<T, T, bool>(Expression.GreaterThan);
    //            greaterThanOrEqual = ExpressionUtil.CreateExpression<T, T, bool>(Expression.GreaterThanOrEqual);
    //            lessThan = ExpressionUtil.CreateExpression<T, T, bool>(Expression.LessThan);
    //            lessThanOrEqual = ExpressionUtil.CreateExpression<T, T, bool>(Expression.LessThanOrEqual);
    //        }

    //        if (Utils.IsCustomStructType(type))
    //        {
    //            memberEqual = ExpressionUtil.MakeMemberEqualsMethod<T, T>();
    //        }
    //        else if (Utils.IsArrayType(type))
    //        {
    //            itemEqual = ExpressionUtil.MakeItemEqualsMethod<T, T>();
    //        }
    //        else
    //        {
    //            equal = ExpressionUtil.CreateExpression<T, T, bool>(Expression.Equal);
    //            notEqual = ExpressionUtil.CreateExpression<T, T, bool>(Expression.NotEqual);
    //        }
    //    }
    //}
    #region Compare
    public interface ICompareValue
    {
        bool ItemEqual(IList ll, IList rl, int index);
        bool MemberEqual<U>(U lo, U ro, FieldInfo field);
    }

    public class ICompareValue<T> :  ICompareValue
    {
        public bool ItemEqual(IList ll, IList rl, int index)
        {
            List<T> llt = (List<T>)ll;
            List<T> rlt = (List<T>)rl;

            T l = llt[index];
            T r = rlt[index];

            return OperationUtils.Compare<T>(l, r, EOperatorType.E_EQUAL);
        }

        public bool MemberEqual<U>(U lo, U ro, FieldInfo field)
        {
            //possible boxing
            T l = (T)field.GetValue(lo);
            T r = (T)field.GetValue(ro);

            return OperationUtils.Compare<T>(l, r, EOperatorType.E_EQUAL);
        }

        public virtual bool Equal(T lhs, T rhs)
        {
            return false;
        }

        public virtual bool NotEqual(T lhs, T rhs)
        {
            return false;
        }

        public virtual bool Greater(T lhs, T rhs)
        {
            return false;
        }

        public virtual bool GreaterEqual(T lhs, T rhs)
        {
            return false;
        }

        public virtual bool Less(T lhs, T rhs)
        {
            return false;
        }

        public virtual bool LessEqual(T lhs, T rhs)
        {
            return false;
        }
    }

    public class CompareValueBool : ICompareValue<bool>
    {
        public override bool Equal(bool lhs, bool rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(bool lhs, bool rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(bool lhs, bool rhs)
        {
            Debug.Check(false);
            return false;
        }

        public override bool GreaterEqual(bool lhs, bool rhs)
        {
            Debug.Check(false);
            return false;
        }

        public override bool Less(bool lhs, bool rhs)
        {
            Debug.Check(false);
            return false;
        }

        public override bool LessEqual(bool lhs, bool rhs)
        {
            Debug.Check(false);
            return false;
        }

    }
    public class CompareValueInt : ICompareValue<int>
    {
        public override bool Equal(int lhs, int rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(int lhs, int rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(int lhs, int rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(int lhs, int rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(int lhs, int rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(int lhs, int rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueLong : ICompareValue<long>
    {
        public override bool Equal(long lhs, long rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(long lhs, long rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(long lhs, long rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(long lhs, long rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(long lhs, long rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(long lhs, long rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueShort : ICompareValue<short>
    {
        public override bool Equal(short lhs, short rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(short lhs, short rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(short lhs, short rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(short lhs, short rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(short lhs, short rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(short lhs, short rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueByte : ICompareValue<sbyte>
    {
        public override bool Equal(sbyte lhs, sbyte rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(sbyte lhs, sbyte rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(sbyte lhs, sbyte rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(sbyte lhs, sbyte rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(sbyte lhs, sbyte rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(sbyte lhs, sbyte rhs)
        {
            return (lhs <= rhs);
        }

    }


    public class CompareValueUInt : ICompareValue<uint>
    {
        public override bool Equal(uint lhs, uint rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(uint lhs, uint rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(uint lhs, uint rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(uint lhs, uint rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(uint lhs, uint rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(uint lhs, uint rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueULong : ICompareValue<ulong>
    {
        public override bool Equal(ulong lhs, ulong rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(ulong lhs, ulong rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(ulong lhs, ulong rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(ulong lhs, ulong rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(ulong lhs, ulong rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(ulong lhs, ulong rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueUShort : ICompareValue<ushort>
    {
        public override bool Equal(ushort lhs, ushort rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(ushort lhs, ushort rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(ushort lhs, ushort rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(ushort lhs, ushort rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(ushort lhs, ushort rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(ushort lhs, ushort rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueUByte : ICompareValue<byte>
    {
        public override bool Equal(byte lhs, byte rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(byte lhs, byte rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(byte lhs, byte rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(byte lhs, byte rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(byte lhs, byte rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(byte lhs, byte rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueChar : ICompareValue<char>
    {
        public override bool Equal(char lhs, char rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(char lhs, char rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(char lhs, char rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(char lhs, char rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(char lhs, char rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(char lhs, char rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueFloat : ICompareValue<float>
    {
        public override bool Equal(float lhs, float rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(float lhs, float rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(float lhs, float rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(float lhs, float rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(float lhs, float rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(float lhs, float rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueDouble : ICompareValue<double>
    {
        public override bool Equal(double lhs, double rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(double lhs, double rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(double lhs, double rhs)
        {
            return (lhs > rhs);
        }

        public override bool GreaterEqual(double lhs, double rhs)
        {
            return (lhs >= rhs);
        }

        public override bool Less(double lhs, double rhs)
        {
            return (lhs < rhs);
        }

        public override bool LessEqual(double lhs, double rhs)
        {
            return (lhs <= rhs);
        }

    }

    public class CompareValueString : ICompareValue<string>
    {
        public CompareValueString()
        {

        }

        public override bool Equal(string lhs, string rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(string lhs, string rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(string lhs, string rhs)
        {
            Debug.Check(false);
            return false;
        }

        public override bool GreaterEqual(string lhs, string rhs)
        {
            Debug.Check(false);
            return false;
        }

        public override bool Less(string lhs, string rhs)
        {
            Debug.Check(false);
            return false;
        }

        public override bool LessEqual(string lhs, string rhs)
        {
            Debug.Check(false);
            return false;
        }

    }

    public class CompareValueObject : ICompareValue<object>
    {
        public override bool Equal(object lhs, object rhs)
        {
            return lhs == rhs;
        }
        public override bool NotEqual(object lhs, object rhs)
        {
            return lhs != rhs;
        }

        public override bool Greater(object lhs, object rhs)
        {
            Debug.Check(false);
            return false;
        }

        public override bool GreaterEqual(object lhs, object rhs)
        {
            Debug.Check(false);
            return false;
        }

        public override bool Less(object lhs, object rhs)
        {
            Debug.Check(false);
            return false;
        }

        public override bool LessEqual(object lhs, object rhs)
        {
            Debug.Check(false);
            return false;
        }

    }

    public class ComparerRegister
    {
        private static Dictionary<Type, ICompareValue> ms_valueComparers = null;

        private static void Register()
        {
            {
                CompareValueBool pComparer = new CompareValueBool();
                ms_valueComparers.Add(typeof(bool), pComparer);
            }

            {
                CompareValueInt pComparer = new CompareValueInt();
                ms_valueComparers.Add(typeof(int), pComparer);
            }
            {
                CompareValueLong pComparer = new CompareValueLong();
                ms_valueComparers.Add(typeof(long), pComparer);
            }
            {
                CompareValueShort pComparer = new CompareValueShort();
                ms_valueComparers.Add(typeof(short), pComparer);
            }
            {
                CompareValueByte pComparer = new CompareValueByte();
                ms_valueComparers.Add(typeof(sbyte), pComparer);
            }

            {
                CompareValueUInt pComparer = new CompareValueUInt();
                ms_valueComparers.Add(typeof(uint), pComparer);
            }
            {
                CompareValueULong pComparer = new CompareValueULong();
                ms_valueComparers.Add(typeof(ulong), pComparer);
            }
            {
                CompareValueUShort pComparer = new CompareValueUShort();
                ms_valueComparers.Add(typeof(ushort), pComparer);
            }
            {
                CompareValueUByte pComparer = new CompareValueUByte();
                ms_valueComparers.Add(typeof(byte), pComparer);
            }

            {
                CompareValueChar pComparer = new CompareValueChar();
                ms_valueComparers.Add(typeof(char), pComparer);
            }

            {
                CompareValueFloat pComparer = new CompareValueFloat();
                ms_valueComparers.Add(typeof(float), pComparer);
            }
            {
                CompareValueDouble pComparer = new CompareValueDouble();
                ms_valueComparers.Add(typeof(double), pComparer);
            }
            {
                CompareValueString pComparer = new CompareValueString();
                ms_valueComparers.Add(typeof(string), pComparer);
            }
            {
                CompareValueObject pComparer = new CompareValueObject();
                ms_valueComparers.Add(typeof(object), pComparer);
            }
        }

        public static void Init()
        {
            if (ms_valueComparers == null)
            {
                ms_valueComparers = new Dictionary<Type, ICompareValue>();

                Register();
            }
        }

        public static void Cleanup()
        {
            ms_valueComparers.Clear();

            //ms_valueComparers.UnRegister();
            ms_valueComparers = null;
        }

        public static void RegisterType<T, TCOMPARER>() where TCOMPARER : ICompareValue, new()
        {
            TCOMPARER pComparer = new TCOMPARER();
            ms_valueComparers.Add(typeof(T), pComparer);
        }

        public static ICompareValue<T> Get<T>()
        {
            Type type = typeof(T);

            if (ms_valueComparers.ContainsKey(type))
            {
                ICompareValue pComparer = ms_valueComparers[type];
                return (ICompareValue<T>)pComparer;
            }

            return null;
        }

        public static ICompareValue Get(Type type)
        {
            if (ms_valueComparers.ContainsKey(type))
            {
                ICompareValue pComparer = ms_valueComparers[type];
                return pComparer;
            }

            return null;
        }

    }
    #endregion Compare

    #region Compute
    ///
    public interface IComputeValue
    {

    }

    public interface IComputeValue<T> : IComputeValue
    {
        T Add(T opr1, T opr2);

        T Sub(T opr1, T opr2);

        T Mul(T opr1, T opr2);

        T Div(T opr1, T opr2);
    }

    public class ComputeValueInt : IComputeValue<int>
    {
        public int Add(int lhs, int rhs)
        {
            int r = (lhs + rhs);
            return r;
        }

        public int Sub(int lhs, int rhs)
        {
            int r = (lhs - rhs);
            return r;
        }

        public int Mul(int lhs, int rhs)
        {
            int r = (lhs * rhs);
            return r;
        }


        public int Div(int lhs, int rhs)
        {
            int r = (lhs / rhs);
            return r;
        }

    }

    public class ComputeValueLong : IComputeValue<long>
    {
        public long Add(long lhs, long rhs)
        {
            long r = (lhs + rhs);
            return r;
        }

        public long Sub(long lhs, long rhs)
        {
            long r = (lhs - rhs);
            return r;
        }

        public long Mul(long lhs, long rhs)
        {
            long r = (lhs * rhs);
            return r;
        }


        public long Div(long lhs, long rhs)
        {
            long r = (lhs / rhs);
            return r;
        }

    }

    public class ComputeValueShort : IComputeValue<short>
    {
        public short Add(short lhs, short rhs)
        {
            short r = (short)(lhs + rhs);
            return r;
        }

        public short Sub(short lhs, short rhs)
        {
            short r = (short)(lhs - rhs);
            return r;
        }

        public short Mul(short lhs, short rhs)
        {
            short r = (short)(lhs * rhs);
            return r;
        }


        public short Div(short lhs, short rhs)
        {
            short r = (short)(lhs / rhs);
            return r;
        }

    }

    public class ComputeValueByte : IComputeValue<sbyte>
    {
        public sbyte Add(sbyte lhs, sbyte rhs)
        {
            sbyte r = (sbyte)(lhs + rhs);
            return r;
        }

        public sbyte Sub(sbyte lhs, sbyte rhs)
        {
            sbyte r = (sbyte)(lhs - rhs);
            return r;
        }

        public sbyte Mul(sbyte lhs, sbyte rhs)
        {
            sbyte r = (sbyte)(lhs * rhs);
            return r;
        }


        public sbyte Div(sbyte lhs, sbyte rhs)
        {
            sbyte r = (sbyte)(lhs / rhs);
            return r;
        }

    }

    public class ComputeValueUInt : IComputeValue<uint>
    {
        public uint Add(uint lhs, uint rhs)
        {
            uint r = (lhs + rhs);
            return r;
        }

        public uint Sub(uint lhs, uint rhs)
        {
            uint r = (lhs - rhs);
            return r;
        }

        public uint Mul(uint lhs, uint rhs)
        {
            uint r = (lhs * rhs);
            return r;
        }


        public uint Div(uint lhs, uint rhs)
        {
            uint r = (lhs / rhs);
            return r;
        }

    }

    public class ComputeValueULong : IComputeValue<ulong>
    {
        public ulong Add(ulong lhs, ulong rhs)
        {
            ulong r = (lhs + rhs);
            return r;
        }

        public ulong Sub(ulong lhs, ulong rhs)
        {
            ulong r = (lhs - rhs);
            return r;
        }

        public ulong Mul(ulong lhs, ulong rhs)
        {
            ulong r = (lhs * rhs);
            return r;
        }


        public ulong Div(ulong lhs, ulong rhs)
        {
            ulong r = (lhs / rhs);
            return r;
        }

    }

    public class ComputeValueUShort : IComputeValue<ushort>
    {
        public ushort Add(ushort lhs, ushort rhs)
        {
            ushort r = (ushort)(lhs + rhs);
            return r;
        }

        public ushort Sub(ushort lhs, ushort rhs)
        {
            ushort r = (ushort)(lhs - rhs);
            return r;
        }

        public ushort Mul(ushort lhs, ushort rhs)
        {
            ushort r = (ushort)(lhs * rhs);
            return r;
        }


        public ushort Div(ushort lhs, ushort rhs)
        {
            ushort r = (ushort)(lhs / rhs);
            return r;
        }

    }

    public class ComputeValueUByte : IComputeValue<byte>
    {
        public byte Add(byte lhs, byte rhs)
        {
            byte r = (byte)(lhs + rhs);
            return r;
        }

        public byte Sub(byte lhs, byte rhs)
        {
            byte r = (byte)(lhs - rhs);
            return r;
        }

        public byte Mul(byte lhs, byte rhs)
        {
            byte r = (byte)(lhs * rhs);
            return r;
        }


        public byte Div(byte lhs, byte rhs)
        {
            byte r = (byte)(lhs / rhs);
            return r;
        }

    }


    public class ComputeValueFloat : IComputeValue<float>
    {
        public float Add(float lhs, float rhs)
        {
            float r = (lhs + rhs);
            return r;
        }

        public float Sub(float lhs, float rhs)
        {
            float r = (lhs - rhs);
            return r;
        }

        public float Mul(float lhs, float rhs)
        {
            float r = (lhs * rhs);
            return r;
        }


        public float Div(float lhs, float rhs)
        {
            float r = (lhs / rhs);
            return r;
        }

    }

    public class ComputeValueDouble : IComputeValue<double>
    {
        public double Add(double lhs, double rhs)
        {
            double r = (lhs + rhs);
            return r;
        }

        public double Sub(double lhs, double rhs)
        {
            double r = (lhs - rhs);
            return r;
        }

        public double Mul(double lhs, double rhs)
        {
            double r = (lhs * rhs);
            return r;
        }


        public double Div(double lhs, double rhs)
        {
            double r = (lhs / rhs);
            return r;
        }

    }

    public class ComputerRegister
    {
        private static Dictionary<Type, IComputeValue> ms_valueComputers = null;

        private static void Register()
        {
            {
                ComputeValueInt pComparer = new ComputeValueInt();
                ms_valueComputers.Add(typeof(int), pComparer);
            }
            {
                ComputeValueLong pComparer = new ComputeValueLong();
                ms_valueComputers.Add(typeof(long), pComparer);
            }
            {
                ComputeValueShort pComparer = new ComputeValueShort();
                ms_valueComputers.Add(typeof(short), pComparer);
            }
            {
                ComputeValueByte pComparer = new ComputeValueByte();
                ms_valueComputers.Add(typeof(sbyte), pComparer);
            }

            {
                ComputeValueUInt pComparer = new ComputeValueUInt();
                ms_valueComputers.Add(typeof(uint), pComparer);
            }
            {
                ComputeValueULong pComparer = new ComputeValueULong();
                ms_valueComputers.Add(typeof(ulong), pComparer);
            }
            {
                ComputeValueUShort pComparer = new ComputeValueUShort();
                ms_valueComputers.Add(typeof(ushort), pComparer);
            }
            {
                ComputeValueUByte pComparer = new ComputeValueUByte();
                ms_valueComputers.Add(typeof(byte), pComparer);
            }

            {
                ComputeValueFloat pComparer = new ComputeValueFloat();
                ms_valueComputers.Add(typeof(float), pComparer);
            }
            {
                ComputeValueDouble pComparer = new ComputeValueDouble();
                ms_valueComputers.Add(typeof(double), pComparer);
            }

        }

        public static void Init()
        {
            if (ms_valueComputers == null)
            {
                ms_valueComputers = new Dictionary<Type, IComputeValue>();
                Register();
            }
        }

        public static void Cleanup()
        {
            ms_valueComputers.Clear();
            ms_valueComputers = null;
        }

        public static IComputeValue<T> Get<T>()
        {
            Type type = typeof(T);

            if (ms_valueComputers.ContainsKey(type))
            {
                IComputeValue pComparer = ms_valueComputers[type];
                return (IComputeValue<T>)pComparer;
            }

            Debug.Check(false);

            return null;
        }

    }

    #endregion

    public static class Operator<T>
    {
        public static T Add(T left, T right)
        {
            IComputeValue<T> c = ComputerRegister.Get<T>();
            Debug.Check(c != null);

            if (c != null)
            {
                return c.Add(left, right);
            }

            return default(T);
        }

        public static T Subtract(T left, T right)
        {
            IComputeValue<T> c = ComputerRegister.Get<T>();
            Debug.Check(c != null);

            if (c != null)
            {
                return c.Sub(left, right);
            }

            return default(T);
        }

        public static T Multiply(T left, T right)
        {
            IComputeValue<T> c = ComputerRegister.Get<T>();
            Debug.Check(c != null);

            if (c != null)
            {
                return c.Mul(left, right);
            }

            return default(T);
        }

        public static T Divide(T left, T right)
        {
            IComputeValue<T> c = ComputerRegister.Get<T>();
            Debug.Check(c != null);

            if (c != null)
            {
                return c.Div(left, right);
            }

            return default(T);
        }

        private static bool EqualByMember(T left, T right)
        {
            //not a list
            Debug.Check(!(left is IList));
            Type type = typeof(T);

            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            FieldInfo[] fields = type.GetFields(bindingFlags);

            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo field = fields[i];

                if (field.IsLiteral || field.IsInitOnly)
                {
                    continue;
                }

                ICompareValue c = ComparerRegister.Get(field.FieldType);

                if (c != null)
                {
                    bool bEqual = c.MemberEqual(left, right, field);

                    if (!bEqual)
                    {
                        return false;
                    }
                }
                else
                {
                    //Debug.LogWarning(string.Format("Type {0} Equal might cause GC, please provide ComparerRegister for it!", field.FieldType.Name));

                    object l = field.GetValue(left);
                    object r = field.GetValue(right);

                    if (l == null && r == null)
                    {
                        //both are null, they are equal
                    }
                    else if (l == null || r == null)
                    {
                        return false;
                    }
                    else
                    {
                        bool bEqual = l.Equals(r);

                        if (!bEqual)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool ClassEqual(T left, T right)
        {
            ICompareValue<T> c = ComparerRegister.Get<T>();

            if (c == null)
            {
                //class as a struct, it might cause low performance
                return EqualByMember(left, right);
            }

            //if ICompareValue<T> is generated and registered, it will run to here to avoid possible boxing
            return c.Equal(left, right);
        }

        public static bool ListEqual(T left, T right)
        {
            IList ll = (left as IList);

            if (ll != null)
            {
                IList rl = (right as IList);

                if (ll.Count != rl.Count)
                {
                    return false;
                }

                Type type = typeof(T);

                Type elementType = type.GetGenericArguments()[0];
                ICompareValue c = ComparerRegister.Get(elementType);

                if (c != null)
                {
                    for (int i = 0; i < ll.Count; ++i)
                    {
                        if (!c.ItemEqual(ll, rl, i))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool Equal(T left, T right)
        {
            ICompareValue<T> c = ComparerRegister.Get<T>();

            if (c == null)
            {
                Type type = typeof(T);

                if (!type.IsValueType)
                {
                    ICompareValue<object> co = ComparerRegister.Get<object>();

                    if (co != null)
                    {
                        return co.Equal(left, right);
                    }
                }
                else
                {
                    //Debug.LogWarning(string.Format("Type {0} Equal might cause GC, please provide ComparerRegister for it!", typeof(T).Name));
                    return left.Equals(right);
                }
            }

            Debug.Check(c != null);

            if (c != null)
            {
                return c.Equal(left, right);
            }

            return false;
        }

        public static bool NotEqual(T left, T right)
        {
            ICompareValue<T> c = ComparerRegister.Get<T>();

            if (c == null)
            {
                Type type = typeof(T);

                if (!type.IsValueType)
                {
                    ICompareValue<object> co = ComparerRegister.Get<object>();

                    if (co != null)
                    {
                        return co.NotEqual(left, right);
                    }
                }
                else
                {
                    //Debug.LogWarning(string.Format("Type {0} Equal might cause GC, please provide ComparerRegister for it!", typeof(T).Name));
                    return !left.Equals(right);
                }
            }

            Debug.Check(c != null);

            if (c != null)
            {
                return c.NotEqual(left, right);
            }

            return false;
        }

        public static bool GreaterThan(T left, T right)
        {
            ICompareValue<T> c = ComparerRegister.Get<T>();
            Debug.Check(c != null);

            if (c != null)
            {
                return c.Greater(left, right);
            }

            return false;
        }

        public static bool GreaterThanOrEqual(T left, T right)
        {
            ICompareValue<T> c = ComparerRegister.Get<T>();
            Debug.Check(c != null);

            if (c != null)
            {
                return c.GreaterEqual(left, right);
            }

            return false;
        }

        public static bool LessThan(T left, T right)
        {
            ICompareValue<T> c = ComparerRegister.Get<T>();
            Debug.Check(c != null);

            if (c != null)
            {
                return c.Less(left, right);
            }

            return false;
        }

        public static bool LessThanOrEqual(T left, T right)
        {
            ICompareValue<T> c = ComparerRegister.Get<T>();
            Debug.Check(c != null);

            if (c != null)
            {
                return c.LessEqual(left, right);
            }

            return false;
        }

    }

    public enum EOperatorType
    {
        E_INVALID,
        E_ASSIGN,        // =
        E_ADD,           // +
        E_SUB,           // -
        E_MUL,           // *
        E_DIV,           // /
        E_EQUAL,         // ==
        E_NOTEQUAL,      // !=
        E_GREATER,       // >
        E_GREATEREQUAL,  // >=
        E_LESS,          // <
        E_LESSEQUAL      // <=
    }

    public static class OperationUtils
    {
        public static EOperatorType ParseOperatorType(string operatorType)
        {
            switch (operatorType)
            {
                case "Invalid":
                    return EOperatorType.E_INVALID;

                case "Assign":
                    return EOperatorType.E_ASSIGN;

                case "Assignment":
                    return EOperatorType.E_ASSIGN;

                case "Add":
                    return EOperatorType.E_ADD;

                case "Sub":
                    return EOperatorType.E_SUB;

                case "Mul":
                    return EOperatorType.E_MUL;

                case "Div":
                    return EOperatorType.E_DIV;

                case "Equal":
                    return EOperatorType.E_EQUAL;

                case "NotEqual":
                    return EOperatorType.E_NOTEQUAL;

                case "Greater":
                    return EOperatorType.E_GREATER;

                case "GreaterEqual":
                    return EOperatorType.E_GREATEREQUAL;

                case "Less":
                    return EOperatorType.E_LESS;

                case "LessEqual":
                    return EOperatorType.E_LESSEQUAL;
            }

            Debug.Check(false);
            return EOperatorType.E_INVALID;
        }

        public static bool Compare<T>(T left, T right, EOperatorType comparisonType)
        {
            Type type = typeof(T);
            if (!type.IsValueType)
            {
                bool bLeftNull = (left == null);
                bool bRightNull = (right == null);

                if (bLeftNull && bRightNull) // both are null
                {
                    if (comparisonType == EOperatorType.E_EQUAL)
                    {
                        return true;
                    }
                    else if (comparisonType == EOperatorType.E_NOTEQUAL)
                    {
                        return false;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
                else if (bLeftNull || bRightNull) // one is null and the other is not null
                {
                    if (comparisonType == EOperatorType.E_EQUAL)
                    {
                        return false;
                    }
                    else if (comparisonType == EOperatorType.E_NOTEQUAL)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
            }

            ICompareValue<T> c = ComparerRegister.Get<T>();

            if (c != null)
            {
                switch (comparisonType)
                {
                    case EOperatorType.E_EQUAL:
                        return Operator<T>.Equal(left, right);

                    case EOperatorType.E_NOTEQUAL:
                        return Operator<T>.NotEqual(left, right);

                    case EOperatorType.E_GREATER:
                        return Operator<T>.GreaterThan(left, right);

                    case EOperatorType.E_GREATEREQUAL:
                        return Operator<T>.GreaterThanOrEqual(left, right);

                    case EOperatorType.E_LESS:
                        return Operator<T>.LessThan(left, right);

                    case EOperatorType.E_LESSEQUAL:
                        return Operator<T>.LessThanOrEqual(left, right);
                }
            }

            if (!type.IsValueType)
            {
                bool bEqual = Object.ReferenceEquals(left, right);

                if (bEqual)
                {
                    // not equal then, continue check its members below
                    return true;
                }
            }

            if (Utils.IsCustomStructType(type))
            {
                bool bEqual = Operator<T>.ClassEqual(left, right);

                switch (comparisonType)
                {
                    case EOperatorType.E_EQUAL:
                        return bEqual;

                    case EOperatorType.E_NOTEQUAL:
                        return !bEqual;
                }
            }
            else if (Utils.IsArrayType(type))
            {
                bool bEqual = Operator<T>.ListEqual(left, right);

                switch (comparisonType)
                {
                    case EOperatorType.E_EQUAL:
                        return bEqual;

                    case EOperatorType.E_NOTEQUAL:
                        return !bEqual;
                }
            }
            else if (Utils.IsEnumType(type))
            {
                switch (comparisonType)
                {
                    case EOperatorType.E_EQUAL:
                        return Operator<T>.Equal(left, right);

                    case EOperatorType.E_NOTEQUAL:
                        return Operator<T>.NotEqual(left, right);
                        //case EOperatorType.E_GREATER: return Operator<int>.GreaterThan(iLeft, iRight);
                        //case EOperatorType.E_GREATEREQUAL: return Operator<int>.GreaterThanOrEqual(iLeft, iRight);
                        //case EOperatorType.E_LESS: return Operator<int>.LessThan(iLeft, iRight);
                        //case EOperatorType.E_LESSEQUAL: return Operator<int>.LessThanOrEqual(iLeft, iRight);
                }

                Debug.Check(false, "enum only supports eq or neq");
            }
            else
            {
                Debug.Check(false, "unsupported type for compare");
            }

            return false;
        }

        public static T Compute<T>(T left, T right, EOperatorType computeType)
        {
            Type type = typeof(T);

            if (!Utils.IsAgentType(type) && !Utils.IsCustomClassType(type) && !Utils.IsCustomStructType(type) && !Utils.IsArrayType(type) && !Utils.IsStringType(type))
            {
                if (Utils.IsEnumType(type) || type == typeof(char))
                {
                    Debug.Check(false);
                    //int iLeft = Convert.ToInt32(left);
                    //int iRight = Convert.ToInt32(right);

                    //switch (computeType)
                    //{
                    //    case EOperatorType.E_ADD: return (T)(object)Operator<int>.Add(iLeft, iRight);
                    //    case EOperatorType.E_SUB: return (T)(object)Operator<int>.Subtract(iLeft, iRight);
                    //    case EOperatorType.E_MUL: return (T)(object)Operator<int>.Multiply(iLeft, iRight);
                    //    case EOperatorType.E_DIV: return (T)(object)Operator<int>.Divide(iLeft, iRight);
                    //}
                }
                else
                {
                    switch (computeType)
                    {
                        case EOperatorType.E_ADD:
                            return Operator<T>.Add(left, right);

                        case EOperatorType.E_SUB:
                            return Operator<T>.Subtract(left, right);

                        case EOperatorType.E_MUL:
                            return Operator<T>.Multiply(left, right);

                        case EOperatorType.E_DIV:
                            return Operator<T>.Divide(left, right);
                    }
                }
            }

            Debug.Check(false);
            return left;
        }
    }

    public class ValueConverter<T>
    {
        public static bool Convert(string valueStr, out T result)
        {
            result = (T)StringUtils.FromString(typeof(T), valueStr, false);

            return true;
        }
    }
}//namespace behaviac
