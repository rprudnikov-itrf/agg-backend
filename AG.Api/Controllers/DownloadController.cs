using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AG.Core.Helpers;
using System.IO;

namespace AG.Api.Controllers
{
    public class DownloadController : Controller
    {
        [ValidateInput(false)]
        public ActionResult Index(string file)
        {
            if (string.IsNullOrEmpty(file))
                return new HttpStatusCodeResult(400);

            var fileContents = YandexDiskHelper.Share.Download(file);
            if (fileContents == null || string.IsNullOrEmpty(fileContents.href))
                return new HttpStatusCodeResult(404);

            return Redirect(fileContents.href);
        }
    }
}
