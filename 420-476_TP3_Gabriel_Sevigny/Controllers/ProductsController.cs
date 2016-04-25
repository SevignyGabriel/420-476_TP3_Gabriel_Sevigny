using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _420_476_TP3_Gabriel_Sevigny.Models;
using System.IO;

namespace _420_476_TP3_Gabriel_Sevigny.Controllers
{
    public class ProductsController : Controller
    {
        private NorthwindEntities db = new NorthwindEntities();

        // GET: Products
        public ActionResult Index()
        {
            if (LoginController.isLoggedIn())
            {
                var products = db.Products.Include(p => p.Category).Include(p => p.Supplier);
                Create();
                return View(products.ToList());
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public ActionResult Index(String filter, String category)
        {
            if (LoginController.isLoggedIn())
            {
                Create();
                var products = db.Products.Where(p => p.CategoryID == 1);
                if (category != "All")
                    products = db.Products.Where(p => p.ProductName.Contains(filter) && p.Category.CategoryName == category).Include(p => p.Category).Include(p => p.Supplier);
                else
                    products = db.Products.Where(p => p.ProductName.Contains(filter)).Include(p => p.Category).Include(p => p.Supplier);
                return View(products.ToList());
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (LoginController.isLoggedIn())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            if (LoginController.isLoggedIn())
            {
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName");
                return View();
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued,Photo")] Product product)
        {
            if (LoginController.isLoggedIn())
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            if (MimeMapping.GetMimeMapping(file.FileName).Equals("image/png") || MimeMapping.GetMimeMapping(file.FileName).Equals("image/jpeg"))
                            {
                                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                var fileExtension = Path.GetExtension(file.FileName);
                                var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName + fileExtension);
                                file.SaveAs(path);
                                product.Photo = fileName + fileExtension;
                                db.Products.Add(product);
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            else
                                TempData["Error"] = "The file you're trying to upload isn't a 'png' or a 'jpg'!";
                        }
                        else
                            TempData["Error"] = "You didn't select any file to upload!";
                    }

                    return RedirectToAction("Create");
                }

                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
                return View(product);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (LoginController.isLoggedIn())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
                return View(product);
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued,Photo")] Product product)
        {
            if (LoginController.isLoggedIn())
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            if (MimeMapping.GetMimeMapping(file.FileName).Equals("image/png") || MimeMapping.GetMimeMapping(file.FileName).Equals("image/jpeg"))
                            {
                                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                var fileExtension = Path.GetExtension(file.FileName);
                                var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName + fileExtension);
                                file.SaveAs(path);
                                product.Photo = fileName + fileExtension;

                                db.Entry(product).State = EntityState.Modified;
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["Error"] = "The file you're trying to upload isn't a 'png' or a 'jpg'!";
                                return RedirectToAction("Edit");
                            }
                        }
                        else
                            TempData["Error"] = "You didn't select any file to upload!";
                    }

                    return RedirectToAction("Edit");
                }
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
                return View(product);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (LoginController.isLoggedIn())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (LoginController.isLoggedIn())
            {
                Product product = db.Products.Find(id);
                db.Products.Remove(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
