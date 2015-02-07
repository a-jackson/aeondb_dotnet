using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Utility
{
    internal class Size : SemanticType<int>
    {
        public const int TypeSize = sizeof(int);

        public Size(int size)
            : base(value => value > 0, size)
        {

        }
    }
}
