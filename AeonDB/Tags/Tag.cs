using AeonDB.Storage;
using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Tags
{
    public abstract class Tag
    {
        private string name;
        private Size dataSize;
        private TagType tagType;
        private AeonDB aeondb;
        private Timestore timestore;

        internal Tag(string name, int dataSize, TagType tagType, AeonDB aeondb)
        {
            this.name = name;
            this.dataSize = new Size(dataSize);
            this.tagType = tagType;
            this.aeondb = aeondb;
            this.timestore = new Timestore(this, this.aeondb.Directory + this.name);
        }

        public string Name
        {
            get { return this.name; }
        }

        internal TagType Type
        {
            get { return this.tagType; }
        }

        internal Size DataSize
        {
            get { return this.dataSize; }
        }

        public void AddValue(Timestamp timestamp, object value)
        {
            this.timestore.AddValue(timestamp, value);
        }
    }
}
