using System;

namespace UnityTest
{
    public class InvalidPathException : Exception
    {
        public InvalidPathException(string path)
            : base("Invalid path part " + path) {
        }
    }
}