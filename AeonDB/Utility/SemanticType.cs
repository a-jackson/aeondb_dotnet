using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Utility
{
    public abstract class SemanticType<TData> : IEquatable<SemanticType<TData>>, IEqualityComparer<SemanticType<TData>>, IComparable, IComparable<SemanticType<TData>>
        where TData : IComparable<TData>
    {
        private TData value;

        protected SemanticType(Func<TData, bool> isValid, TData value)
        {
            if (isValid != null && !isValid(value))
            {
                throw new ArgumentException(string.Format("Trying to set a {0} to {1} which is invalid", this.GetType(), value));
            }

            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Timestamp);
        }

        public virtual TData Value
        {
            get { return value; }
            private set { this.value = value; }
        }

        public override int GetHashCode()
        {
            return this.GetHashCode(this);
        }

        public bool Equals(SemanticType<TData> other)
        {
            return this.Value.Equals(other.Value);
        }

        public bool Equals(SemanticType<TData> x, SemanticType<TData> y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(SemanticType<TData> obj)
        {
            return obj.Value.GetHashCode();
        }

        public static bool operator ==(SemanticType<TData> a, SemanticType<TData> b)
        {
            if (!object.ReferenceEquals(a, null) && !object.ReferenceEquals(b, null))
            {
                return a.Value.Equals(b.Value);
            }
            else
            {
                return object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null);
            }
        }

        public static bool operator !=(SemanticType<TData> a, SemanticType<TData> b)
        {
            return !(a == b);
        }

        public static implicit operator TData(SemanticType<TData> a)
        {
            return a.Value;
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as SemanticType<TData>);
        }

        public int CompareTo(SemanticType<TData> other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public static bool operator <(SemanticType<TData> a, SemanticType<TData> b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(SemanticType<TData> a, SemanticType<TData> b)
        {
            return a.CompareTo(b) > 0;
        }
    }
}
