using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Tags
{
    public class Int16Tag : Tag
    {
        internal Int16Tag(string name)
            : base(name, sizeof(double), TagType.Int16)
        {

        }
    }
}
