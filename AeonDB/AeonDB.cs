using AeonDB.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB
{
    /// <summary>
    /// Class for managing the database as a whole.
    /// </summary>
    public sealed class AeonDB
    {
        private string directory;
        private TagDatabase tagDatabase;

        public AeonDB(string directory)
        {
            // Ensure the directory separator is appeneded to the path.
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                directory = directory + Path.DirectorySeparatorChar;
            }

            this.Directory = directory;
            Initialise();
        }

        /// <summary>
        /// Gets the directory the database is stored in.
        /// </summary>
        public string Directory
        {
            get { return directory; }
            private set { directory = value; }
        }

        public Tag CreateTag(string name, TagType type)
        {
            return this.tagDatabase.CreateNewTag(name, type);
        }

        public Tag GetTag(string name)
        {
            return this.tagDatabase[name];
        }

        /// <summary>
        /// Initialises the database. Creates the folder structure if it does not already exist.
        /// </summary>
        private void Initialise()
        {
            try
            {
                var dirInfo = new DirectoryInfo(this.Directory);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }

                this.tagDatabase = new TagDatabase(this);
            }
            catch (Exception e)
            {
                throw new AeonException("Failed to initialise database.", e);
            }
        }
    }
}
