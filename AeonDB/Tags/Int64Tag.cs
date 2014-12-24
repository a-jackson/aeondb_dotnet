using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Tags
{
    public class Int64Tag : Tag
    {
        internal Int64Tag(string name, AeonDB aeondb)
            : base(name, sizeof(double), TagType.Int64, aeondb)
        {

        }
    }
}
