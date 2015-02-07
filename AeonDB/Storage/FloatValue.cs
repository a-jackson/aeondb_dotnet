using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    public class FloatValue : StoredValue
    {
        public FloatValue()
        {
            this.Time = new Timestamp(0);
            this.Value = default(float);
        }

        internal override void ReadValueFromPage(BinaryReader br)
        {
            this.Time = new Timestamp(br.ReadInt64());
            this.Value = br.ReadSingle();
        }

        internal override void SaveValueToPage(BinaryWriter bw)
        {
            bw.Write(this.Time);
            bw.Write((float)this.Value);
        }
    }
}
