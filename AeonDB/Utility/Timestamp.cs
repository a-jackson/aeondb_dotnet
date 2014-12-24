using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Utility
{
    internal class Timestamp : IEquatable<Timestamp>, IEqualityComparer<Timestamp>, IComparable, IComparable<Timestamp>
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0);
        private DateTime dateTime;
        private long timestamp;

        public Timestamp(DateTime dateTime)
        {
            this.dateTime = dateTime;
            this.timestamp = (long)(dateTime - Epoch).TotalSeconds;
        }

        public Timestamp(long timeStamp)
        {
            this.timestamp = timeStamp;
            this.dateTime = (Epoch + new TimeSpan(this.timestamp * 10000));
        }

        public DateTime DateTime
        {
            get { return this.dateTime; }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Timestamp);
        }

        public override int GetHashCode()
        {
            return this.GetHashCode(this);
        }

        public bool Equals(Timestamp other)
        {
            return this.dateTime == other.dateTime;
        }

        public bool Equals(Timestamp x, Timestamp y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Timestamp obj)
        {
            return obj.dateTime.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as Timestamp);
        }

        public int CompareTo(Timestamp other)
        {
            return this.dateTime.CompareTo(other.dateTime);
        }

        public static bool operator <(Timestamp a, Timestamp b)
        {
            return a.dateTime < b.dateTime;
        }

        public static bool operator >(Timestamp a, Timestamp b)
        {
            return a.dateTime > b.dateTime;
        }

        public static bool operator ==(Timestamp a, Timestamp b)
        {
            return a.dateTime == b.dateTime;
        }

        public static bool operator !=(Timestamp a, Timestamp b)
        {
            return a.dateTime != b.dateTime;
        }

        public static implicit operator long(Timestamp a)
        {
            return a.timestamp;
        }
    }
}
