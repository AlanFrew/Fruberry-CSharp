using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Fruberry {
    public interface IStructure<T> : ICollection<T>, ICollection { //IEnumerable and IEnumerable<T> are included indirectly
        new IStructure<T> Add(T item);

        new bool Remove(T item);

        new int Count();

        int Length { get; protected set; }

        new bool Contains(T item);

        new IStructure<T> Clear();

        T Pop();

        T Peek();

        IStructure<T> Enqueue(T item);

        T Dequeue();

        //IStructure<T> Create(params Prefer[] priorities);

        //IStructure<T> Create(IEnumerable<Prefer> priorities);

        IList<Prefer> Constraints { get; }

        static IStructure<T> New(params Prefer[] priorities) {
            priorities = priorities.Where(_ => _ != Prefer.Nothing).ToArray();

            if (priorities.Length == 0 || priorities.One(_ => _ == Prefer.AllowDupes)) return new RedBlackTree<T>();

            var constraints = priorities.Where(_ => _ == Prefer.AllowDupes || _ == Prefer.NoDupes || _ == Prefer.FixedSize || _ == Prefer.NoCompare || _ == Prefer.FixedSize);

            var structures = new Chain<IStructure<T>>(Structures);

            foreach (var constraint in constraints) {
                foreach (var structure in structures) {
                    if (structure.Value.Constraints.Contains(constraint) == false) {
                        structures.Remove(structure);
                    }
                }
            }

            var wishes = priorities.Where(_ => _ == Prefer.Add || _ == Prefer.Remove || _ == Prefer.Find || _ == Prefer.MinMemory || _ == Prefer.MaxMemory).ToList();

            if (wishes[0] == Prefer.Remove && structures.Any(_ => _ is WaitList<T>)) return new WaitList<T>();

            if (structures.Any(_ => _ is RedBlackTree<T>)) return new RedBlackTree<T>(); //Well rounded performance

            return new Chain<T>();
        }

        static Func<T, T, int> Compare;

        static IList<IStructure<T>> Structures { get; } = new List<IStructure<T>> {
            new Chain<T>(),
            new WaitList<T>(),
            new RedBlackTree<T>(),
            new Pool<T>(0, null),
        };
    }
}
