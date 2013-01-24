using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        public readonly string UnitTestFolder = "TestDriveUnitTest";
        public readonly string TestMaterialFolder = "RawShreds";

        [TestMethod]
        public void GetDriveRootTest()
        {
            Assert.IsNotNull(Drive.GetDriveRoot());
        }

        [TestMethod]
        public void TestSaveAndLoad()
        {
            const string saveDirectory = "TestSaving";
            RemoveDirectory(saveDirectory);
            var saveFolder = new TestTools.Drive(Path.Combine(UnitTestFolder, saveDirectory), TestTools.Drive.Reason.Save);
            Assert.IsTrue( saveFolder.FileCount()==0 );

            var loadFolder = new TestTools.Drive(Path.Combine(UnitTestFolder, TestMaterialFolder), TestTools.Drive.Reason.Read);
            var totalFileCount = loadFolder.FileCount();
            Assert.IsTrue( totalFileCount>0);

            var bitmaps = new List<System.Drawing.Bitmap>(totalFileCount);
            foreach( string filepath in loadFolder.FullFilePaths() )
            {
                var bitmap = new System.Drawing.Bitmap(filepath);
                bitmaps.Add(bitmap);
            }
            Assert.IsTrue( bitmaps.Count==totalFileCount);

            saveFolder.Save(bitmaps,"unittest");
            Assert.IsTrue( saveFolder.FileCount("unittest")==totalFileCount);
            Assert.IsTrue(saveFolder.FileCount()==totalFileCount);
        }

        [TestMethod]
        public void TestExceptions()
        {

        }

        private void RemoveDirectory(string directory)
        {
            var path = Path.Combine(Drive.GetDriveRoot(), UnitTestFolder, directory);
            if (!Directory.Exists(path))
            {
                return;
            }
            var files = Directory.EnumerateFiles(path);
            foreach(string file in files)
            {
                File.Delete( file );
            }
            Directory.Delete(path);
        }
    }
}
