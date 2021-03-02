using System;
using System.Collections.Generic;
using System.Linq;

namespace Fruberry {
    public static class Linq {
        public static bool None<T>(this IEnumerable<T> source) {
            if (source == null) return true;

            return !source.Any();
        }

        public static bool Contains(this IEnumerable<IEnumerable<char>> source, string match) {
            foreach (var sequence in source) {
                var iterator = sequence.GetEnumerator();

                iterator.MoveNext();

                for (int i = 0; i < match.Length; i++) {                 
                    if (iterator.Current != match[i]) break;

                    if (iterator.MoveNext() == false) {
                        if (i == match.Length - 1) return true;
                        else break;
                    }
                }
            }

            return false;
        }

        //public static int Count(this Array source) {
        //    return source.Length;
        //}

        public static bool Contains(this List<IEnumerable<char>> source, string match) {

            return ((IEnumerable<IEnumerable<char>>)source).Contains(match);
        }

        public static bool Contains(this List<List<char>> source, string match) {

            return ((IEnumerable<IEnumerable<char>>)source).Contains(match);
        }

        public static bool Contains(this IEnumerable<List<char>> source, string match) {

            return ((IEnumerable<IEnumerable<char>>)source).Contains(match);
        }

        public static bool IsEqual(this IEnumerable<char> source, string match) {
            var current = source.GetEnumerator();

            for (int i = 0; i < match.Length; i++) {
                if (current.Current != match[i]) return false;
            }

            return true;
        }
    }
}
