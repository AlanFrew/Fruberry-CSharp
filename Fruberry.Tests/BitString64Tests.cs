using NUnit.Framework;

namespace Fruberry.Tests {
    public class BitString64Tests {
        [Test]
        public void BitString64Example() {
            var bitString = new BitString64("10000");

            Assert.True(bitString.ToString() == "10000");
            Assert.True(bitString[0] == 1);
            Assert.True(bitString[1] == 0);

            bitString[4] = 1;
            bitString[5] = 0;

            Assert.True(bitString.ToString() == "100010");

            bitString <<= 0;
            bitString <<= 1;
            bitString <<= '1';
            bitString <<= '0';

            Assert.True(bitString.ToString() == "1000100110");

            bitString >>= 1;

            Assert.True(bitString.ToString() == "11000100110");
        }
    }
}
