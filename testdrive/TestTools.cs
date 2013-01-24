#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace TestTools
{
    public class TestDriveException : Exception
    {
        public TestDriveException() : base()
        {
        }

        public TestDriveException(string message) : base(message)
        {
        }

        public TestDriveException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TestDriveException(SerializationInfo info,
                                     StreamingContext context)
        {
        }
    }

    public class Drive
    {
        #region Reason enum

        public enum Reason
        {
            Read,
            Save
        }

        #endregion

        private static string TESTDRIVE_ROOT;

        private readonly string _directory;
        private readonly Reason _reason;

        public Drive(string relativePath, Reason reason)
        {
            var directory = Path.Combine(GetDriveRoot(), relativePath);
            if (!Directory.Exists(directory) && reason == Reason.Read)
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
            _reason = reason;
            _directory = directory;
        }

        public static string GetDriveRoot()
        {
            if (TESTDRIVE_ROOT == null)
            {
                TESTDRIVE_ROOT = Environment.GetEnvironmentVariable("TESTDRIVE_ROOT");
                if (TESTDRIVE_ROOT == null)
                {
                    throw new TestDriveException(
                        "TESTDRIVE_ROOT Environment Variable not found - please ensure you have it set");
                }
            }
            return TESTDRIVE_ROOT;
        }

        public IEnumerable<String> FullFilePaths()
        {
            return Files().Select(file => Path.Combine(_directory, file));
        }

        public IEnumerable<String> Files()
        {
            return Directory.EnumerateFiles(_directory);
        }

        public IEnumerable<String> Files(string prefix)
        {
            return Directory.EnumerateFiles(_directory).Where(filename => filename.StartsWith(Path.Combine(this._directory,prefix)));
        }

        public int FileCount()
        {
            return Files().Count();
        }

        public int FileCount(string prefix)
        {
            return Files(prefix).Count();
        }

        public void Save(Bitmap bitmap, string prefix)
        {
            if (_reason == Reason.Save)
            {
                bitmap.Save(Path.Combine(_directory, prefix + "_" + (FileCount(prefix) + 1)), ImageFormat.Jpeg);
            }
            else
            {
                throw new TestDriveException("Attempted to Save File in Read Only Directory");
            }
        }

        public void Save(IEnumerable<Bitmap> bitmaps, string prefix)
        {
            foreach (Bitmap bitmap in bitmaps)
            {
                Save(bitmap, prefix);
            }
        }
    }
}