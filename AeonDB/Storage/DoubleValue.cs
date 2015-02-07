using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    public class DoubleValue : StoredValue
    {
        public DoubleValue()
        {
            this.Time = new Timestamp(0);
            this.Value = default(double);
        }

        internal override void ReadValueFromPage(BinaryReader br)
        {
            this.Time = new Timestamp(br.ReadInt64());
            this.Value = br.ReadDouble();
        }

        internal override void SaveValueToPage(BinaryWriter bw)
        {
            bw.Write(this.Time);
            bw.Write((double)this.Value);
        }
    }
}
