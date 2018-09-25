using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace AG.Core.Helpers
{
    public static class DocumentHelper
    {
        public static byte[] ConvetToPdf(byte[] xml)
        {
            object paramSourceDocPath = Path.GetTempFileName();
            object paramMissing = System.Type.Missing;

            System.IO.File.WriteAllBytes((string)paramSourceDocPath, xml);
            if (!System.IO.File.Exists((string)paramSourceDocPath))
                throw new Exception(string.Format("Исходный файл для генерации отчета не найден '{0}'", paramSourceDocPath));

            var wordApplication = null as Application;
            var wordDocument = null as Document;
            var paramExportFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + "-" + DateTime.Now.Ticks.ToString() + ".pdf");
            try
            {
                wordApplication = new Application() { DisplayAlerts = WdAlertLevel.wdAlertsNone };

                var paramExportFormat = WdExportFormat.wdExportFormatPDF;
                var paramOpenAfterExport = false;
                var paramExportOptimizeFor = WdExportOptimizeFor.wdExportOptimizeForPrint;
                var paramExportRange = WdExportRange.wdExportAllDocument;
                var paramStartPage = 0;
                var paramEndPage = 0;
                var paramExportItem = WdExportItem.wdExportDocumentContent;
                var paramIncludeDocProps = true;
                var paramKeepIRM = true;
                var paramCreateBookmarks = WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                var paramDocStructureTags = true;
                var paramBitmapMissingFonts = true;
                var paramUseISO19005_1 = false;

                // Open the source document.
                wordDocument = wordApplication.Documents.Open(
                    ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing);

                // Export it in the specified format.
                if (wordDocument != null)
                    wordDocument.ExportAsFixedFormat(paramExportFilePath,
                        paramExportFormat, paramOpenAfterExport,
                        paramExportOptimizeFor, paramExportRange, paramStartPage,
                        paramEndPage, paramExportItem, paramIncludeDocProps,
                        paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                        paramBitmapMissingFonts, paramUseISO19005_1,
                        ref paramMissing);

                //Domains.AddLog("file", ErrorGroup.ConvertPDF, paramExportFilePath);
                if (System.IO.File.Exists(paramExportFilePath))
                {
                    //throw new Exception(string.Format("Word-PDF: Файл отчета не найден '{0}'", paramExportFilePath));
                    return System.IO.File.ReadAllBytes(paramExportFilePath);
                }
            }
            finally
            {
                // Close and release the Document object.
                if (wordDocument != null)
                {
                    wordDocument.Close(false, ref paramMissing, ref paramMissing);
                    wordDocument = null;
                }

                // Quit Word and release the ApplicationClass object.
                if (wordApplication != null)
                {
                    wordApplication.Quit(false, ref paramMissing, ref paramMissing);
                    wordApplication = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }

                if (System.IO.File.Exists((string)paramSourceDocPath))
                    System.IO.File.Delete((string)paramSourceDocPath);
                if (System.IO.File.Exists(paramExportFilePath))
                    System.IO.File.Delete(paramExportFilePath);
            }

            return null;
        }
    }
}