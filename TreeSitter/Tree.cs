using System.Collections;

namespace TED.TreeSitter;

using static Native;

public class Tree : IDisposable {
    internal IntPtr Handle;

    internal Tree(IntPtr handle) {
        Handle = handle;
    }

    public Node Root => Node.Create(ts_tree_root_node(Handle));

    public void Dispose() {
        ts_tree_delete(Handle);
    }

    public Tree Copy() {
        return new Tree(ts_tree_copy(Handle));
    }
}

public class CacheArray<T> : IReadOnlyList<T> {
    private readonly Func<int, T> _generator;

    public CacheArray(int count, Func<int, T> generator) {
        Count = count;
        _generator = generator;
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator() {
        return new CacheArrayEnumerator<T>(this);
    }

    public int Count { get; }

    public T this[int index] => _generator(index);

    public class CacheArrayEnumerator<T1> : IEnumerator<T> {
        private readonly CacheArray<T> _cacheArray;
        private int index = -1;

        public CacheArrayEnumerator(CacheArray<T> cacheArray) {
            _cacheArray = cacheArray;
        }

        object IEnumerator.Current => Current;

        public bool MoveNext() {
            index++;
            return index < _cacheArray.Count;
        }

        public void Reset() {
            index = -1;
        }

        public T Current => _cacheArray[index];

        public void Dispose() { }
    }
}