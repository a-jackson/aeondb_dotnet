using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    internal class FloatPage : Page
    {
        internal FloatPage(Position position)
            : base(position)
        {
        }

        protected override Size ValueSize
        {
            get { return new Size(sizeof(float)); }
        }

        protected override StoredValue GetNewValue()
        {
            return new FloatValue();
        }
    }
}
