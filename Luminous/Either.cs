#region License
// Copyright © 2014 Łukasz Świątkowski
// http://www.lukesw.net/
//
// This library is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library.  If not, see <http://www.gnu.org/licenses/>.
#endregion

namespace System
{
    [Serializable]
    public struct Either<T1, T2>
    {
        #region Fields

        private T1 _item1;
        private T2 _item2;
        private bool _is2;

        #endregion

        #region Constructors

        public Either(T1 item1)
        {
            _item1 = item1;
            _item2 = default(T2);
            _is2 = false;
        }

        public Either(T2 item2)
        {
            _item1 = default(T1);
            _item2 = item2;
            _is2 = true;
        }

        private Either(T1 item1, T2 item2, bool is2)
        {
            _item1 = item1;
            _item2 = item2;
            _is2 = is2;
        }

        public static Either<T1, T2> MakeFirst(T1 aValue)
        {
            return new Either<T1, T2>(aValue, default(T2), false);
        }

        public static Either<T1, T2> MakeSecond(T2 bValue)
        {
            return new Either<T1, T2>(default(T1), bValue, true);
        }

        #endregion

        #region Properties

        public bool IsFirst
        {
            get { return !_is2; }
        }

        public T1 First
        {
            get
            {
                if (_is2) throw new InvalidOperationException();
                return _item1;
            }
        }

        public bool IsSecond
        {
            get { return _is2; }
        }

        public T2 Second
        {
            get
            {
                if (!_is2) throw new InvalidOperationException();
                return _item2;
            }
        }

        #endregion

        #region Overrided Methods

        public override bool Equals(object other)
        {
            return other is Either<T1, T2> && (this == (Either<T1, T2>)other);
        }

        public override int GetHashCode()
        {
            return _is2 ? _item2.GetHashCode() : _item1.GetHashCode();
        }

        public override string ToString()
        {
            return _is2 ? _item2.ToString() : _item1.ToString();
        }

        #endregion

        #region Operators

        public static bool operator ==(Either<T1, T2> left, Either<T1, T2> right)
        {
            return left._is2 == right._is2
                   ? (left._is2
                      ? left._item2.Equals(right._item2)
                      : left._item1.Equals(right._item1))
                   : false;
        }

        public static bool operator !=(Either<T1, T2> left, Either<T1, T2> right)
        {
            return !(left == right);
        }

        public static implicit operator Either<T1, T2>(T1 item) { return MakeFirst(item); }
        public static implicit operator Either<T1, T2>(T2 item) { return MakeSecond(item); }
        public static explicit operator T1(Either<T1, T2> either) { return either.First; }
        public static explicit operator T2(Either<T1, T2> either) { return either.Second; }

        #endregion
    }
}
