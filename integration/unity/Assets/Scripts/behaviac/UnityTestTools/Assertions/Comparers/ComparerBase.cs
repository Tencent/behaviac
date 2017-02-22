using System;
using UnityEngine;
using Object = System.Object;

namespace UnityTest
{
    public abstract class ComparerBase : ActionBase
    {
        public enum CompareToType {
            CompareToObject,
            CompareToConstantValue,
            CompareToNull
        }

        public CompareToType compareToType = CompareToType.CompareToObject;

        public GameObject other;
        protected object objOtherVal;
        public string otherPropertyPath = "";
        private MemberResolver memberResolverB;

        protected abstract bool Compare(object a, object b);

        protected override bool Compare(object objVal) {
            if (compareToType == CompareToType.CompareToConstantValue) {
                objOtherVal = ConstValue;

            } else if (compareToType == CompareToType.CompareToNull) {
                objOtherVal = null;

            } else {
                if (other == null)
                { objOtherVal = null; }

                else {
                    if (memberResolverB == null)
                    { memberResolverB = new MemberResolver(other, otherPropertyPath); }

                    objOtherVal = memberResolverB.GetValue(UseCache);
                }
            }

            return Compare(objVal, objOtherVal);
        }

        public virtual Type[] GetAccepatbleTypesForB() {
            return null;
        }

        #region Const value

        public virtual object ConstValue { get; set; }
        public virtual object GetDefaultConstValue() {
            throw new NotImplementedException();
        }

        #endregion

        public override string GetFailureMessage() {
            var message = GetType().Name + " assertion failed.\n" + go.name + "." + thisPropertyPath + " " + compareToType;

            switch (compareToType) {
                case ComparerBase.CompareToType.CompareToObject:
                    message += " (" + other + ")." + otherPropertyPath + " failed.";
                    break;

                case ComparerBase.CompareToType.CompareToConstantValue:
                    message += " " + ConstValue + " failed.";
                    break;

                case ComparerBase.CompareToType.CompareToNull:
                    message += " failed.";
                    break;
            }

            message += " Expected: " + objOtherVal + " Actual: " + objVal;
            return message;
        }
    }

    [Serializable]
    public abstract class ComparerBaseGeneric<T> : ComparerBaseGeneric<T, T>
    {
    }

    [Serializable]
    public abstract class ComparerBaseGeneric<T1, T2> : ComparerBase
    {
        public T2 constantValueGeneric = default(T2);

        public override Object ConstValue {
            get {
                return constantValueGeneric;
            }
            set {
                constantValueGeneric = (T2) value;
            }
        }

        public override Object GetDefaultConstValue() {
            return default(T2);
        }

        static bool IsValueType(Type type) {
#if !UNITY_METRO
            return type.IsValueType;
#else
            return false;
#endif
        }

        protected override bool Compare(object a, object b) {
            var type = typeof(T2);

            if (b == null && IsValueType(type)) {
                throw new ArgumentException("Null was passed to a value-type argument");
            }

            return Compare((T1)a, (T2)b);
        }

        protected abstract bool Compare(T1 a, T2 b);

        public override Type[] GetAccepatbleTypesForA() {
            return new[] { typeof(T1) };
        }

        public override Type[] GetAccepatbleTypesForB() {
            return new[] {typeof(T2)};
        }

        protected override bool UseCache { get { return true; } }
    }
}
