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
        private long dataSize;
        private TagType tagType;

        internal Tag(string name, long dataSize, TagType tagType)
        {
            this.name = name;
            this.dataSize = dataSize;
            this.tagType = tagType;
        }

        public string Name
        {
            get { return this.name; }
        }

        internal TagType Type
        {
            get { return this.tagType; }
        }
    }
}
