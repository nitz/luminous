namespace Luminous.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DictionaryList<T> : IList<T>
    {
        private Dictionary<int, T> _keyIsIndex;
        private Dictionary<T, int> _keyIsObject;
        private int _firstIndex;
        private int _lastIndex;
        private int _count;

        public DictionaryList()
        {
            _keyIsIndex = new Dictionary<int, T>();
            _keyIsObject = new Dictionary<T, int>();
        }

        public int IndexOf(T item)
        {
            if (!_keyIsObject.ContainsKey(item)) return -1;
            return _keyIsObject[item] - _firstIndex;
        }

        public void Insert(int index, T item)
        {
            // pierwszy
            if (_count == 0)
            {
                _keyIsIndex[0] = item;
                _keyIsObject[item] = 0;
            }
            // na początek
            else if (index == 0)
            {
                _firstIndex--;
                _keyIsIndex[_firstIndex] = item;
                _keyIsObject[item] = _firstIndex;
            }
            // na koniec
            else if (index == _count)
            {
                _lastIndex++;
                _keyIsIndex[_lastIndex] = item;
                _keyIsObject[item] = _lastIndex;
            }
            // do środka
            else
            {
                // przenumerowujemy początek
                if (index < _count / 2)
                {
                    for (int i = _firstIndex; i <= _firstIndex + index; i++)
                    {
                        _keyIsIndex[i - 1] = _keyIsIndex[i];
                        _keyIsObject[_keyIsIndex[i - 1]] = i - 1;
                    }
                    _firstIndex--;
                }
                // przenumerowujemy koniec
                else
                {
                    for (int i = _lastIndex; i >= _firstIndex + index; i--)
                    {
                        _keyIsIndex[i + 1] = _keyIsIndex[i];
                        _keyIsObject[_keyIsIndex[i + 1]] = i + 1;
                    }
                    _lastIndex++;
                }
                _keyIsIndex[_firstIndex + index] = item;
                _keyIsObject[item] = _firstIndex + index;
            }
            _count++;
        }

        public void RemoveAt(int index)
        {
            // ostatni
            if (_count == 1)
            {
                _keyIsIndex.Clear();
                _keyIsObject.Clear();
                _firstIndex = _lastIndex = 0;
            }
            // z początku
            if (index == 0)
            {
                _keyIsObject.Remove(_keyIsIndex[_firstIndex]);
                _keyIsIndex.Remove(_firstIndex);
                _firstIndex++;
            }
            // z końca
            else if (index == _count)
            {
                _keyIsObject.Remove(_keyIsIndex[_lastIndex]);
                _keyIsIndex.Remove(_lastIndex);
                _lastIndex--;
            }
            // ze środka
            else
            {
                _keyIsObject.Remove(_keyIsIndex[_firstIndex + index]);
                _keyIsIndex.Remove(_firstIndex + index);
                // przenumerowujemy początek
                if (index < _count / 2)
                {
                    for (int i = _firstIndex + index; i > _firstIndex; i--)
                    {
                        _keyIsIndex[i] = _keyIsIndex[i - 1];
                        _keyIsObject[_keyIsIndex[i]] = i;
                    }
                    _keyIsIndex.Remove(_firstIndex);
                    _firstIndex++;
                }
                // przenumerowujemy koniec
                else
                {
                    for (int i = _firstIndex + index; i < _lastIndex; i++)
                    {
                        _keyIsIndex[i] = _keyIsIndex[i + 1];
                        _keyIsObject[_keyIsIndex[i]] = i;
                    }
                    _keyIsIndex.Remove(_lastIndex);
                    _lastIndex--;
                }
            }
            _count--;
        }

        public T this[int index]
        {
            get
            {
                return _keyIsIndex[_firstIndex + index];
            }
            set
            {
                T oldValue = _keyIsIndex[_firstIndex + index];
                _keyIsIndex[_firstIndex + index] = value;
                _keyIsObject.Remove(oldValue);
                _keyIsObject.Add(value, _firstIndex + index);
            }
        }

        public void Add(T item)
        {
            Insert(_count, item);
        }

        public void Clear()
        {
            _firstIndex = _lastIndex = _count = 0;
            _keyIsIndex.Clear();
            _keyIsObject.Clear();
        }

        public bool Contains(T item)
        {
            return _keyIsObject.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _keyIsIndex.OrderBy(x => x.Key).Select(x => x.Value).ToList().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _keyIsIndex[_firstIndex + i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
