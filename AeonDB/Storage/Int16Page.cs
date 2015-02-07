using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    internal class Int16Page : Page
    {
        internal Int16Page(Position position)
            : base(position)
        {
        }

        protected override Size ValueSize
        {
            get { return new Size(sizeof(short)); }
        }

        protected override StoredValue GetNewValue()
        {
            return new Int16Value();
        }
    }
}
