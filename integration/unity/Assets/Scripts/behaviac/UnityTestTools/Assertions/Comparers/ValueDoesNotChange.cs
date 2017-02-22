namespace UnityTest
{
    public class ValueDoesNotChange : ActionBase
    {
        private object val = null;

        protected override bool Compare(object a) {
            if (val == null)
            { val = a; }

            if (!val.Equals(a))
            { return false; }

            return true;
        }
    }
}
