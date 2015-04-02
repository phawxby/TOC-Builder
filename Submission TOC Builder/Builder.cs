using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Submission_TOC_Builder
{
    public partial class Builder : Form
    {
        public Builder()
        {
            InitializeComponent();

        }

        private void btn_SelectDirectory_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                if (!String.IsNullOrWhiteSpace(txt_Path.Text) && Directory.Exists(txt_Path.Text))
                {
                    dlg.SelectedPath = txt_Path.Text;
                }

                dlg.ShowDialog();

                var dir = new DirectoryInfo(dlg.SelectedPath);

                if (dir.Exists)
                {
                    txt_Path.Text = dlg.SelectedPath;

                    updateFoundLbl();
                }
                else
                {
                    MessageBox.Show("Selected path is invalid");
                }
            }
        }

        private void updateFoundLbl()
        {
            if (!string.IsNullOrEmpty(txt_Path.Text))
            {
                var dir = new DirectoryInfo(txt_Path.Text);

                if (dir.Exists)
                {
                    lbl_Found.Text = String.Format("Found {0} pdfs", GetValidPdfsInDirectory(dir, SearchOption.AllDirectories).Count());
                }
            }
            else
            {
                lbl_Found.Text = "";
            }
        }

        private void btn_Build_Click(object sender, EventArgs e)
        {
            Build();
        }

        private void Build()
        {
            var dir = new DirectoryInfo(txt_Path.Text);

            foreach (var subdir in dir.GetDirectories())
            {
                BuildPdfForDir(subdir, false);
            }

            BuildPdfForDir(dir, true);
            lbl_Found.Text = "Done!";
        }

        private void txt_Path_TextChanged(object sender, EventArgs e)
        {
            btn_Build.Enabled = (!String.IsNullOrWhiteSpace(txt_Path.Text) && Directory.Exists(txt_Path.Text));
        }

        private List<FileInfo> GetValidPdfsInDirectory(DirectoryInfo dir, SearchOption searchOption)
        {
            var returnLst = new List<FileInfo>();

            // Check we have a directory and that it exists
            if (dir.Exists)
            {
                // Loop each pdf file in the directory
                foreach (var file in dir.GetFiles("*.pdf", searchOption))
                {
                    // Get the depth
                    var depth = GetFileDepthFromDir(file, dir);

                    // If the file depth is less that <= 1 and it contains toc then it is a toc. We obviously don't want to include
                    // that again
                    if (depth <= 1 && file.Name.IndexOf("toc.pdf", StringComparison.OrdinalIgnoreCase) > -1)
                        continue;

                    returnLst.Add(file);
                }
            }

            return returnLst;
        }

        /// <summary>
        /// Gets the relative file path from the root directory
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private String GetFileRelativePathFromDir (FileInfo file, DirectoryInfo dir)
        {
            // Check we have a selected dir and it exists
            if (dir.Exists)
            {
                // Check the path ends with \ to make sure it is properly removed from the file
                var dirPath = dir.FullName;
                if (!dirPath.EndsWith("\\"))
                    dirPath += "\\";

                // If the file exists then replace the dir path with empty string to make it relative
                if (file.Exists && file.FullName.IndexOf(dirPath, StringComparison.OrdinalIgnoreCase) > -1)
                    return file.FullName.Replace(dirPath, "");
            }

            return null;
        }

        /// <summary>
        /// Gets the depth of the current file within the selected directory
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private Int32 GetFileDepthFromDir(FileInfo file, DirectoryInfo dir)
        {
            // Get the relative path
            var relativePath = GetFileRelativePathFromDir(file, dir);

            // Check it isn't null in case the file ges missing
            if (!String.IsNullOrWhiteSpace(relativePath))
            {
                // Split and count based on the number of \ for the depth
                return relativePath.Split('\\').Length - 1;
            }

            // Return 0 otherwise
            return 0;
        }

        private void BuildPdfForDir(DirectoryInfo dir, Boolean coreTechnicalDocument)
        {
            if (dir.Exists)
            {
                var doc = new XmlDocument();
                var htmlNode = doc.CreateElement("html");
                doc.AppendChild(htmlNode);

                var headNode = doc.CreateElement("head");
                htmlNode.AppendChild(headNode);

                if (File.Exists("Style/pdf-style.css"))
                {
                    var styleNode = doc.CreateElement("style");
                    headNode.AppendChild(styleNode);

                    var stylesNode = doc.CreateComment(File.ReadAllText("Style/pdf-style.css"));
                    styleNode.AppendChild(stylesNode);
                }

                var bodyNode = doc.CreateElement("body");
                htmlNode.AppendChild(bodyNode);

                var ulNode = doc.CreateElement("ul");
                bodyNode.AppendChild(ulNode);

                if (coreTechnicalDocument)
                {
                    foreach (var subDir in dir.GetDirectories())
                    {
                        foreach (var file in subDir.GetFiles("*.pdf"))
                        {
                            BuildListItemForFile(file, dir, ulNode);
                        }
                    }
                }
                else
                {
                    foreach (var subDir in dir.GetDirectories())
                    {
                        BuildListForDir(subDir, dir, ulNode, coreTechnicalDocument);
                    }

                    foreach (var file in GetValidPdfsInDirectory(dir, SearchOption.TopDirectoryOnly))
                    {
                        BuildListItemForFile(file, dir, ulNode);
                    }
                }

                var filename = String.Format("{0}-toc", coreTechnicalDocument ? "ctd" : dir.Name.ToLowerInvariant());

                // doc.Save(Path.Combine(dir.FullName, filename + ".html"));

                HtmlToPdf(doc, Path.Combine(dir.FullName, filename + ".pdf"));
            }
        }

        private void HtmlToPdf(XmlDocument doc, String savePath)
        {
            using (MemoryStream msOutput = new MemoryStream())
            using (TextReader reader = new StringReader(doc.OuterXml))
            {
                using (Document document = new Document(PageSize.A4, 30, 30, 30, 30))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, msOutput);

                    document.Open();

                    XMLWorkerHelper.GetInstance().ParseXHtml(
                        writer,
                        document,
                        reader);

                    document.Close();
                }

                byte[] buffer = msOutput.ToArray();
                using (FileStream fs = File.Create(savePath))
                {
                    fs.Write(buffer, 0, (int)buffer.Length);
                }
            }
        }

        private void BuildListForDir(DirectoryInfo dir, DirectoryInfo rootDir, XmlNode node, Boolean coreTechnicalDocument)
        {
            var li = node.OwnerDocument.CreateElement("li");
            node.AppendChild(li);

            var span = node.OwnerDocument.CreateElement("span");
            span.InnerText = dir.Name;
            li.AppendChild(span);

            var ulNode = node.OwnerDocument.CreateElement("ul");
            li.AppendChild(ulNode);

            if (!coreTechnicalDocument)
            {
                foreach (var subDir in dir.GetDirectories())
                {
                    BuildListForDir(subDir, rootDir, ulNode, coreTechnicalDocument);
                }
            }

            foreach (var file in GetValidPdfsInDirectory(dir, SearchOption.TopDirectoryOnly))
            {
                BuildListItemForFile(file, rootDir, ulNode);
            }
        }

        private void BuildListItemForFile(FileInfo file, DirectoryInfo rootDir, XmlNode node)
        {
            var li = node.OwnerDocument.CreateElement("li");
            node.AppendChild(li);

            var a = node.OwnerDocument.CreateElement("a");
            a.SetAttribute("href", GetFileRelativePathFromDir(file, rootDir));
            a.InnerText = file.Name;
            li.AppendChild(a);
        }

        private void Builder_Load(object sender, EventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (!String.IsNullOrWhiteSpace(args[1]) && Directory.Exists(args[1]))
                {
                    txt_Path.Text = args[1];
                }
            }

            Build();
        }
    }
}
