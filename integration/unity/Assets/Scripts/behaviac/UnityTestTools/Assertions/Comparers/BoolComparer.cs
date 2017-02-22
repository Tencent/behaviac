namespace UnityTest
{
    public class BoolComparer : ComparerBaseGeneric<bool>
    {
        protected override bool Compare(bool a, bool b) {
            return a == b;
        }
    }
}
