using System;
using System.Text;
using NUnit.Framework;

namespace Fruberry.Tests {
    public class WaitListTests {
        [Test]
        public void Validate_Random() {
            var numOperations = 1000;

            var random = new Random((int)DateTime.Now.Ticks);

            var waitList = new WaitList<string>();
            for (var counter = 0; counter < numOperations; ++counter) {
                var opType = random.Next(0, 2);

                if (opType == 0) {
                    byte[] lastName = new byte[2];
                    random.NextBytes(lastName);

                    waitList.Enqueue(Encoding.Default.GetString(lastName));

                    if (waitList.IsConsistent() == false) {
                        Console.WriteLine("Test fails after enqueue operation # " + counter);
                    }
                }
                else {
                    if (waitList.Count > 0) {
                        waitList.Dequeue();

                        if (waitList.IsConsistent() == false) {
                            Console.WriteLine("Test fails after dequeue operation # " + counter);
                        }
                    }
                }
            }
            Console.WriteLine("\nAll tests passed");
        }

        [Test]
        public void Enumerate() {
            var waitlist = new WaitList<string> { "foo", "bar", "baz" };

            var joined = new StringBuilder();

            foreach(var item in waitlist) {
                joined.Append(item); 
            }

            Assert.AreEqual("barfoobaz", joined.ToString());
        }
    }
}
