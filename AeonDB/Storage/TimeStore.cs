using AeonDB.Structure;
using AeonDB.Tags;
using AeonDB.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB.Storage
{
    internal class Timestore : IDisposable
    {
        /// <summary>
        /// The size of the file's header.
        /// </summary>
        private const int HeaderSize = sizeof(long) // firstTimeSaved
            + sizeof(long) // lastTimeSaved
            + sizeof(long) // currentPage
            + sizeof(int) //pageCount
            + sizeof(int) // version
            + 8; // magic number

        private const long MagicNumber = 0x41454f4e544d5354;
        private const int Version = 1;

        private BTree index;
        private Tag tag;
        private string fileName;
        private Timestamp lastTimeSaved;
        private Timestamp firstTimeSaved;
        private Page currentPage;
        private int pageCount;
        private FileStream file;

        internal Timestore(Tag tag, string fileName)
        {
            this.tag = tag;
            this.fileName = fileName;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void Initalise()
        {
            if (File.Exists(this.fileName) && File.Exists(this.fileName + ".index"))
            {
                // Already exists. Load from disk
                this.index = new BTree(this.fileName + ".index");
                Load();
            }
            else if (File.Exists(this.fileName) && !File.Exists(this.fileName + ".index"))
            {
                // Index missing
                throw new AeonException("Index missing for tag: " + this.tag.Name);
            }
            else
            {
                // Create new
            }
        }

        /// <summary>
        /// Loads the tree's header and root node.
        /// </summary>
        private void Load()
        {
            this.file = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            var header = new byte[HeaderSize];
            file.Read(header, 0, HeaderSize);

            long magicNumber;
            int version;

            using (var ms = new MemoryStream(header))
            {
                using (var br = new BinaryReader(ms))
                {
                    magicNumber = br.ReadInt64();
                    version = br.ReadInt32();
                    this.firstTimeSaved = new Timestamp(br.ReadInt64());
                    this.lastTimeSaved = new Timestamp(br.ReadInt64());
                    var currentPagePosition = br.ReadInt64();
                    this.pageCount = br.ReadInt32();
                }
            }

            if (magicNumber != MagicNumber)
            {
                throw new AeonException("Unrecognised file format");
            }

            if (version < Version)
            {
                // Older format. Upgrade.
            }
            else if (version > Version)
            {
                throw new AeonException("Database created on newer of AeonDB. You must upgrade software to use.");
            }



            this.file.Close();
            this.file = null;
        }
    }
}
