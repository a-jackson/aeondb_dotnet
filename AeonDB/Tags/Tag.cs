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
        private AeonDB aeondb;

        internal Tag(string name, long dataSize, TagType tagType, AeonDB aeondb)
        {
            this.name = name;
            this.dataSize = dataSize;
            this.tagType = tagType;
            this.aeondb = aeondb;
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
