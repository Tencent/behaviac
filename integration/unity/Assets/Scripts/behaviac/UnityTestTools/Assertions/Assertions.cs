using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityTest
{
    public static class Assertions
    {
        public static void CheckAssertions() {
            var assertions = Object.FindObjectsOfType(typeof(AssertionComponent)) as AssertionComponent[];
            CheckAssertions(assertions);
        }

        public static void CheckAssertions(AssertionComponent assertion) {
            CheckAssertions(new[] {assertion});
        }

        public static void CheckAssertions(GameObject gameObject) {
            CheckAssertions(gameObject.GetComponents<AssertionComponent> ());
        }

        public static void CheckAssertions(AssertionComponent[] assertions)
        {
            if (!Debug.isDebugBuild)
                return;

            for (int i = 0; i < assertions.Length; ++i)
            {
                var assertion = assertions[i];
                assertion.checksPerformed++;
                var result = assertion.Action.Compare();

                if (!result)
                {
                    assertion.hasFailed = true;
                    assertion.Action.Fail(assertion);
                }
            }
        }
    }
}
