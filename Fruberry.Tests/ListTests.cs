using NUnit.Framework;
using System;
using List = Fruberry.List<string>;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Fruberry.Tests {
    public class ListTester<T> : List<T> {
        public new bool Validate() {
            return base.Validate();
        }
    }

    public class ListTests {
        [Test]
        public void Constructor() {
            var list = new List();

            Assert.True(list.Capacity == 0);
            Assert.True(list.Length == 0);

            list = new List(0);

            Assert.True(list.Capacity == 0);
            Assert.True(list.Length == 0);

            list = new List(3);

            Assert.True(list.Capacity == 3);
            Assert.True(list.Length == 0);
        }

        [Test]
        public void Enumerate() {
            var tree = new List<string> { "foo", "bar", "baz" };

            var joined = new StringBuilder();

            foreach (var item in tree) {
                joined.Append(item);
            }

            Assert.AreEqual("foobarbaz", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (IEnumerable)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("foobarbaz", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (IEnumerable<string>)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("foobarbaz", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (ICollection)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("foobarbaz", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (ICollection<string>)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("foobarbaz", joined.ToString());

            joined = new StringBuilder();

            foreach (var item in (IStructure<string>)tree) {
                joined.Append(item);
            }

            Assert.AreEqual("foobarbaz", joined.ToString());
        }

        [Test]
        public void Capacity() {
            Assert.AreEqual(ExceptionBehavior.BestEffort, List.ExceptionBehavior);
            var list = new List(5);
            Assert.AreEqual(5, list.Capacity);

            list.Capacity = 6;
            Assert.AreEqual(6, list.Capacity);

            list.Capacity = 0;
            Assert.AreEqual(0, list.Capacity);

            list.Capacity = -1;
            Assert.AreEqual(0, list.Capacity);

            list.AddRange(new[] { "one", "two", "three", "four" });
            Assert.AreEqual(4, list.Capacity);

            List.ExceptionBehavior = ExceptionBehavior.Throw;
            Assert.AreEqual(ExceptionBehavior.Throw, List.ExceptionBehavior);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.Capacity = -1);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.Capacity = 3);

            list.Clear();
            list.Capacity = 0;
        }

        [Test]
        public void Regression() {
            var list = new System.Collections.Generic.List<string>(5);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void Index() {
            var list = new List { "zero", "one", "two", "three", "four" };
            List.ExceptionBehavior = ExceptionBehavior.BestEffort;

            Assert.AreEqual("zero", list[0]);
            Assert.AreEqual("four", list[4]);
            Assert.AreEqual("four", list[5]);
            Assert.AreEqual("zero", list[-1]);

            List.ExceptionBehavior = ExceptionBehavior.Abort;

            Assert.AreEqual(null, list[5]);
            Assert.AreEqual(null, list[-1]);

            List.ExceptionBehavior = ExceptionBehavior.Throw;

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { var test = list[-1]; } );
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { var test = list[5]; });
        }

        [Test]
        public void AddRemove_Random() {
            var numOperations = 1000;

            var random = new Random((int)DateTime.Now.Ticks);

            var list = new ListTester<string>();
            var backup = new List<string>();
            for (var counter = 0; counter < numOperations; ++counter) {
                var opType = random.Next(0, 2);

                if (opType == 0) {
                    var item = TestingUtil.GenerateRandomLetters(3);

                    list.Enqueue(item);
                    backup.Add(item);

                    Assert.AreEqual(backup.Count, list.Count);

                    foreach (var backupItem in backup) {
                        Assert.True(list.Contains(backupItem));
                    }

                    Console.WriteLine("Add");
                    Assert.True(list.Validate(), "Fail on Enqueue, operation " + counter);
                }
                else if (list.Count > 0) {
                    var count = list.Count(item => item.Equals(list.Peek()));

                    var item = list.Dequeue();

                    //Assert.False(list.Contains(item), "Fail on Dequeue, operation " + counter);
                    Assert.True(list.Count(_ => _.Equals(item)) == count - 1, "Fail on Dequeue, operation " + counter);
                    Assert.True(backup.Remove(item));
                    Assert.AreEqual(backup.Count, list.Count);

                    foreach (var backupItem in backup) {
                        Assert.True(list.Contains(backupItem), "Fail on Dequeue, operation " + counter);
                    }

                    Console.WriteLine("Remove");
                    Assert.True(list.Validate(), "Fail on Dequeue, operation " + counter);
                }
            }
        }

        [Test]
        public void CopyTo() {
            List.ExceptionBehavior = ExceptionBehavior.BestEffort;

            var list = new List { "zero", "one", "two", "three", "four" };

            var array = new string[5];

            list.CopyTo(array, 0);

            Assert.AreEqual("zero", array[0]);
            Assert.AreEqual("four", array[4]);

            array = new string[3];

            list.CopyTo(array, -1);

            Assert.AreEqual("zero", array[0]);
            Assert.AreEqual("two", array[2]);

            array = new string[0];

            list.CopyTo(array, 0);

            List.ExceptionBehavior = ExceptionBehavior.Throw;

            array = new string[3];

            Assert.Throws(typeof(InvalidOperationException), () => list.CopyTo(array, 0));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => list.CopyTo(array, -1));
        }
    }
}
