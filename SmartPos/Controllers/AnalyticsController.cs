using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Highsoft.Web.Mvc.Charts;
using Newtonsoft.Json;
using SmartPos.NHibernate;

namespace SmartPos.Controllers
{
    public class AnalyticsController : Controller
    {
        // GET: Analytics
        public ActionResult Dashboard()
        {
            
            return View();
        }

        public JsonResult GetData()
        {
            List<List<double>> totalSales = new List<List<double>>();
            
            //GET TOTAL SALES FROM ORDER TABLE AND CONVERT DATES TO JS SUPPORTED FORMAT 
            using (var session = NHibernateSessionHelper.OpenSession())
            {
                var date = DateTime.Today.ToString("MM/dd/yyyy");
                var query = "select IsNull(sum(GrossTotal),0) from [Order] o where convert(Date,o.[Date]) = '" + date  + "'";
                double sales = session.CreateSQLQuery(query).UniqueResult<double>();
                totalSales.Add(new List<double>(new double[] { DateTime.Parse(date).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds, sales }));
                for (int c = -1; c > -7; c--)
                {
                    date = DateTime.Today.AddDays(c).ToString("MM/dd/yyyy");
                    query = "select IsNull(sum(GrossTotal),0) from [Order] o where convert(Date,o.[Date]) = '" + date  + "'";
                    sales = session.CreateSQLQuery(query).UniqueResult<double>();
                    totalSales.Add(new List<double>(new double[] { DateTime.Parse(date).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds, sales }));
                }
                session.Close();
            }
            var json = JsonConvert.SerializeObject(totalSales);
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
    }

    public class sale
    {
        public DateTime Date { set; get; }
        public float Sale { set; get; }
    }

    
}