using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AG.Core.Task;

namespace AG.Api.Controllers
{
    public class DissolutionController : Controller
    {
        //http://doc.rostaxi.info/dissolution?agg=d1bf2d6baf30419f8addad3bb0ed1d7b&db=1b06dd703adf4944b880dbeb891077a8
        public ActionResult Index(string agg, string db)
        {
            var model = СгенерироватьРасторжение.Run(agg, db, Server.MapPath("/bin"));
            if (model == null)
                return new HttpStatusCodeResult(404);

            return RedirectToAction(null, "download", new { file = model.public_key });
        }
    }
}
