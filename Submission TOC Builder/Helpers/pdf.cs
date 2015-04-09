using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Submission_TOC_Builder.Helpers
{
    public static class pdf
    {
        /// <summary>
        /// Turns the supplied html into a pdf
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="savePath"></param>
        public static void HtmlToPdf(XmlDocument doc, String savePath)
        {
            HtmlToPdf(doc.OuterXml, savePath);
        }

        /// <summary>
        /// Turns the supplied html into a pdf
        /// </summary>
        /// <param name="html"></param>
        /// <param name="savePath"></param>
        public static void HtmlToPdf(String html, String savePath)
        {
            //------------------------------------------------
            // First we create the PDF using the html
            //------------------------------------------------

            // Create a memory stream to hold the inital PDF in memory rather than temp files
            using (MemoryStream msOutput = new MemoryStream())
            {
                // Read in the html
                using (TextReader reader = new StringReader(html))
                {
                    // Create an A4 sized document
                    using (Document document = new Document(PageSize.A4, 30, 30, 30, 30))
                    {
                        // Add a creation date 
                        document.AddCreationDate();

                        // Get an instance of the PdfWriter to output it directly back into the memory stream
                        PdfWriter writer = PdfWriter.GetInstance(document, msOutput);

                        // Open the document for writing
                        document.Open();

                        // Convert the HTML to PDF
                        XMLWorkerHelper.GetInstance().ParseXHtml(
                            writer,
                            document,
                            reader);

                        // Close the document
                        document.Close();
                    }
                }

                //------------------------------------------------
                // Then we must convert the URI links to gotor links to move directly between PDF's
                //------------------------------------------------

                // Pull the PDF from memory
                byte[] buffer = msOutput.ToArray();
                // SOURCE: http://stackoverflow.com/questions/8140339/using-itextsharp-to-extract-and-update-links-in-an-existing-pdf

                // Read the PDF from memory 
                using (var reader = new PdfReader(buffer))
                {
                    // Loop through each page individually. We can't do this in one go unfortunately. 
                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        // Get all elements on the page
                        var page = reader.GetPageN(i);
                        // And then filter it to annotations
                        var annots = page.GetAsArray(PdfName.ANNOTS);

                        //Make sure we have something
                        if ((annots == null) || (annots.Length == 0))
                            continue;

                        //Loop through each annotation
                        foreach (PdfObject annot in annots.ArrayList)
                        {
                            //Convert the itext-specific object as a generic PDF object
                            PdfDictionary AnnotationDictionary = (PdfDictionary)PdfReader.GetPdfObject(annot);

                            //Make sure this annotation has a link
                            if (!AnnotationDictionary.Get(PdfName.SUBTYPE).Equals(PdfName.LINK))
                                continue;

                            //Get the ACTION for the current annotation
                            PdfDictionary AnnotationAction = (PdfDictionary)AnnotationDictionary.Get(PdfName.A);

                            // Check we have something again
                            if (AnnotationAction != null)
                            {
                                // Pull the URI for the action
                                var uriObj = AnnotationAction.Get(PdfName.URI);

                                // If it has a URI
                                if (uriObj != null && uriObj.IsString())
                                {
                                    // Convert it to a string
                                    var uri = uriObj.ToString();
                                    // And then check if it's a PDF
                                    if (!String.IsNullOrWhiteSpace(uri) && uri.EndsWith(".pdf"))
                                    {
                                        // Replace the / in the path to \ for windows
                                        uri = uri.Replace('/', '\\');

                                        // Remove the URI
                                        AnnotationAction.Remove(PdfName.URI);
                                        // Change to GOTOR and set the file
                                        AnnotationAction.Put(PdfName.S, PdfName.GOTOR);
                                        AnnotationAction.Put(PdfName.F, new PdfString(uri));
                                    }
                                }
                            }
                        }
                    }

                    //------------------------------------------------
                    // Now we save the creation to disk
                    //------------------------------------------------

                    // Create a file to write out to
                    using (FileStream FS = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        // Create a document to copy into and a write to push to disk
                        using (Document Doc = new Document())
                        using (PdfCopy writer = new PdfCopy(Doc, FS))
                        {
                            Doc.Open();
                            // Loop and add each page to the writer
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                writer.AddPage(writer.GetImportedPage(reader, i));
                            }

                            // Close and save
                            Doc.Close();
                        }
                    }

                    // Close the inital reader, we're all done
                    reader.Close();
                }
            }
        }
    }
}
