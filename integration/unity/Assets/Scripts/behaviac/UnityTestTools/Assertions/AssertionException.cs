using System;

namespace UnityTest
{
    public class AssertionException : Exception
    {
        private AssertionComponent assertion;

        public AssertionException(AssertionComponent assertion) : base(assertion.Action.GetFailureMessage()) {
            this.assertion = assertion;
        }

        public override string StackTrace {
            get {
                return "Created in " + assertion.GetCreationLocation();
            }
        }
    }
}
