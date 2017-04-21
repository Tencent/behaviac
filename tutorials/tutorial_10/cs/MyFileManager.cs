using System;
using System.Collections.Generic;
using System.Text;

namespace tutorial_10
{
    public class MyFileManager : behaviac.FileManager
    {
        public MyFileManager()
        {
        }

        public override byte[] FileOpen(string filePath, string ext)
        {
            return base.FileOpen(filePath, ext);
        }

        public override void FileClose(string filePath, string ext, byte[] fileHandle)
        {
            base.FileClose(filePath, ext, fileHandle);
        }        
    }
}
