using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeonDB
{
    public class AeonDB
    {
        private string directory;

        public AeonDB(string directory)
        {
            this.Directory = directory;
            Initialise();
        }

        public string Directory
        {
            get { return directory; }
            private set { directory = value; }
        }

        private void Initialise()
        {
            try
            {
                var dirInfo = new DirectoryInfo(this.Directory);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
            }
            catch (Exception e)
            {
                throw new AeonException("Failed to initialise database.", e);
            }
        }
    }
}
