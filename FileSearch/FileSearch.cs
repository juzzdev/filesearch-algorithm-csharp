using System;
using System.IO;
using System.Linq;

namespace FileSearch
{
    public class FileSearch
    {
        public enum SearchMode
        {
            BreadthFirst,
            DepthFirst
        }

        public string FindFile(string path, string searchPattern)
        {
            return FindFile(path, searchPattern, SearchMode.BreadthFirst, null);
        }

        public string FindFile(string path, string searchPattern, SearchMode mode)
        {
            return FindFile(path, searchPattern, mode, null);
        }

        public string FindFile(string path, string searchPattern, uint? maxDepth)
        {
            return FindFile(path, searchPattern, SearchMode.BreadthFirst, maxDepth);
        }

        public string FindFile(string path, string searchPattern, SearchMode mode, uint? maxDepth)
        {
            string filePath = null;

            switch (mode)
            {
                case SearchMode.BreadthFirst:
                    {
                        filePath = Directory.GetFiles(path, searchPattern).FirstOrDefault();

                        if (filePath == null)
                            filePath = FindFileBreadthFirst(path, searchPattern, maxDepth);
                        break;
                    }

                case SearchMode.DepthFirst:
                    {
                        filePath = FindFileDepthFirst(path, searchPattern, maxDepth);
                        break;
                    }
            }

            return filePath;
        }

        private string FindFileBreadthFirst(string path, string searchPattern, uint? maxDepth)
        {
            string filePath = null;

            // Only search subfolders if we have not exceeded the maximum depth.
            if (!maxDepth.HasValue || maxDepth.Value > 0)
            {
                try
                {
                    var subfolderPaths = Directory.GetDirectories(path);

                    // Search the top level of each subfolder first.
                    foreach (var subfolderPath in subfolderPaths)
                    {
                        filePath = Directory.GetFiles(subfolderPath, searchPattern).FirstOrDefault();

                        // Stop if a match is found.
                        if (filePath != null)
                            break;
                    }

                    // Continue deeper if a match has not been found.
                    if (filePath == null)
                    {
                        if (maxDepth.HasValue)
                            // We're going one level deeper.
                            maxDepth -= 1U;

                        // Search subfolders of each subfolder.
                        foreach (var subfolderPath in subfolderPaths)
                        {
                            filePath = FindFileBreadthFirst(subfolderPath, searchPattern, maxDepth);

                            // Stop if a match is found.
                            if (filePath != null)
                                break;
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    //log here
                }
            }

            return filePath;
        }

        private string FindFileDepthFirst(string path, string searchPattern, uint? maxDepth)
        {
            string filePath = null;

            try
            {
                // Search the current folder first.
                filePath = Directory.GetFiles(path, searchPattern).FirstOrDefault();

                // Only search subfolders if we have not exceeded the maximum depth.
                if (filePath == null && (!maxDepth.HasValue || maxDepth.Value > 0))
                {
                    if (maxDepth.HasValue)
                        // We're going one level deeper.
                        maxDepth -= 1U;

                    // Search each subfolder.
                    foreach (var subfolderPath in Directory.GetDirectories(path))
                    {
                        filePath = FindFileDepthFirst(subfolderPath, searchPattern, maxDepth);

                        // Stop if a match is found.
                        if (filePath != null)
                            break;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
               //log here
            }

            return filePath;
        }
    }
}
