using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class TestDriveException : Exception
    {
        public TestDriveException() : base() { }
        public TestDriveException(string message) : base(message) { }
        public TestDriveException(string message, System.Exception inner) : base(message, inner) { }  
        protected TestDriveException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class Drive{
        private static string TESTDRIVE_ROOT;
        public static string GetDriveRoot()
        {
            if ( Drive.TESTDRIVE_ROOT == null )
            {
                Drive.TESTDRIVE_ROOT = System.Environment.GetEnvironmentVariable("TESTDRIVE_ROOT");
                if (Drive.TESTDRIVE_ROOT == null)
                {
                    throw new TestDriveException("TESTDRIVE_ROOT Environment Variable not found - please ensure you have it set");
                }
            }
            return Drive.TESTDRIVE_ROOT;
        }

        private readonly string _directory;
        private readonly Reason _reason;
        
        public IEnumerable<String> Files()
        {
            return Directory.EnumerateFiles(this._directory);
        }

        public IEnumerable<String> Files(string prefix)
        {
            return Directory.EnumerateFiles(this._directory).Where(filename => filename.StartsWith(prefix));
        }

        public int FileCount()
        {
            return Files().Count();
        }

        public int FileCount(string prefix)
        {
            return Files(prefix).Count();
        }

        public Drive(string relativePath, Reason reason )
        {
           var directory = Path.Combine(Drive.GetDriveRoot(), relativePath);
           if ( !Directory.Exists(directory) && reason == Reason.Read )
           {
               throw new TestDriveException("no such directory: " + directory);
           }
           else if (Directory.Exists(directory) && reason == Reason.Save)
           {
               throw new TestDriveException("Attempting to open an existing directory: " + directory);
           }
           else if (!Directory.Exists(directory) && reason == Reason.Save)
           {
                Directory.CreateDirectory(directory);
           }
            this._reason = reason;
            this._directory = directory;
        }

        public void Save(Bitmap bitmap, string prefix)
        {
            if (_reason == Reason.Save)
            {
                bitmap.Save(Path.Combine(this._directory, prefix+"_"+(FileCount(prefix)+1)),ImageFormat.Jpeg);
            }
            else
            {
                throw new TestDriveException("Attempted to Save File in Read Only Directory");
            }
        }

        public void Save(IEnumerable<Bitmap> bitmaps, string prefix)
        {
            foreach(Bitmap bitmap in bitmaps)
            {
                Save(bitmap, prefix);
            }
        }

        public enum Reason { Read, Save }
    }
}
