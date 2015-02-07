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
        private static readonly Size HeaderSize = new Size(
            Timestamp.TypeSize  // firstTimeSaved
            + Timestamp.TypeSize // lastTimeSaved
            + Position.TypeSize // currentPage
            + sizeof(int) //pageCount
            + sizeof(int) // version
            + 8); // magic number

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
            this.Initalise();
        }

        public void Dispose()
        {
            
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
                this.index = null;

                this.firstTimeSaved = new Timestamp(0);
                this.lastTimeSaved = new Timestamp(0);

                // First page would go immediately after the header.
                this.currentPage = GetNewPage(new Position(HeaderSize));
                this.pageCount = 1;
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
            Position currentPagePosition;

            using (var ms = new MemoryStream(header))
            {
                using (var br = new BinaryReader(ms))
                {
                    magicNumber = br.ReadInt64();
                    version = br.ReadInt32();
                    this.firstTimeSaved = new Timestamp(br.ReadInt64());
                    this.lastTimeSaved = new Timestamp(br.ReadInt64());
                    currentPagePosition = new Position(br.ReadInt64());
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

            if (this.pageCount > 0)
            {
                this.currentPage = this.GetNewPage(currentPagePosition);
                this.currentPage.Load(this.file);
            }


            this.file.Close();
            this.file = null;
        }

        private Page GetNewPage(Position currentPagePosition)
        {
            Page newPage;

            switch (this.tag.Type)
            {
                case TagType.Double:
                    newPage = new DoublePage(currentPagePosition);
                    break;
                case TagType.Float:
                    newPage = new FloatPage(currentPagePosition);
                    break;
                case TagType.Boolean:
                    newPage = new BooleanPage(currentPagePosition);
                    break;
                case TagType.Int16:
                    newPage = new Int16Page(currentPagePosition);
                    break;
                case TagType.Int32:
                    newPage = new Int32Page(currentPagePosition);
                    break;
                case TagType.Int64:
                    newPage = new Int64Page(currentPagePosition);
                    break;
                default:
                    throw new AeonException("Unrecognised tag type.");
            }

            return newPage;
        }

        private void CreateFile()
        {
            if (this.pageCount == 0 || this.currentPage == null || this.currentPage.ValueCount == 0)
            {
                throw new InvalidOperationException("Cannot save timestore until it has at least 1 value.");
            }

            this.file = new FileStream(this.fileName, FileMode.Create, FileAccess.Write, FileShare.None);

            this.UpdateHeader(file);
            this.currentPage.Save(file);

            this.file.Close();
            this.file = null;
        }

        private void UpdateHeader(FileStream file) 
        {
            file.Seek(0, SeekOrigin.Begin);

            var header = new byte[HeaderSize];
            using (var ms = new MemoryStream(header))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(MagicNumber); // Magic number
                    bw.Write(Version); // Version
                    bw.Write(this.firstTimeSaved); //First Time Saved
                    bw.Write(this.lastTimeSaved); // Last Time Saved
                    bw.Write(this.currentPage.Position); // Current Page Position
                    bw.Write(this.pageCount); // Page Count
                }
            }

            file.Write(header, 0, HeaderSize);
        }

        public void AddValue(Timestamp timestamp, object value)
        {
            if (this.index == null && this.currentPage.ValueCount == 0)
            {
                // First value to timestore.
                this.firstTimeSaved = timestamp;
                this.lastTimeSaved = timestamp;
                this.currentPage.AddValue(timestamp, value);

                this.index = new BTree(this.fileName + ".index", 50);
                this.index.Open();
                this.index.Insert(this.currentPage.PageTime, this.currentPage.Position);
                this.index.Close();

                this.CreateFile();
                return;
            }

            if (timestamp < this.firstTimeSaved)
            {
                throw new ArgumentException("Timestamp must be after the first value in timestore.");
            }

            if (timestamp < this.lastTimeSaved)
            {
                throw new ArgumentException("Timestamp must be after the last value in the timestore.");
            }

            this.Open();

            if (this.currentPage.ValueCount >= Page.PageValueCount || timestamp >= this.currentPage.PageTime + Page.PageValueCount)
            {
                // Current Page is full. Need to add new. There should be a page every Page.PageValueCount seconds whether it has values or not.
                Position newPagePosition;
                Timestamp newPageTime;
                do
                {
                    this.currentPage.Save(this.file);
                    newPagePosition = new Position(this.currentPage.Position + this.currentPage.PageSize);
                    newPageTime = new Timestamp(this.currentPage.PageTime + Page.PageValueCount);
                    this.currentPage = GetNewPage(newPagePosition);
                    this.currentPage.PageTime = newPageTime;
                    this.index.Insert(newPageTime, newPagePosition);
                } while (newPageTime + Page.PageValueCount < timestamp);
            }

            this.currentPage.AddValue(timestamp, value);
            this.currentPage.Save(this.file);
            this.Close();
        }

        private void Open()
        {
            this.file = new FileStream(this.fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            this.index.Open();
        }

        private void Close()
        {
            this.file.Close();
            this.file = null;
            this.index.Close();
        }
    }
}
