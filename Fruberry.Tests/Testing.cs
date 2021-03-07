using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fruberry.Tests {
    public static class Check {
        public static void Contains<T>(IEnumerable<T> actual, T expected, string message = null, params object[] args) {
            if (actual == null || !actual.Any(_ => _ == null && expected == null || _.Equals(expected) == false)) {
                Assert.Fail(message, args);
            }

            return;
        }
    }
}
