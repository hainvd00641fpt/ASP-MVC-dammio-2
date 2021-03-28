using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OneCMVC1.Models;

namespace OneCMVC1.Controllers
{
    public class LinksController : Controller
    {
        private OneCEntities db = new OneCEntities();

        // GET: Links
        public ActionResult Index(string sortOrder)
        {
            //1. Them bien NameSortParm de biet trang thai sap xep tang giam o View
            if (String.IsNullOrEmpty(sortOrder))
            {
                ViewBag.NameSortParm = "name_desc";
            }
            else
            {
                ViewBag.NameSortPart = "";
            }
            // ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "description_desc" : "{description";


            //2. truy van lay tat ca duong dan
            var links = from l in db.Links
                        select l;
            //3. sap xep theo thuoc tinh LinkName
            switch (sortOrder)
            {
                //3.1 neu bien sortName giam thi sap xep theo LinkName
                case "name_desc":
                    links = links.OrderByDescending(s => s.LinkName);
                break;
                //3.2 sap xep tang dan theo linkDescription
                case "Description":
                    links = links.OrderBy(s => s.LinkDescription);
                    break;
                //3.3 sap xep giam dan theo linkDescription
                case "description_desc":
                    links = links.OrderByDescending(s => s.LinkDescription);
                    break;

                //3.4 mac dinh sap xep tan
                default:
                    links = links.OrderBy(s => s.LinkName);
                    break;
            }
            //4.tra ket qua ra view
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
