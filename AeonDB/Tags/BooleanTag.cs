using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Tags
{
    public class BooleanTag : Tag
    {
        internal BooleanTag(string name, AeonDB aeonDB)
            : base(name, sizeof(double), TagType.Boolean, aeonDB)
        {

        }
    }
}
