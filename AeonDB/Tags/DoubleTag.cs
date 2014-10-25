using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Tags
{
    public class DoubleTag : Tag
    {
        internal DoubleTag(string name)
            : base(name, sizeof(double), TagType.Double)
        {

        }
    }
}
