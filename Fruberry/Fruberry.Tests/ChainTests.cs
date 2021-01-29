using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BusyBeaver {
    public class ChainTests {
        [Test]
        public void ChainAdd() {
            var chain = new Chain<string>();

            foreach (var link in chain) { } //assert that enumeration works properly

            foreach (var value in (IEnumerable)chain) { } //assert that enumeration works properly

            Assert.That(chain.Count == 0);

            Verify(chain, false);

            var foo = "foo";
            chain.AddLast(foo);

            Assert.That(chain.First == foo);
            Assert.That(chain.Last == foo);
            Assert.That(chain.Count == 1);
            Assert.That(chain.Length == 1);

            Verify(chain, false);

            chain.Remove(chain.Head);

            Assert.That(chain.Count == 0);

            Verify(chain, false);

            var bar = "bar";
            chain.AddLast(foo);
            chain.AddLast(bar);

            Assert.That(chain.Count == 2);
            Assert.That(chain.First == foo);
            Assert.That(chain.Last == bar);

            Verify(chain, false);

            chain.Remove(chain.Tail);

            Assert.That(chain.Count == 1);
            Assert.That(chain.First == foo);
            Assert.That(chain.Last == foo);

            Verify(chain, false);

            chain.AddLast(bar);
            chain.Remove(chain.Head);

            Assert.That(chain.Count == 1);
            Assert.That(chain.First == bar);
            Assert.That(chain.Last == bar);

            Verify(chain, false);

            chain.Remove(chain.Head);

            var baz = "baz";
            chain.AddLast(foo);
            chain.AddLast(bar);
            chain.AddLast(baz);

            var counted = 0;
            foreach (var value in (IEnumerable)chain) {
                Assert.That(value is string);
                counted++;
            }

            Assert.That(counted == 3);

            Verify(chain, false);

            var chain2 = new Chain<string>();
            chain2.AddLast(foo);
            chain2.AddLast(bar);
            chain2.AddLast(baz);

            Chain<string>.Enumerator<string> iterator = chain2.GetEnumerator(); //explicit type to enforce correct overload

            var counted2 = 0;
            while (iterator.MoveNext()) {
                if (iterator.Current.Value == bar) { }
                counted2++;
            }

            Assert.That(counted2 == 3);

            foreach (var link in chain2) {
                if (link.Value == bar) chain2.Remove(link);
            }

            Assert.That(chain2.Length == 2);
            Assert.That(chain2.First == foo);
            Assert.That(chain2.Last == baz);

            Verify(chain2, false);

            var list = new List<string>(chain2);

            Assert.That(list.Count == 2);
            Assert.That(list.First() == foo);
            Assert.That(list.Last() == baz);

            var blab = "blab";
            chain2.AddLast(bar);
            chain2.AddLast(blab);

            var removedCount = 0;
            foreach (var link in chain2) {
                chain2.Remove(link);
                removedCount++;
            }

            Assert.That(chain2.Count == 0);
            Assert.That(removedCount == 4);

            Verify(chain2, false);
        }

        [Test]
        public void ChainRemove() {
            var chain = new Chain<int> { 1, 2 };
            chain.Remove(chain.Head);

            Assert.That(chain.Count == 1);
            Assert.That(chain.Head.Value == 2);

            Verify(chain, true);
        }

        [Test]
        public void ChainSort() {
            var chain = new Chain<int>();
            Chain<int>.Comparer = (left, right) => left <= right;

            chain.Add(1);

            chain.Sort();

            Assert.That(chain.Count == 1);
            Assert.That(chain.First == 1);
            Assert.That(chain.Last == 1);

            Verify(chain, true);

            chain.Add(0);

            chain.Sort();

            Assert.That(chain.Count == 2);
            Assert.That(chain.First == 0);
            Assert.That(chain.Last == 1);

            Verify(chain, true);

            chain.Add(-1);

            chain.Sort();

            Assert.That(chain.Count == 3);
            Assert.That(chain.First == -1);
            Assert.That(chain.Last == 1);
            Assert.That(chain.Head.Next.Value == 0);
            Assert.That(chain.Tail.Previous.Value == 0);

            Verify(chain, true);

            var index = 0;
            var current = chain.Head;
            while (current != chain.Tail) {
                current = current.Next;
                index++;
            }

            Assert.That(index == chain.Length - 1);

            chain.Sort(); //no-op

            Assert.That(chain.Count == 3);
            Assert.That(chain.First == -1);
            Assert.That(chain.Last == 1);
            Assert.That(chain.Head.Next.Value == 0);
            Assert.That(chain.Tail.Previous.Value == 0);

            Verify(chain, true);

            var chain2 = new Chain<string>();
            Chain<string>.Comparer = (left, right) => left.CompareTo(right) <= 0;

            chain2.Add("delta");
            chain2.Add("charlie");
            chain2.Add("bravo");
            chain2.Add("alpha");

            chain2.Sort();

            Assert.That(chain2.Count == 4);
            Assert.That(chain2.First == "alpha");
            Assert.That(chain2.Last == "delta");
            Assert.That(chain2.Head.Next.Value == "bravo");
            Assert.That(chain2.Tail.Previous.Value == "charlie");

            Verify(chain2, true);

            var chain4 = new Chain<int>();
            chain4.Add(2);
            chain4.Add(2);
            chain4.Add(2);
            chain4.Add(1);
            chain4.Add(1);
            chain4.Add(1);

            chain4.Sort();

            Assert.That(chain4.Count == 6);
            Assert.That(chain4.First == 1);
            Assert.That(chain4.Last == 2);

            Verify(chain4, true);

            //var simpleLoop = new TopLoop("1A ", 0, new Machine());
            //var nestedLoop = new TopLoop("1A ", 0, new Machine());
            //nestedLoop.Parts.Add(new LoopPart(0, 0, 3, nestedLoop, 0), nestedLoop, false);

            //var chain5 = new Chain<Loop> { simpleLoop, nestedLoop, simpleLoop.Copy(null), nestedLoop.Copy(null) };

            //var chainDistinct = new Chain<Loop>(chain5.Distinct().OrderBy(loop => loop.LoopText));
            //Chain<Loop>.Comparer = (left, right) => left.LoopText.CompareTo(right.LoopText) <= 0;
            //var chainSorted = chain5.Sort();
            //chainSorted.Dedupe();

            //Assert.That(chainDistinct.Count() == chainSorted.Count);

            //var i = 0;
            //foreach (var loop in chainDistinct) {
            //    Assert.AreEqual(loop.Value, chainSorted[i]);
            //    Assert.AreEqual(loop.Value.GetHashCode(), chainSorted[i].GetHashCode());
            //    i++;
            //}
        }

        [Test]
        public void ChainSortPerformance() {
            var random = new Random();
            //var chain3 = new Chain<int>() {
            //    700162, 752258, 885954, 852302, 592291, 801061, 344432, 143904, 346915, 881418
            //};

            var chain3 = new Chain<int>();

            for (var i = 0; i < 10000; i++) {
                chain3.Add(random.Next(0, 100000));
            }

            Debug.WriteLine(chain3.Print());
            Console.WriteLine(chain3.Print());

            var timer1 = new Stopwatch();
            timer1.Start();
            var sorted = chain3.OrderBy(value => value);
            var foo = sorted.First();
            timer1.Stop();

            Verify(new Chain<int>(sorted), true);

            var timer2 = new Stopwatch();
            timer2.Start();
            chain3.Sort();
            timer2.Stop();

            Verify(chain3, true);

            var timer4 = new Stopwatch();
            timer4.Start();
            sorted = chain3.OrderBy(value => value);
            foo = sorted.First();
            timer4.Stop();

            var timer3 = new Stopwatch();
            timer3.Start();
            chain3.Sort();
            timer3.Stop();
        }

        [Test]
        public void ChainSortLarge() {
            var random = new Random();
            var chain3 = new Chain<string>() {
               "0C 0A 0B 1A 0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E ", "0C 0A 0B 1A 0A 1B 1C ", "0A 0B 1A 0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 0C ", "0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C ", "0C 0A 0B 1A 0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E ", "0C 0A 0B 1A 0A 1B 1C 1D 1E 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E ", "0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C ", "0B 1A 0A 1B 1C ", "1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B ", "1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "1E 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A ", "0B 1A 0A 1B 1C 1D 1E 0A 1B 1C 0D 0A 0B 1A 0A 1B 0C 0A ", "0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A ", "0B 1A 0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C ", "1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A ", "1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B ", "0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A ", "0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A ", "1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A ", "1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B ", "0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A ", "1A 0A 1B 1C 1D 1E 0A 1B 0C 0A 0B ", "1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A ", "1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B ", "1A 0A 1B 1C ", "1A 0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B ", "0A 1B 1C ", "0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A ", "0A 0B 1A 0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A ", "0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A ", "1B 1C ", "0A 0B 1A 0A 1B 1C ", "1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A ", "1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A ", "1C ", "1B 0C 0A 0B 1A 0A 1B 1C ", "1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C ", "0A 0B 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "0A 1B 0C 0A 0B 1A 0A 1B 1C ", "1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B ", "1C 1D 1E 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A ", "0B 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C 1D 1E 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A ", "1B 0C 0A 0B 1A 0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A ", "0A 1B 0C 0A 0B 1A 0A 1B 1C 1D 1E 0A 0B 1A 0A 1B 1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B 1C ", "1C 0D 0A 0B 1A 0A 1B 1C 1D 1E 1A 0A 1B 0C 0A 0B 1A 0A 1B "
            };

            //var chain3 = new Chain<int>();

            //for (var i = 0; i < 10000; i++) {
            //    chain3.Add(random.Next(0, 100000));
            //}

            Debug.WriteLine(chain3.Print());
            Console.WriteLine(chain3.Print());

            var timer1 = new Stopwatch();
            timer1.Start();
            var sorted = chain3.OrderBy(value => value);
            timer1.Stop();

            var timer2 = new Stopwatch();
            timer2.Start();
            chain3.Sort();
            timer2.Stop();

            chain3.Sort();
            chain3.Sort();

            var current = chain3.Head;
            while (current.Next != null) {
                Assert.That(current.Value.CompareTo(current.Next.Value) <= 0);
                current = current.Next;
            }

            Verify(chain3, true);
        }

        private bool Verify<T>(Chain<T> chain, bool isSorted) {
            if (chain.Count == 0) {
                Assert.That(chain.Head == null);
                Assert.That(chain.Tail == null);
                return true;
            }

            Assert.That(chain.Head.Previous == null);
            Assert.That(chain.Tail.Next == null);

            if (chain.Count == 1) {
                Assert.That(chain.Head == chain.Tail);
                return true;
            }

            var seen = new HashSet<Chainlink<T>>();
            var current = chain.Head;
            var i = 0;
            for (; i < chain.Count; i++) {
                Assert.That(current.Next != null && current.Next.Previous == current || current == chain.Tail);
                Assert.That(current.Previous != null && current.Previous.Next == current || current == chain.Head);

                if (isSorted) {
                    Assert.That(current == chain.Tail || Chain<T>.Comparer(current.Value, current.Next.Value));
                    Assert.That(current == chain.Head || Chain<T>.Comparer(current.Previous.Value, current.Value));
                }

                Assert.That(seen.Add(current));

                if (i < chain.Count - 1) current = current.Next;
            }

            Assert.That(current == chain.Tail);

            return true;
        }

        [Test]
        public void ChainDedupe() {
            var chain = new Chain<string> { "one", "two", "one", "two", "three", "two", "one", "four" };
            chain.Sort();
            chain.Dedupe();

            Assert.That(chain.Count == 4);
            Assert.That(chain.First == "four");
            Assert.That(chain.Head.Next.Value == "one");
            Assert.That(chain.Tail.Previous.Value == "three");
            Assert.That(chain.Last == "two");

            var chain4 = new Chain<string> {
                "0A ", "0A 0B 1A 0B 0A 1B 1C 0D 1D 1A 0B ", "0A 0B 1A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 0B 1A 1B 1C 1D 1A 0B ", "0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 0A 1B 1C 0D 1D ", "0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A ", "0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D ", "0A 1B 1C 0D 1D ", "0A 1B 1C 0D 1D 0A 0B 1A ", "0A 1B 1C 0D 1D 0A 0B 1A 1B ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 1A ", "0A 1B 1C 0D 1D 0A 1B 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 1B 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 1B 1C 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 1B 1C 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A ", "0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 0D 1D 0A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B ", "0A 1B 1C 1D 1A 1B 1C 0D 1D 0A 0B 1A 0B 0A ", "0B 0A ", "0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A ", "0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A ", "0B 0A 1B 1C 0D 1D 0A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A ", "0B 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "0B 1A 0B 1A 1B 1C 1D 1A 0B 0A ", "0B 1A 1B 1C 1D 1A 0B 0A ", "0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B ", "0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 0B 0A ", "0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C ", "0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0B 1A 1B 1C 1D 1A 0B 0A ", "0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C ", "1A 0B 0A ", "1A 0B 0A 1B 1C 0D 1D 0A 0B ", "1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D ", "1A 0B 0A 1B 1C 0D 1D 0A 1B 1C ", "1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B ", "1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A ", "1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B ", "1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C ", "1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D ", "1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D ", "1A 0B 1A 1B 1C 0D 1D 0A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B ", "1A 1B 1C 0D 1D 0A 0B 1A 0B 0A 1B 1C 0D 1D 1A 0B 0A ", "1A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D ", "1A 1B 1C 1D 1A 0B 0A ", "1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A ", "1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 0B ", "1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B ", "1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C ", "1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D ", "1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C ", "1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B ", "1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C ", "1B 1C 0D 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A ", "1B 1C 0D 1D 0A 0B 1A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B ", "1B 1C 0D 1D 0A 1B 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 1B 1C 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B ", "1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1B 1C 1D 1A 0B 0A ", "1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B ", "1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B ", "1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A ", "1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A ", "1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1C 1D 1A 0B 0A ", "1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B ", "1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A ", "1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C 1D 1A 0B 0A 1B 1C 0D ", "1D 1A 0B 0A ", "1D 1A 0B 0A 0B ", "1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 1B 1C 0D 1D 0A 0B 1A 1B 1C ", "1D 1A 0B 0A 1B 1C 0D 1D 0A 1B 1C 1D 1A 0B 0A 1B 1C 0D 1D 0A 0B 1A "
            };

            chain4.Dedupe();

            var foo = chain4.Distinct();

            Assert.That(chain4.Count == foo.Count());
        }

        [Test]
        public void ChainPrint() {
            var chain = new Chain<string>() { "abba", "kababra" };

            var foo = chain.Print();
        }
    }
}
