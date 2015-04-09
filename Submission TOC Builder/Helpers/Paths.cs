using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Submission_TOC_Builder.Helpers
{
    public static class Paths
    {
        /// <summary>
        /// Gets the relative file path from the root directory
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static String GetRelativePathFromDir(FileSystemInfo path, FileSystemInfo rootDir)
        {
            // Check the path ends with \ to make sure it is properly removed from the file
            var dirPath = rootDir.FullName;
            if (!dirPath.EndsWith("\\"))
                dirPath += "\\";

            // If the file exists then replace the dir path with empty string to make it relative
            if (path.FullName.IndexOf(dirPath, StringComparison.OrdinalIgnoreCase) > -1)
            {
                var relative = path.FullName.Replace(dirPath, "");
                return relative;
            }

            return null;
        }

        /// <summary>
        /// Gets the depth of the current file within the selected directory. Returns -1 if it's not in the supplied directory
        /// </summary>
        /// <example>
        /// Root dir:   c:/foo/
        /// File:       c:/foo/cheese.pdf               Depth: 0
        /// File:       c:/foo/bar/cheese.pdf           Depth: 1
        /// File:       c:/zebra/cheese.pdf             Depth: -1
        /// </example>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Int32 GetDepthFromDir(FileSystemInfo path, FileSystemInfo rootDir)
        {
            // Get the relative path
            var relativePath = GetRelativePathFromDir(path, rootDir);

            // Check it isn't null in case the file ges missing
            if (!String.IsNullOrWhiteSpace(relativePath))
            {
                // Split and count based on the number of \ for the depth
                return relativePath.Split('\\').Length - 1;
            }

            // Return 0 otherwise
            return -1;
        }
    }
}
