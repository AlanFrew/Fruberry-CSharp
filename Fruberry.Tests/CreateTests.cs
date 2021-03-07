using NUnit.Framework;

namespace Fruberry.Tests {
    public class CreateTests {
        [Test]
        public void Create() {
            //given an ordered list of constraints and metics to optimize, return the optimal data structure
            Assert.True(IStructure<int>.New(new[] { Prefer.Nothing }) is RedBlackTree<int>);
            Assert.True(IStructure<int>.New(new Prefer[] { }) is RedBlackTree<int>);

            Assert.True(IStructure<int>.New(Prefer.Remove) is WaitList<int>);
            Assert.True(IStructure<int>.New(Prefer.Remove, Prefer.NoCompare) is Chain<int>);
        }
    }
}
