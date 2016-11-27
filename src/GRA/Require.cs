using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA
{
    public static class Require
    {
        // easier null checking, code found at http://stackoverflow.com/a/26335162
        // originally created by Steven https://www.cuttingedge.it/blogs/steven/
        public static T IsNotNull<T>(T instance, string paramName) where T : class
        {
            // Use ReferenceEquals in case T overrides equals.
            if (object.ReferenceEquals(null, instance)) {
                // Call a method that throws instead of throwing directly. This allows
                // this IsNotNull method to be inlined.
                ThrowArgumentNullException(paramName);
            }

            return instance;
        }

        private static void ThrowArgumentNullException(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
