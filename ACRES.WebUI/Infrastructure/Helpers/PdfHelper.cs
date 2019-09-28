using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MagicApps.Infrastructure.Helpers;
using MagicApps.Infrastructure.Services;
using MagicApps.Models;
using Microsoft.AspNet.Identity.Owin;
using ACRES.Domain.Context;
using ACRES.Domain.Entities;
using ACRES.Domain.Models;

namespace ACRES.WebUI.Infrastructure.Helpers
{

    public class PdfHelper
    {
        private ApplicationDbContext db;

        public string ServiceUserId;

        public PdfHelper(string serviceUserId)
        {
            this.db = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            this.ServiceUserId = serviceUserId;
        }

        public void Split(string pdfFilePath, string outputPath, int page)
        {
            const int pageNameSuffix = 1;

            // Intialize a new PdfReader instance with the contents of the source Pdf file:  
            PdfReader reader = new PdfReader(pdfFilePath);

            FileInfo file = new FileInfo(pdfFilePath);
            string pdfFileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + "-";

            SplitAndSaveInterval(pdfFilePath, outputPath, 1, page, string.Format(pdfFileName + "{0}", pageNameSuffix));

        }

        private void SplitAndSaveInterval(string pdfFilePath, string outputPath, int startPage, int interval, string pdfFileName)
        {
            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                Document document = new Document();
                PdfCopy copy = new PdfCopy(document, new FileStream(string.Format("{0}\\{1}.pdf", outputPath, pdfFileName), FileMode.Create));
                document.Open();

                for (int pagenumber = startPage; pagenumber < (startPage + interval); pagenumber++)
                {
                    if (reader.NumberOfPages >= pagenumber)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, pagenumber));
                    }
                    else
                    {
                        break;
                    }

                }

                document.Close();
            }
        }

    }

    public class ITextEvents : PdfPageEventHelper
    {

        // This is the contentbyte object of the writer
        PdfContentByte cb;
        // we will put the final number of pages in a template
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer
        BaseFont baseFont = null;
        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;

        // we override the onOpenDocument method
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            string error;

            try
            {
                PrintTime = DateTime.Now;
                baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
                error = de.Message;
            }
            catch (System.IO.IOException ioe)
            {
                error = ioe.Message;
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            Rectangle pageSize = document.PageSize;

            cb.BeginText();
            cb.SetFontAndSize(baseFont, 8);
            cb.SetTextMatrix(pageSize.GetLeft(35), pageSize.GetTop(20));
            //cb.ShowText("Reference # " + this.Reference.ReferenceId);
            cb.SetRGBColorFill(100, 100, 100);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(baseFont, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
                "",
                pageSize.GetRight(35),
                pageSize.GetTop(20), 0);
            cb.EndText();

            // Otherwise, changes text of main body
            cb.SetRGBColorFill(0, 0, 0);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            int pageN = writer.PageNumber;
            String text = "Page " + pageN;
            float len = baseFont.GetWidthPoint(text, 8);

            Rectangle pageSize = document.PageSize;
            cb.SetRGBColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(baseFont, 8);
            cb.SetTextMatrix(pageSize.GetLeft(35), pageSize.GetBottom(20));
            cb.ShowText(text);
            cb.EndText();
            cb.AddTemplate(template, pageSize.GetLeft(35) + len, pageSize.GetBottom(20));

            cb.BeginText();
            cb.SetFontAndSize(baseFont, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
                "Last updated: " + PrintTime.ToString("dd/MM/yyyy HH:mm"),
                pageSize.GetRight(35),
                pageSize.GetBottom(20), 0);
            cb.EndText();

            //address on footer
            string address = String.Join(", ", Settings.COMPANY_ADDRESS);
            cb.BeginText();
            cb.SetFontAndSize(baseFont, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER,
                address,
                pageSize.GetLeft(250),
                pageSize.GetBottom(20), 0);
            cb.EndText();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            //template.BeginText();
            // template.SetFontAndSize(bf, 8);
            // template.SetTextMatrix(0, 0);
            // template.ShowText("" + (writer.PageNumber - 1));
            // template.EndText();
        }
    }
}