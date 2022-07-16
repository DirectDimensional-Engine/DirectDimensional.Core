using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectDimensional.Core.Miscs {
    public interface IPoolingCollection<T> : IEnumerable<T> where T : class {
        T Get();
        void Collect(T obj);
        int Count { get; }
        void Clear();
    }

    public sealed class PoolingList<T> : IPoolingCollection<T> where T : class {
        private readonly List<T> _items;
        private readonly Func<T, bool> _condition;
        private readonly Func<T> _create;
        private readonly Action<T>? _onReturn;

        public int Count => _items.Count;

        public PoolingList(Func<T, bool> condition, Func<T> create) : this(8, condition, create, null) { }
        public PoolingList(Func<T, bool> condition, Func<T> create, Action<T>? onReturn) : this(8, condition, create, onReturn) { }
        public PoolingList(int capacity, Func<T, bool> condition, Func<T> create, Action<T>? onReturn) {
            _items = new(capacity);
            _condition = condition;
            _create = create;
            _onReturn = onReturn;
        }

        public T Get() {
            for (int i = 0; i < _items.Count; i++) {
                var item = _items[i];

                if (_condition(item)) {
                    _items.Remove(item);

                    _onReturn?.Invoke(item);
                    return item;
                }
            }

            return _create();
        }

        public void Collect(T obj) {
            _items.Add(obj);
        }

        public void Clear() {
            _items.Clear();
        }

        public IEnumerator<T> GetEnumerator() {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
    public sealed class PoolingStack<T> : IPoolingCollection<T> where T : class {
        private readonly Stack<T> _items;
        private readonly Func<T, bool> _condition;
        private readonly Func<T> _create;
        private readonly Action<T>? _onReturn;

        public int Count => _items.Count;

        public PoolingStack(Func<T, bool> condition, Func<T> create) : this(8, condition, create, null) { }
        public PoolingStack(Func<T, bool> condition, Func<T> create, Action<T>? onReturn) : this(8, condition, create, onReturn) { }

        public PoolingStack(int capacity, Func<T, bool> condition, Func<T> create, Action<T>? onReturn) {
            _items = new(capacity);
            _condition = condition;
            _create = create;
            _onReturn = onReturn;
        }

        public T Get() {
            if (_items.TryPeek(out var obj) && _condition(obj)) {
                _onReturn?.Invoke(obj);
                _items.Pop();

                return obj;
            }

            return _create();
        }

        public void Collect(T obj) {
            _items.Push(obj);
        }

        public void Clear() {
            _items.Clear();
        }

        public IEnumerator<T> GetEnumerator() {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
    public sealed class PoolingQueue<T> : IPoolingCollection<T> where T : class {
        private readonly Queue<T> _items;
        private readonly Func<T, bool> _condition;
        private readonly Func<T> _create;
        private readonly Action<T>? _onReturn;

        public int Count => _items.Count;

        public PoolingQueue(Func<T, bool> condition, Func<T> create) : this(8, condition, create, null) { }
        public PoolingQueue(Func<T, bool> condition, Func<T> create, Action<T>? onReturn) : this(8, condition, create, onReturn) { }
        public PoolingQueue(int capacity, Func<T, bool> condition, Func<T> create, Action<T>? onReturn) {
            _items = new(capacity);
            _condition = condition;
            _create = create;
            _onReturn = onReturn;
        }

        public T Get() {
            if (_items.TryPeek(out var obj) && _condition(obj)) {
                _onReturn?.Invoke(obj);
                _items.Dequeue();

                return obj;
            }

            return _create();
        }

        public void Collect(T obj) {
            _items.Enqueue(obj);
        }

        public void Clear() {
            _items.Clear();
        }

        public IEnumerator<T> GetEnumerator() {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
