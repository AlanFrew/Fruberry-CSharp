using NUnit.Framework;
using System;

namespace Fruberry.Tests {
    public class Recycler<T> : Pool<T> {
        public Recycler(int poolSize, Func<T> instantiator, int startingSize = 0) : base(poolSize, instantiator, startingSize) {
        }

        public override T SurfaceItem() {
            if (Reservoir.Count == 0) {
                SinkItem(ActiveItems[0]);
            }

            return base.SurfaceItem();
        }
    }

    public class PoolTests {
        [Test]
        public void Subclass() {
            var pool = new Pool<string>(1, () => TestingUtil.GenerateRandomLetters(3), 1);

            var item1 = pool.SurfaceItem();
            var item2 = pool.SurfaceItem();

            Assert.AreNotEqual(item1, default(string));
            Assert.AreEqual(item2, default(string));

            var recycler = new Recycler<string>(1, () => TestingUtil.GenerateRandomLetters(3), 1);

            item1 = recycler.SurfaceItem();
            item2 = recycler.SurfaceItem();

            Assert.AreEqual(item1, item2);
        }
    }
}
