using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.IO;
using AG.Core.Helpers;
using AG.Core.Models;

namespace AG.Core.Task
{
    public static class СчетаФактуры
    {
        public static void Run()
        {
            var company = AggregatorHelper.Client.List();
            var culture = System.Globalization.CultureInfo.GetCultureInfo("ru");
            var files = System.IO.Directory.GetFiles(@"E:\factur", "*.xls");
            var opt = new ParallelOptions() { MaxDegreeOfParallelism = 4 };
            Parallel.ForEach(files, opt, file =>
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var matchName = Regex.Match(fileName, "Счет-фактура № (\\d+) от (.*)", RegexOptions.Singleline);
                    Console.WriteLine(fileName);

                    if (!matchName.Success)
                        return;

                    var number = Convert.ToInt32(matchName.Groups[1].Value);
                    var date = DateTime.ParseExact(matchName.Groups[2].Value, "dd MMMM yyyy г", culture);

                    object paramMissing = System.Type.Missing;

                    //var fileSign = Path.Combine(Environment.CurrentDirectory, "Resources", "sign.png");
                    var excelApplication = new Microsoft.Office.Interop.Excel.Application() { DisplayAlerts = false };
                    var excelDocument = null as Workbook;
                    var paramExportFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + "-" + DateTime.Now.Ticks.ToString() + ".pdf");

                    var paramExportFormat = XlFixedFormatType.xlTypePDF;
                    var paramExportQuality = XlFixedFormatQuality.xlQualityStandard;
                    var paramOpenAfterPublish = false;
                    var paramIncludeDocProps = true;
                    var paramIgnorePrintAreas = false;
                    var paramFromPage = System.Type.Missing;
                    var paramToPage = System.Type.Missing;
                    var move = false;

                    try
                    {
                        // Open the source document.
                        excelDocument = excelApplication.Workbooks.Open(
                            file, paramMissing, paramMissing,
                            paramMissing, paramMissing, paramMissing,
                            paramMissing, paramMissing, paramMissing,
                            paramMissing, paramMissing, paramMissing,
                            paramMissing, paramMissing, paramMissing);

                        var xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelDocument.Worksheets.get_Item(1);
                        var rowINN = xlWorkSheet.Cells[12, 2].Value as string;
                        var companyId = null as AggregatorClient;
                        if (rowINN != null)
                        {
                            var match = Regex.Match(rowINN, ": (\\d+)/", RegexOptions.Singleline);
                            if (!match.Success)
                                match = Regex.Match(rowINN, ": (\\d+)", RegexOptions.Singleline);

                            if (match.Success && !string.IsNullOrEmpty(match.Groups[1].Value))
                            {
                                var innValue = match.Groups[1].Value.Trim();
                                if ("7731293323".Equals(innValue))
                                    innValue = "7731313315";
                                else if ("7727251419".Equals(innValue))
                                    innValue = "771365144342";
                                else if ("5042128001".Equals(innValue))
                                    innValue = "501108116528";
                                else if ("7716739123".Equals(innValue))
                                    innValue = "7716247019";
                                else if ("502771269434".Equals(innValue))
                                    innValue = "5027186030";
                                else if ("7733738426".Equals(innValue))
                                    innValue = "771402533692";
                                else if ("7703707413".Equals(innValue))
                                    innValue = "772195151101";
                                else if ("7722826092".Equals(innValue))
                                    innValue = "691006984966";
                                else if ("561407678107".Equals(innValue))
                                    innValue = "505204236857";
                                else if ("772506576396".Equals(innValue))
                                    innValue = "772195151101";

                                var rowName = xlWorkSheet.Cells[10, 2].Value as string;
                                var matchCompanyName = Regex.Match(rowName, ": (.+)", RegexOptions.Singleline);

                                var companyItem = company
                                    .Where(p => p.Company != null && !string.IsNullOrWhiteSpace(p.Company.INN) && string.Equals(p.Company.INN.Trim(), innValue))
                                    .ToArray();

                                if (companyItem.Count() > 1)
                                {
                                    companyItem = companyItem
                                        .Where(p => matchCompanyName.Groups[1].Value.ToUpper().Contains(p.Company.OrgName.ToUpper().Trim().TrimEnd('.')))
                                        .ToArray();
                                }

                                if (companyItem.Count() == 0)
                                {
                                    Console.WriteLine(innValue + " = " + file + ": " + companyItem.Count());
                                    return;
                                }
                                else if (companyItem.Count() == 1)
                                {
                                    companyId = companyItem.First();
                                }
                                else
                                {
                                    companyId = companyItem.Where(p => p.Enable && p.Contract != null).OrderBy(p => p.Contract.Date).FirstOrDefault();
                                    if (companyId == null)
                                    {
                                        Console.WriteLine(companyItem.Count());
                                        return;
                                    }
                                }
                            }
                        }

                        var domain = AggregatorHelper.Client.Get(companyId.Agg, companyId.Db);
                        if (domain == null)
                            return;

                        var rowCost = xlWorkSheet.Cells[18, 23].Value as double?;
                        if (!rowCost.HasValue || rowCost.Value == 0)
                            Console.WriteLine(file + ": cost = 0");

                        // добавить подпись и печать
                        //if (!string.IsNullOrEmpty(fileSign))
                        //{
                        //    var oRange = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[17, 6];
                        //    var Left = (float)((double)oRange.Left);
                        //    var Top = (float)((double)oRange.Top);
                        //    var Weight = excelApplication.Application.CentimetersToPoints(5.60);
                        //    var Height = excelApplication.Application.CentimetersToPoints(4.80);
                        //    xlWorkSheet.Shapes.AddPicture(fileSign, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, (float)Weight, (float)Height);
                        //}

                        // Export it in the specified format.
                        if (excelDocument != null)
                        {
                            xlWorkSheet.PageSetup.Orientation = XlPageOrientation.xlLandscape;
                            xlWorkSheet.PageSetup.FitToPagesTall = 1;
                            xlWorkSheet.PageSetup.FitToPagesWide = 1;
                            xlWorkSheet.PageSetup.Zoom = false;

                            excelDocument.ExportAsFixedFormat(paramExportFormat,
                                paramExportFilePath, paramExportQuality,
                                paramIncludeDocProps, paramIgnorePrintAreas, paramFromPage,
                                paramToPage, paramOpenAfterPublish,
                                paramMissing);

                            if (System.IO.File.Exists(paramExportFilePath))
                            {
                                var pdf = System.IO.File.ReadAllBytes(paramExportFilePath);
                                var aggregatorFile = new AggregatorFile()
                                {
                                    Date = date,
                                    FileName = string.Format("{0}_invoice_{1:yyyyMMdd}.pdf", domain.Login, date),
                                    Group = StaticHelper.GroupBill,
                                    Description = string.Format("Счет-фактура №{0} от {1:dd MMMM yyyy}", number, date)
                                };

                                var path = YandexDiskHelper.Folders.Combine(StaticHelper.ClientsFolder, domain.Contract.Number);
                                if (!YandexDiskHelper.Folders.Exist(path))
                                    YandexDiskHelper.Folders.Add(path);

                                var remoteFile = YandexDiskHelper.Folders.Combine(path, aggregatorFile.FileName);
                                YandexDiskHelper.Files.Upload(remoteFile, pdf);

                                var resource = YandexDiskHelper.Share.Publish(remoteFile);
                                if (resource != null && !string.IsNullOrEmpty(resource.public_key))
                                {
                                    aggregatorFile.FileUrl = StaticHelper.GenerateDownloadUrl(resource.public_key);

                                    AggregatorHelper.File.Add(domain.Agg, domain.Db, aggregatorFile);
                                }

                                move = true;
                            }
                        }
                    }
                    finally
                    {
                        // Close and release the Document object.
                        if (excelDocument != null)
                        {
                            excelDocument.Close(false, paramMissing, paramMissing);
                            excelDocument = null;
                        }

                        // Quit Word and release the ApplicationClass object.
                        if (excelApplication != null)
                        {
                            excelApplication.Quit();
                            excelApplication = null;

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }

                        if (System.IO.File.Exists(paramExportFilePath))
                            System.IO.File.Delete(paramExportFilePath);

                        if (move)
                            System.IO.File.Move(file, file.Replace("factur", "factur-ok"));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}
