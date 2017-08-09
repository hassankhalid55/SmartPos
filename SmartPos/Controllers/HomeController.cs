using Infragistics.Web.Mvc;
using NHibernate.Linq;
using NHibernate.Transform;
using SmartPos.Models;
using SmartPos.NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartPos.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        // GET: Home/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        // GET: Home/Create
        public ActionResult Create(Product product)
        {
            using (var session = NHibernateSessionHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var prod = session.Query<Product>().FirstOrDefault(x => x.Pid == product.Pid);
                    if (prod != null)
                    {//aq
                        prod.Description = product.Description;
                        prod.Price = product.Price;
                        prod.Qty += product.Qty;
                        session.SaveOrUpdate(prod);
                    }
                    else
                        session.Save(product);
                    transaction.Commit();
                }
                session.Close();
            }
            return RedirectToAction("DisplayProducts");
            
        }

        [GridDataSourceAction]
        public ActionResult DisplayProducts()
        {
            using(var session = NHibernateSessionHelper.OpenSession())
            {
                var query = (from x in session.Query<Product>()
                             select x);

                //var query1 = from product in session.Query<Product>()
                //             join stockin in session.Query<StockIn>()
                //             on product.Pid equals stockin.Pid
                //             group

                //string sqlQuery = "select p.Sn as Sn, p.Pid as Pid, p.[Description] as Description, p.Price as Price , sum(s.StockedIn) as Qty from POS_db.dbo.Product " +
                //                  "p join POS_db.dbo.StockIn s on p.Pid = s.Pid group by p.Pid, p.[Description], p.Sn, p.Price";

//                string sqlQuery1 = @"select product.Sn, product.Pid, Product.Description, Product.Price
//, IsNull(stocks.StockedIn-stocks.StockedOut, stocks.StockedIn) as 'Qty' from product join
//(select stockins.ProductId, stockins.Stockedin, stockouts.StockedOut
//from
//(select s.Pid as 'ProductId', sum(s.StockedIn) as 'StockedIn' from StockIn s group by
//s.Pid) stockins left join
//(select o.Pid as 'ProductId', sum(o.StockedOut) as 'StockedOut' 
//from StockOut o group by
//o.Pid) stockouts on stockins.ProductId = stockouts.ProductId) stocks on 
//product.Pid = stocks.ProductId";

                string sqlQuery1 = @"select product.Sn, product.Pid, Product.Description, stocks.Price as Price
, IsNull(stocks.StockedIn-stocks.StockedOut, stocks.StockedIn) as 'Qty' from product join
(select stockins.ProductId, stockins.Stockedin, stockouts.StockedOut, stockins.Price
from (select s.Pid as 'ProductId', sum(s.StockedIn) as 'StockedIn', sum(Price)/count(Pid) as Price from StockIn s group by s.Pid) stockins 
left join
(select o.Pid as 'ProductId', sum(o.StockedOut) as 'StockedOut' 
from StockOut o group by
o.Pid) stockouts on stockins.ProductId = stockouts.ProductId
) stocks on 
product.Pid = stocks.ProductId
";

                //var query1 = (session.CreateSQLQuery(sqlQuery).SetResultTransformer(Transformers.AliasToBean<Product>()));
                var query2 = session.CreateSQLQuery(sqlQuery1).SetResultTransformer(Transformers.AliasToBean<Product>());

                //List<Product> p = new List<Product>(query1.List<Product>());
                List<Product> products = new List<Product>(query2.List<Product>());

                //List < Product > products = query.ToList<Product>();
                //return View(products.AsQueryable<Product>());

                return View(products.AsQueryable<Product>());
            }
        }

        public class ExtendedProduct
        {

        }


        [GridDataSourceAction]
        public ActionResult GenerateOrder()
        {
            List<ExtendedSalesItem> eSaleItems = new List<ExtendedSalesItem>();
            return View(eSaleItems.AsQueryable<ExtendedSalesItem>());
        }

        public void SaveOrder()
        {
            string customerName = Session["customerName"].ToString();
            DateTime orderDate = DateTime.Parse(Session["orderDate"].ToString());
            float totalAmount = float.Parse(Session["totalAmount"].ToString());
            //Session["customerName"] = null;
            //Session["orderDate"] = null;
            string orderId = orderDate.ToString("yyMMddHHmmss");

            //create customer
            Customer customer = new Customer();
            customer.Cid = long.Parse(orderId);
            customer.Name = customerName;

            //Create Order
            Order order = new Order();
            order.Oid = long.Parse(orderId);
            order.Date = DateTime.Parse(orderDate.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            order.Cid = customer.Cid;
            order.GrossTotal = totalAmount;

            //create salesItem
            SalesItem sItem = new SalesItem();
            //Session.Clear();
            using (var session = NHibernateSessionHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(customer);
                    transaction.Commit();
                    session.Close();

                }
            }

            using (var session = NHibernateSessionHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(order);
                    transaction.Commit();
                    session.Close();
                }
            }

            //Save data from Order grid into salesitem table
            ViewData["GenerateCompactJSONResponse"] = false;
            GridModel m = new GridModel();
            List<Transaction<ExtendedSalesItem>> salesItemTransaction = m.LoadTransactions<ExtendedSalesItem>(HttpContext.Request.Form["ig_transactions"]);
            foreach (Transaction<ExtendedSalesItem> t in salesItemTransaction)
            {
                var row = t.row;
                ExtendedSalesItem p = new ExtendedSalesItem();
                if (t.type == "row") //update record
                {
                    int id = int.Parse(t.rowId);
                    {
                        using (var session = NHibernateSessionHelper.OpenSession())
                        {
                            using (var transaction = session.BeginTransaction())
                            {
                                //ExtendedSalesItem esi = session.Get<ExtendedSalesItem>(id);
                                //SalesItem si = new SalesItem();
                                //si.Pid = esi.


                                ////session


                                //session.Update(p);
                                //transaction.Commit();
                                //session.Close();
                            }
                        }
                    }
                }
                else if (t.type == "newrow") //new row added
                {
                    if (row != null)
                    {
                        SalesItem si = new SalesItem();
                        si.Sid = row.Sn;
                        si.Pid = row.Pid;
                        si.Qty = row.Qty;
                        si.Oid = order.Oid;
                        si.Total = row.Total;
                   
                        using (var session = NHibernateSessionHelper.OpenSession())
                        {
                            using (var transaction = session.BeginTransaction())
                            {
                                session.Save(si);
                                transaction.Commit();
                                session.Close();
                            }
                        }
                    }
                }
                else if (t.type == "deleterow")
                {

                    //var id = t.rowId;
                    //{
                    //    using (var session = NHibernateSessionHelper.OpenSession())
                    //    using (var transaction = session.BeginTransaction())
                    //    {
                    //        p = session.Get<Product>(int.Parse(id));
                    //        session.Delete(p);
                    //        transaction.Commit();
                    //        session.Close();
                    //    }
                    //}
                }
            }

        }

        
        public JsonResult getDnC(string customerName, string totalAmount)
        {
            if (customerName != null && totalAmount != null)
            {
                Session.Clear();
                Session["customerName"] = customerName;
                Session["orderDate"] = DateTime.UtcNow;
                Session["totalAmount"] = totalAmount;
                return Json(new { status = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = "false" }, JsonRequestBehavior.AllowGet);
        }

        public void UpdateProductsWhenOrderSaved()
        {
            ViewData["GenerateCompactJSONResponse"] = false;
            GridModel m = new GridModel();
            List<Transaction<Product>> productTransactions = m.LoadTransactions<Product>(HttpContext.Request.Form["ig_transactions"]);
            foreach (Transaction<Product> t in productTransactions)
            {
                var row = t.row;
                Product p = new Product();
                if (t.type == "row") //update record
                {
                    int id = int.Parse(t.rowId);
                    {
                        using (var session = NHibernateSessionHelper.OpenSession())
                        {
                            using (var transaction = session.BeginTransaction())
                            {

                                p = session.Get<Product>(id);
                                p.Pid = row.Pid;

                                //calculate quantity of product available
                                string queryForAvailableQty = "select isnull((select sum(s.StockedIn) as 'StockedIn' from StockIn s where Pid = " + p.Pid + " group by s.Pid), 0) - " +
                                                              "isnull((select sum(o.StockedOut) as 'StockedIn' from StockOut o where Pid = " + p.Pid + " group by o.Pid), 0) as query";

                                int query = session.CreateSQLQuery(queryForAvailableQty).UniqueResult<int>();
                                //string s = query.ToString();
                                //int avQty =0;
                                //if (s != null)
                                //{
                                //    avQty = int.Parse(s);
                                //}

                                int qtyStockedOut = query - row.Qty;
                                p.Price = row.Price;
                                p.Qty = row.Qty;
                                p.Description = row.Description;





                                //add new stock in entry
                                StockOut stockOut = new StockOut();
                                stockOut.Date = DateTime.UtcNow;
                                stockOut.Pid = row.Pid;
                                stockOut.Price = row.Price;
                                stockOut.StockedOut = qtyStockedOut;

                                session.Save(stockOut);

                                session.Update(p);
                                transaction.Commit();
                                session.Close();
                            }
                        }
                    }
                }
                else if (t.type == "newrow") //new row added
                {
                    if (row != null)
                    {
                        p.Pid = row.Pid;
                        p.Price = row.Price;
                        p.Qty = row.Qty;
                        p.Description = row.Description;
                        using (var session = NHibernateSessionHelper.OpenSession())
                        {
                            using (var transaction = session.BeginTransaction())
                            {
                                session.Save(p);
                                transaction.Commit();
                                session.Close();
                            }
                        }
                    }
                }
                else if (t.type == "deleterow")
                {

                    var id = t.rowId;
                    {
                        using (var session = NHibernateSessionHelper.OpenSession())
                        using (var transaction = session.BeginTransaction())
                        {
                            p = session.Get<Product>(int.Parse(id));
                            session.Delete(p);
                            transaction.Commit();
                            session.Close();
                        }
                    }
                }
            }
        }

        public ActionResult UpdateProducts()
        {

            ViewData["GenerateCompactJSONResponse"] = false;
            GridModel m = new GridModel();
            List<Transaction<Product>> productTransactions = m.LoadTransactions<Product>(HttpContext.Request.Form["ig_transactions"]);
            //foreach (Transaction<Product> t in productTransactions)
            if (productTransactions.Count == 0)
            {
                return View();
            }
            Transaction<Product> t = productTransactions.Last();
            {
                var row = t.row;
                Product p = new Product();
                if (t.type == "row") //update record
                {
                    int id = int.Parse(t.rowId);
                    {
                        using (var session = NHibernateSessionHelper.OpenSession())
                        {
                            using (var transaction = session.BeginTransaction())
                            {
                                p = session.Get<Product>(id);
                                p.Pid = row.Pid;
                                p.Price = 1;
                                p.Qty = row.Qty;
                                p.Description = row.Description;

                                session.Update(p);

                                //add new stock in entry
                                StockIn stockIn = new StockIn();
                                stockIn.Date = DateTime.UtcNow;
                                stockIn.Pid = row.Pid;
                                stockIn.StockedIn = row.Qty;
                                stockIn.Price = row.Price;

                                session.Save(stockIn);

                                transaction.Commit();
                                session.Close();
                            }
                        }
                    }
                }
                else if (t.type == "newrow") //new row added
                {
                    if (row != null)
                    {
                        p.Pid = row.Pid;
                        p.Price = 1;
                        p.Qty = row.Qty;
                        p.Description = row.Description;
                        using (var session = NHibernateSessionHelper.OpenSession())
                        {
                            using (var transaction = session.BeginTransaction())
                            {
                                session.Save(p);

                                //add new stock in entry
                                StockIn stockIn = new StockIn();
                                stockIn.Date = DateTime.UtcNow;
                                stockIn.Pid = row.Pid;
                                stockIn.Price = row.Price;
                                stockIn.StockedIn = row.Qty;

                                session.Save(stockIn);

                                transaction.Commit();
                                session.Close();
                            }
                        }
                    }
                }
                else if (t.type == "deleterow")
                {

                    var id = t.rowId;
                    {
                        using (var session = NHibernateSessionHelper.OpenSession())
                        using (var transaction = session.BeginTransaction())
                        {
                            p = session.Get<Product>(int.Parse(id));
                            session.Delete(p);
                            transaction.Commit();
                            session.Close();
                        }
                    }
                }
                
            }
            return View();
        }

        public ActionResult CreateOrder()
        {
            return View();
        }

        public ActionResult Temp()
        {
            return View();
        }

        //// POST: Home/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Home/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
