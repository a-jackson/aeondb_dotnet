using AeonDB.Storage;
using AeonDB.Tags;
using AeonDB.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB
{
    public class Query : IEnumerable<StoredValue>
    {
        private readonly AeonDB aeonDB;

        private Tag tag;
        private Timestamp startTime;
        private Timestamp endTime;

        public Query(AeonDB aeonDB)
        {
            this.aeonDB = aeonDB;
            this.tag = null;
            this.startTime = null;
            this.endTime = null;
        }

        public void SetTag(string tagname)
        {
            this.tag = this.aeonDB.GetTag(tagname);
        }

        public void SetTag(Tag tag)
        {
            this.tag = tag;
        }

        public void SetStartTime(DateTime startTime)
        {
            this.startTime = new Timestamp(startTime);
        }

        public void SetStartTime(Timestamp startTime)
        {
            this.startTime = startTime;
        }

        public void SetDuration(TimeSpan duration)
        {
            if (this.startTime == null)
            {
                throw new AeonException("Cannot set duration until start time is set.");
            }

            this.endTime = new Timestamp(this.startTime.DateTime.Add(duration));
        }

        public void SetDuration(int seconds)
        {
            if (this.startTime == null)
            {
                throw new AeonException("Cannot set duration until start time is set.");
            }

            this.endTime = new Timestamp(this.startTime.DateTime.AddSeconds(seconds));
        }

        public void SetEndTime(DateTime endTime)
        {
            this.endTime = new Timestamp(endTime);
        }

        public void SetEndTime(Timestamp endTime)
        {
            this.endTime = endTime;
        }

        public IEnumerator<StoredValue> GetEnumerator()
        {
            if (this.tag == null || this.startTime == null || this.endTime == null)
            {
                throw new AeonException("Cannot enumerate query until tag, start time and end time or duration are configured.");
            }

            if (this.startTime >= this.endTime)
            {
                throw new AeonException("Start time should be before end time.");
            }

            var values = this.tag.GetValues(this.startTime);
            foreach (var value in values)
            {
                if (value.Time <= this.endTime)
                {
                    yield return value;
                }
                else
                {
                    break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
