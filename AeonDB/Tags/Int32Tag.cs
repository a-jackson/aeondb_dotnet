using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Tags
{
    public class Int32Tag : Tag
    {
        internal Int32Tag(string name, AeonDB aeondb)
            : base(name, sizeof(double), TagType.Int32, aeondb)
        {

        }
    }
}
