using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OneCMVC1.Models;
using System.Linq.Dynamic;
using System.Linq.Expressions;


namespace OneCMVC1.Controllers
{
    public class LinksController : Controller
    {
        private OneCEntities db = new OneCEntities();

        // GET: Links
        public ActionResult Index(string sortProperty, string sortOrder)
        {
            // 1. Tao bien ViewBag SortOrder de giu trang thai sap tang hay giam
            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "desc" : "";

            // 2. lay tat ca ten thuoc tinh cua lop Link (LinkID, LinkName, LinkURL,...)
            var properties = typeof(Link).GetProperties();
            string s = String.Empty;
            foreach (var item in properties)
            {
                // 2.1 kiem tra xem thuoc tinh nay la virtual (public virtual Category Category...)
                var isVirtual = item.GetAccessors()[0].IsVirtual;

                // 2.2. thuoc tih binh thuong thi cho phep sep xep
                if (!isVirtual)
                {
                    ViewBag.Headings += "<th><a href='?sortProperty=" + item.Name + "&sortOrder=" +
                        ViewBag.SortOrder + "'>" + item.Name + "</a></th>";
                }
                // 2.3. thuoc tinh virtual (public virtual Category Category...) thi khong duoc sxep
                // => can tao lien ket
                else ViewBag.Headings += "<th>" + item.Name + "</th>";
            }

            // 3. truy van lay tat ca duong dan
            var links = from l in db.Links
                        select l;

            // 4. tao thuoc tinh sap xep mac dinh la "LinkID"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "LinkID";

            // 5. sap xep tang giamr bang phuong thuc OrderBy su dung trong thu vien Dynamic LINQ
            if (sortOrder == "desc") links = links.OrderBy(sortProperty + " desc");
            else links = links.OrderBy(sortProperty);

            // 6. tra ket qua ra view
            return View(links.ToList());
        }

        // GET: Links/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Link link = db.Links.Find(id);
            if (link == null)
            {
                return HttpNotFound();
            }
            return View(link);
        }

        // GET: Links/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Links/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LinkID,LinkName,LinkURL,LinkDescription,CategoryID")] Link link)
        {
            if (ModelState.IsValid)
            {
                db.Links.Add(link);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", link.CategoryID);
            return View(link);
        }

        // GET: Links/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Link link = db.Links.Find(id);
            if (link == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", link.CategoryID);
            return View(link);
        }

        // POST: Links/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LinkID,LinkName,LinkURL,LinkDescription,CategoryID")] Link link)
        {
            if (ModelState.IsValid)
            {
                db.Entry(link).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", link.CategoryID);
            return View(link);
        }

        // GET: Links/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Link link = db.Links.Find(id);
            if (link == null)
            {
                return HttpNotFound();
            }
            return View(link);
        }

        // POST: Links/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Link link = db.Links.Find(id);
            db.Links.Remove(link);
            db.SaveChanges();
            return RedirectToAction("Index");
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
