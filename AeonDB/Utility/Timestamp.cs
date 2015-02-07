using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Utility
{
    public class Timestamp : SemanticType<long>
    {
        /// <summary>
        /// The size of a timestamp.
        /// </summary>
        public const int TypeSize = sizeof(long);

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0);
        private DateTime dateTime;

        public Timestamp(DateTime dateTime) : base(null, (long)(dateTime - Epoch).TotalSeconds)
        {
            this.dateTime = dateTime;
        }

        public Timestamp(long timeStamp) : base (null, timeStamp)
        {
            this.dateTime = (Epoch + new TimeSpan(this.Value * 10000000));
        }

        public DateTime DateTime
        {
            get { return this.dateTime; }
        }
    }
}
