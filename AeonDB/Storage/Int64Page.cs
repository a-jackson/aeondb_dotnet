using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    internal class Int64Page : Page
    {
        internal Int64Page(Position position)
            : base(position)
        {
        }

        protected override Size ValueSize
        {
            get { return new Size(sizeof(long)); }
        }

        protected override StoredValue GetNewValue()
        {
            return new Int64Value();
        }
    }
}
