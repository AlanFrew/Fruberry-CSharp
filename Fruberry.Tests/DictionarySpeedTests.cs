using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fruberry.Tests {
    public class DictionarySpeedTests {
        private int count = 50000;
        [Test]
        public void DictionaryVsListSpeed() {
            var dictionary = new Dictionary<Version, Version>();
            var list = new List<Version>();
            var tree = new RedBlackTree<Version>();

            IStructure<Version>.Compare = (a, b) => a.Major - b.Major;

            var stopwatch = new Stopwatch();

            var inputs = new List<Version>(count);

            for (int i = 0; i < count; i++) {
                inputs.Add(new Version(i, i));
            }

            stopwatch.Start();

            for (int i = 0; i < count; i++) {
                var entry = inputs[i];
                dictionary.Add(entry, entry);
            }

            //for (int i = 0; i < count; i++) {
            //    var entry = inputs[i];
            //    _ = dictionary[entry];
            //}

            for (int i = 0; i < count; i++) {
                var entry = inputs[i];
                dictionary.Remove(entry);
            }

            stopwatch.Stop();

            var stopwatch2 = new Stopwatch();
            stopwatch2.Start();

            for (int i = 0; i < count; i++) {
                var entry = inputs[i];
                list.Add(entry);
            }

            //for (int i = 0; i < count; i++) {
            //    var entry = inputs[i];
            //    _ = list.Contains(entry);
            //}

            for (int i = 0; i < count; i++) {
                var entry = inputs[i];
                list.Remove(entry);
            }

            stopwatch2.Stop();

            var stopwatch3 = new Stopwatch();
            stopwatch3.Start();

            for (int i = count - 1; i >= 0; i--) {
                var entry = inputs[i];
                tree.Add(entry);
            }

            for (int i = 0; i < count; i++) {
                var entry = inputs[i];
                tree.Contains(entry);
            }

            for (int i = count - 1; i >= 0; i--) {
                var entry = inputs[i];
                tree.Remove(entry);
            }

            stopwatch3.Stop();

            Console.WriteLine($"dictionary: {stopwatch.ElapsedMilliseconds} list: {stopwatch2.ElapsedMilliseconds} tree: {stopwatch3.ElapsedMilliseconds}");
        }

        [Test]
        public void DictionaryVsListPreallocatedSpeed() {
            var dictionary = new Dictionary2<object, object>(count);
            var list = new List<object>(count);
            var stopwatch = new Stopwatch();

            var inputs = new List<object>(count);

            for (int i = 0; i < count; i++) {
                inputs.Add(new object());
            }

            stopwatch.Start();

            for (int i = 0; i < count; i++) {
                var entry = inputs[i];
                dictionary.Add(entry, entry);
            }

            stopwatch.Stop();

            var stopwatch2 = new Stopwatch();
            stopwatch2.Start();

            for (int i = 0; i < count; i++) {
                var entry = inputs[i];
                list.Add(i);
            }

            stopwatch2.Stop();

            Console.WriteLine($"dictionary: {stopwatch.ElapsedMilliseconds} list: {stopwatch2.ElapsedMilliseconds}");
        }

        [Test]
        public void AddSpeed() {
            var dictionary = new Dictionary2<object, object>(count);
            var list = new List<object>(count);
            var chain = new Chain<object>();

            var stopwatch = new Stopwatch();

            var inputs = new List<object>(count);

            for (var i = 0; i < count; i++) {
                inputs.Add(new object());
            }

            stopwatch.Start();

            for (var i = 0; i < count; i++) {
                var entry = inputs[i];
                dictionary.Add(entry, entry);
            }

            stopwatch.Stop();

            var stopwatch2 = new Stopwatch();
            stopwatch2.Start();

            for (int i = 0; i < count; i++) {
                var entry = inputs[i];
                list.Add(entry);
            }

            stopwatch2.Stop();

            var stopwatch3 = new Stopwatch();
            stopwatch3.Start();

            for (var i = 0; i < count; i++) {
                var entry = inputs[i];
                chain.Add(entry);
            }

            stopwatch3.Stop();

            Console.WriteLine($"dictionary: {stopwatch.ElapsedMilliseconds} list: {stopwatch2.ElapsedMilliseconds} chain: {stopwatch3.ElapsedMilliseconds}");
        }
    }
}
