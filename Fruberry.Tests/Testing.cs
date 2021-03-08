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

    public static class TestingUtil {
        public static string GenerateRandomString(int length) {
            var random = new Random((int)DateTime.Now.Ticks);

            var result = new char[length];

            for (var i = 0; i < result.Length; i++) {
                result[i] = (char)random.Next(33, 127);
            }

            return new string(result);
        }

        public static string GenerateRandomLetters(int length) {
            var random = new Random((int)DateTime.Now.Ticks);

            var result = new char[length];

            for (var i = 0; i < result.Length; i++) {
                result[i] = (char)random.Next(97, 123);
            }

            return new string(result);
        }
    }
}
