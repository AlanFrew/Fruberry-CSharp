using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fruberry {
    public abstract class Structure<T> {
        public static IStructure<T> Create(params Prefer[] priorities) {
            var constraints = priorities.Where(_ => _ == Prefer.AllowDupes || _ == Prefer.NoDupes || _ == Prefer.FixedSize || _ == Prefer.NoCompare);

            if (priorities.Length == 0 || priorities[0] == Prefer.Nothing) return new Chain<T>();

            //if (priorities[0] == Prefer.Remove && typeof(T) is IEnumerable<char>) return new WaitList<T>();
            return new Chain<T>();
        }
    }
}
