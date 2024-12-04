using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Desktoptale
{
    public static class FileEnumerator
    {
        public static IEnumerable<string> EnumerateFilesRecursive(string path, string searchPattern)
        {
            List<string> outputPaths = new List<string>();

            try
            {
                IEnumerable<string> directories = Directory.EnumerateDirectories(path).OrderBy(d => d);
                foreach (string directory in directories)
                {
                    outputPaths.AddRange(EnumerateFilesRecursive(directory, searchPattern));
                }
            }
            catch (Exception e) { }

            try
            {
                IEnumerable<string> files = Directory.EnumerateFiles(path, searchPattern);
                outputPaths.AddRange(files.OrderBy(s => s));
            } 
            catch (Exception e) { }
            
            return outputPaths;
        }
    }
}