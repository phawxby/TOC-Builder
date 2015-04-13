using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Submission_TOC_Builder.Builders
{
    /// <summary>
    /// Builds a NeeS Table of Contents for the supplied directory
    /// </summary>
    public class NeesToc : ABuilder
    {
        public override int FoundFiles
        {
            get
            {
                if (this.ValidDir)
                {
                    return GetSourcePdfsInDirectory(_dir, _dir, SearchOption.AllDirectories).Count();
                }

                return 0;
            }
        }

        /// <summary>
        /// Performs a build of a Nees Toc
        /// </summary>
        /// <returns></returns>
        public override Boolean Build()
        {
            if (this.ValidDir)
            {
                foreach (var subdir in _dir.GetDirectories())
                {
                    BuildPdfForDir(subdir, false);
                }

                BuildPdfForDir(_dir, true);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the valid pdf's with the given directory
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        private List<FileInfo> GetSourcePdfsInDirectory(DirectoryInfo dir, DirectoryInfo rootDir, SearchOption searchOption)
        {
            var returnLst = new List<FileInfo>();

            // Check we have a directory and that it exists
            if (dir.Exists && rootDir.Exists)
            {
                // Loop each pdf file in the directory
                foreach (var file in dir.GetFiles("*.pdf", searchOption))
                {
                    // Get the depth
                    var depth = Helpers.Paths.GetDepthFromDir(file, rootDir);

                    // Set the maximum depth we should skip toc files
                    var maxDepth = 0;
                    // If dir and rootDir are the same we're looking at the ctd, step 1 level deeper
                    if (dir.FullName == rootDir.FullName)
                    {
                        maxDepth = 1;
                    }

                    // If the file depth is less that <= 1 and it contains toc then it is a toc. We obviously don't want to include
                    // that again
                    if (depth <= maxDepth && file.Name.IndexOf("toc.pdf", StringComparison.OrdinalIgnoreCase) > -1)
                        continue;

                    returnLst.Add(file);
                }
            }

            return returnLst;
        }

        /// <summary>
        /// Builds a PDF for the given dir.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="coreTechnicalDocument">Is this the root CTD?</param>
        private void BuildPdfForDir(DirectoryInfo dir, Boolean coreTechnicalDocument)
        {
            if (this.ValidDir)
            {
                var doc = new XmlDocument();
                var htmlNode = doc.CreateElement("html");
                doc.AppendChild(htmlNode);

                var headNode = doc.CreateElement("head");
                htmlNode.AppendChild(headNode);

                // If the pdf-styles.css file exists then use it to build the pdf
                // If companies want branding, they can do it here
                if (File.Exists("Style/pdf-style.css"))
                {
                    var styleNode = doc.CreateElement("style");
                    headNode.AppendChild(styleNode);

                    var stylesNode = doc.CreateComment(File.ReadAllText("Style/pdf-style.css"));
                    styleNode.AppendChild(stylesNode);
                }

                var bodyNode = doc.CreateElement("body");
                htmlNode.AppendChild(bodyNode);

                var ul = bodyNode.OwnerDocument.CreateElement("ul");
                bodyNode.AppendChild(ul);

                // Builds the file list for the supplied directory
                BuildForDir(dir, dir, ul, coreTechnicalDocument);

                // Create a sensible file name
                var filename = String.Format("{0}-toc", coreTechnicalDocument ? "ctd" : dir.Name.ToLowerInvariant());

                // doc.Save(Path.Combine(dir.FullName, filename + ".html"));

                // Save to PDF
                Helpers.Pdf.HtmlToPdf(doc, Path.Combine(dir.FullName, filename + ".pdf"));
            }
        }

        /// <summary>
        /// Builds a list for the supplied directory
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="rootDir"></param>
        /// <param name="node"></param>
        /// <param name="coreTechnicalDocument"></param>
        private void BuildForDir(DirectoryInfo dir, DirectoryInfo rootDir, XmlNode node, Boolean coreTechnicalDocument)
        {
            // A ctd has a slightly different structure, go straight into sub directories
            if (coreTechnicalDocument)
            {
                foreach (var subDir in dir.GetDirectories())
                {
                    BuildFileListItemsForDir(subDir, rootDir, node);
                }
            }
            else
            {
                // Build all dirs
                BuildDirListItemsForDir(dir, rootDir, node);
            }
        }

        /// <summary>
        /// Builds the list items for a specific directory
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="rootDir"></param>
        /// <param name="node"></param>
        private void BuildDirListItemsForDir(DirectoryInfo dir, DirectoryInfo rootDir, XmlNode node)
        {
            foreach (var subDir in dir.GetDirectories())
            {
                // Create a label for the dir
                var li = node.OwnerDocument.CreateElement("li");
                node.AppendChild(li);

                var span = node.OwnerDocument.CreateElement("span");
                span.InnerText = subDir.Name;
                li.AppendChild(span);

                var ul = node.OwnerDocument.CreateElement("ul");
                li.AppendChild(ul);

                BuildDirListItemsForDir(subDir, rootDir, ul);
            }

            BuildFileListItemsForDir(dir, rootDir, node);
        }

        /// <summary>
        /// Builds the list items for a specific directory
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="rootDir"></param>
        /// <param name="node"></param>
        private void BuildFileListItemsForDir(DirectoryInfo dir, DirectoryInfo rootDir, XmlNode node)
        {
            foreach (var file in GetSourcePdfsInDirectory(dir, rootDir, SearchOption.TopDirectoryOnly))
            {
                BuildListItemForFile(file, rootDir, node);
            }
        }

        /// <summary>
        /// Builds the individual list item for a file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rootDir"></param>
        /// <param name="node"></param>
        private void BuildListItemForFile(FileInfo file, DirectoryInfo rootDir, XmlNode node)
        {
            var li = node.OwnerDocument.CreateElement("li");
            node.AppendChild(li);

            var a = node.OwnerDocument.CreateElement("a");
            a.SetAttribute("href", Helpers.Paths.GetRelativePathFromDir(file, rootDir));
            a.InnerText = file.Name;
            li.AppendChild(a);
        }
    }
}
