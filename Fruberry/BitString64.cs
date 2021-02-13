using System;

namespace Fruberry {
    public struct BitString64 {
        public byte Length;

        private long _data;

        public BitString64(int length) {
            Length = (byte)length;
            _data = 0;
        }

        public BitString64(string newValue) {
            _data = 0;
            for (var i = 0; i < newValue.Length; i++) {
                _data <<= 1;
                _data += newValue[i] - '0';
            }

            Length = (byte)newValue.Length;
        }

        public BitString64(BitString64 other) {
            _data = other._data;
            Length = other.Length;
        }

        public int this[int index] {
            get => (_data >> (Length - index - 1)) % 2 == 0 ? 0 : 1;
            set {
                if (index >= Length) {
                    _data <<= (index - Length + 1);
                    index = Length;
                }

                var mask = 1U << (Length - index - 1);
                if (value == 0 || value == '0') {
                    mask = ~mask;
                    _data &= mask;
                }
                else {
                    _data |= mask;
                }

                if (Length < index + 1) Length = (byte)(index + 1);
            }
        }

        public override string ToString() {
            var result = new char[Length];

            var temp = _data;
            for (var i = Length - 1; i >= 0; i--) {
                result[i] = (temp % 2 == 0 ? '0' : '1');
                temp >>= 1;
            }

            return new string(result);
        }

        public override bool Equals(object obj) {
            if (!(obj is BitString64)) return false;

            var other = (BitString64)obj;
            return _data == other._data && Length == other.Length;
        }

        public override int GetHashCode() {
            return _data.GetHashCode();
        }

        public static BitString64 operator <<(BitString64 self, int addToBack) {
            self._data <<= 1;

            if (addToBack != 0 && addToBack != '0') self._data += 1;

            self.Length++;

            return self;
        }

        public static BitString64 operator >>(BitString64 self, int addToFront) {
            self.Length++;

            self[0] = addToFront;

            return self;
        }

        public static bool operator ==(BitString64 self, string other) {
            if (self.Length == 0) return other == null || other.Length == 0;

            return self.ToString() == other;
        }

        public static bool operator !=(BitString64 self, string other) {
            return !(self == other);
        }
    }
}
