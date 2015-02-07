using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    public class Int32Value : StoredValue
    {
        public Int32Value()
        {
            this.Time = new Timestamp(0);
            this.Value = default(int);
        }

        internal override void ReadValueFromPage(BinaryReader br)
        {
            this.Time = new Timestamp(br.ReadInt64());
            this.Value = br.ReadInt32();
        }

        internal override void SaveValueToPage(BinaryWriter bw)
        {
            bw.Write(this.Time);
            bw.Write((int)this.Value);
        }
    }
}
