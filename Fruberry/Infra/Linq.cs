﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Fruberry {
    public static class Linq {
        public static Random RandomGenerator = new Random();

        public static bool None<T>(this IEnumerable<T> source) {
            if (source == null) return true;

            return !source.Any();
        }

        /// <summary>
        /// Select a random element from an enumerable
        /// </summary>
        public static T Random<T>(this IEnumerable<T> enumerable) {
            if (enumerable?.Any() != true) {
                return default;
            }

            var count = enumerable.Count();

            return count == 1 ? enumerable.First() : enumerable.ElementAt(RandomGenerator.Next(0, count));
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

        public static bool Contains<T>(this ICollection<T> source, string match) where T : IEnumerable<char> {

            return ((IEnumerable<IEnumerable<char>>)source).Contains(match);
        }

        public static bool Contains<T>(this IEnumerable<T> source, string match) where T : IEnumerable<char> {

            return ((IEnumerable<IEnumerable<char>>)source).Contains(match);
        }

        public static bool Contains(this IList<IEnumerable<char>> source, string match) {

            return ((IEnumerable<IEnumerable<char>>)source).Contains(match);
        }

        public static bool Contains(this List<List<char>> source, string match) {

            return ((IEnumerable<IEnumerable<char>>)source).Contains(match);
        }

        public static bool Contains(this IEnumerable<List<char>> source, string match) {

            return ((IEnumerable<IEnumerable<char>>)source).Contains(match);
        }

        public static bool One<T>(this IEnumerable<T> source, Func<T, bool> selector) {
            if (source.Count() == 1 && selector(source.First())) return true;

            return false;
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
