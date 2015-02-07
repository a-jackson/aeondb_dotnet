using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    internal class DoublePage : Page
    {
        internal DoublePage(Position position)
            : base(position)
        {
        }

        protected override Size ValueSize
        {
            get { return new Size(sizeof(double)); }
        }

        protected override StoredValue GetNewValue()
        {
            return new DoubleValue();
        }
    }
}
