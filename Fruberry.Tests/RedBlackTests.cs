using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Fruberry.Tests {
    public class RedBlackTester<T> : RedBlackTree<T> {
        public new bool Validate() {
            return base.Validate();
        }

        public new Node<T> Nil => base.Nil;
    }

    public class RedBlackTests{
        [Test]
        public void Enumerate() {
            var tree = new RedBlackTree<string> { "foo", "bar", "baz" };

            var joined = new StringBuilder();

            foreach (var item in tree) {
                joined.Append(item);
            }

            Assert.AreEqual("barbazfoo", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (IEnumerable)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("barbazfoo", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (IEnumerable<string>)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("barbazfoo", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (ICollection)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("barbazfoo", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (ICollection<string>)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("barbazfoo", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (IStructure<string>)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("barbazfoo", joined.ToString());
        }

        [Test]
        public void Validate_Random() {
            var numOperations = 1000;

            var random = new Random((int)DateTime.Now.Ticks);

            var redBlackTree = new RedBlackTester<string>();
            var backup = new List<string>();
            for (var counter = 0; counter < numOperations; ++counter) {
                var opType = random.Next(0, 2);

                if (opType == 0) {
                    var item = TestingUtil.GenerateRandomLetters(3);
                    redBlackTree.Enqueue(item);
                    backup.Add(item);

                    Assert.AreEqual(backup.Count, redBlackTree.Count);

                    foreach(var backupItem in backup) {
                        Assert.True(redBlackTree.Contains(backupItem));
                    }

                    Console.WriteLine("Add");
                    Assert.True(redBlackTree.Validate(), "Fail on Enqueue, operation " + counter);
                }
                else if (redBlackTree.Count > 0) {
                    var item = redBlackTree.Dequeue();

                    Assert.False(redBlackTree.Contains(item), "Fail on Dequeue, operation " + counter);
                    Assert.True(backup.Remove(item));
                    Assert.AreEqual(backup.Count, redBlackTree.Count);

                    foreach (var backupItem in backup) {
                        Assert.True(redBlackTree.Contains(backupItem), "Fail on Dequeue, operation " + counter);
                    }

                    Console.WriteLine("Remove");
                    Assert.True(redBlackTree.Validate(), "Fail on Dequeue, operation " + counter);
                }
            }
        }

        [Test]
        public void Validate_Specific() {
            var redBlackTree = new RedBlackTester<string>();

            redBlackTree.Add("a");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Add("b");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Add("c");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Add("d");
            Assert.True(redBlackTree.Validate());

            var backup = new List<string> { "a", "b", "c", "d" };
            foreach (var backupItem in backup) {
                Assert.True(redBlackTree.Contains(backupItem));
            }

            var item = redBlackTree.Pop();
            Assert.True(backup.Remove(item));
            Assert.True(redBlackTree.Validate());
            Assert.False(redBlackTree.Contains(item));

            item = redBlackTree.Dequeue();
            Assert.False(redBlackTree.Contains(item));
            Assert.True(backup.Remove(item));
            Assert.True(redBlackTree.Validate());

            foreach (var backupItem in backup) {
                Assert.True(redBlackTree.Contains(backupItem));
            }
        }

        [Test]
        public void Validate_Specific2() {
            var redBlackTree = new RedBlackTester<string>();

            redBlackTree.Enqueue("c");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue("b");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue("a");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());
        }

        [Test]
        public void Validate_Specific3() {
            var redBlackTree = new RedBlackTester<string>();

            redBlackTree.Enqueue("a");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue("d");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue("c");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue("b");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue("e");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());
        }

        [Test]
        public void Validate_Specific4() {
            var redBlackTree = new RedBlackTester<string>();

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());
        }

        [Test]
        public void Validate_Specific5() {
            var redBlackTree = new RedBlackTester<string>();

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());
        }

        [Test]
        public void Validate_Specific6() {
            var redBlackTree = new RedBlackTester<string>();

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Enqueue(TestingUtil.GenerateRandomLetters(2));
            Assert.True(redBlackTree.Validate());

            redBlackTree.Dequeue();
            Assert.True(redBlackTree.Validate());
        }

        [Test]
        public void Validate_Specific7() {
            var redBlackTree = new RedBlackTester<string>();
            var backup = new List<string>();

            redBlackTree.Add("a");
            backup.Add("a");
            Assert.True(redBlackTree.Validate());

            var item = redBlackTree.Pop();
            Assert.True(backup.Remove(item));
            Assert.True(redBlackTree.Validate());
            Assert.False(redBlackTree.Contains(item));

            redBlackTree.Add("b");
            backup.Add("b");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Add("c");
            backup.Add("c");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Add("d");
            backup.Add("d");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Add("e");
            backup.Add("e");
            Assert.True(redBlackTree.Validate());

            redBlackTree.Add("f");
            backup.Add("f");
            Assert.True(redBlackTree.Validate());

            item = redBlackTree.Pop();
            Assert.True(backup.Remove(item));
            Assert.True(redBlackTree.Validate());
            Assert.False(redBlackTree.Contains(item));

            foreach (var backupItem in backup) {
                Assert.True(redBlackTree.Contains(backupItem));
            }
        }

        [Test]
        public void Validate_Specific8() {
            var redBlackTree = new RedBlackTester<string>();
            var backup = new List<string>();

            Add(redBlackTree, backup, "a");
            //RemoveHelper(redBlackTree, backup);
            Add(redBlackTree, backup, "b");
            Add(redBlackTree, backup, "a");
            Add(redBlackTree, backup, "d");
            Add(redBlackTree, backup, "d");
            DupeRemoveHelper(redBlackTree, backup);
            Add(redBlackTree, backup, "f");
            Add(redBlackTree, backup, "g");
            DupeRemoveHelper(redBlackTree, backup);
            Add(redBlackTree, backup, "h");
            DupeRemoveHelper(redBlackTree, backup);
            DupeRemoveHelper(redBlackTree, backup);
            DupeRemoveHelper(redBlackTree, backup);
            Add(redBlackTree, backup, "i");
            
            foreach (var backupItem in backup) {
                Assert.True(redBlackTree.Contains(backupItem));
            }
        }

        private static string RemoveHelper(RedBlackTester<string> redBlackTree, List<string> backup) {
            var item = redBlackTree.Pop();
            Assert.True(backup.Remove(item));
            Assert.True(redBlackTree.Validate());
            Assert.False(redBlackTree.Contains(item));
            Assert.AreEqual(default(string), redBlackTree.Nil.Value);
            return item;
        }

        private static string DupeRemoveHelper(RedBlackTester<string> redBlackTree, List<string> backup) {
            var item = redBlackTree.Pop();
            Assert.True(backup.Remove(item));
            Assert.True(redBlackTree.Validate());
            Assert.AreEqual(default(string), redBlackTree.Nil.Value);
            return item;
        }

        public void Add<T>(RedBlackTester<T> structure, List<T> backup, T value) {
            structure.Add(value);
            backup.Add(value);
            Assert.True(structure.Validate());
        }
    }
}
