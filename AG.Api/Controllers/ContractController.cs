using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;
using AG.Core.Helpers;
using AG.Core.Models;
using AG.Core.Task;

namespace AG.Api.Controllers
{
    public class ContractController : Controller
    {
        //http://doc.rostaxi.info/contract?agg=d1bf2d6baf30419f8addad3bb0ed1d7b&db=1b06dd703adf4944b880dbeb891077a8
        public ActionResult Index(string agg, string db)
        {
            //return new HttpStatusCodeResult(404);

            var model = СгенерироватьДоговор.Run(agg, db, Server.MapPath("/bin"));
            if (model == null)
                return new HttpStatusCodeResult(404);

            return RedirectToAction(null, "download", new { file = model.public_key });
        }
    }
}