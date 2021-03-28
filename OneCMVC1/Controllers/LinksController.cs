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
            //ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "desc" : "";
            if (sortOrder == "asc") ViewBag.SortOrder = "desc";
            if (sortOrder == "desc") ViewBag.SortOder = "";
            if (sortOrder == "") ViewBag.SortOder = "asc";

            // 2. Lấy tất cả tên thuộc tính của lớp Link (LinkID, LinkName, LinkURL,...)
            var properties = typeof(Link).GetProperties();

            // 2.0. Tạo 1 danh List với mỗi phần tử là kiểu Tuple
            // Tuple<string, bool, int> với tham số lần lượt là <Name, IsVirtual, Order>
            // tức là Tên thuộc tính, Thuộc tính là virtual hay không và Thứ tự thuộc tính
            List<Tuple<string, bool, int>> list = new List<Tuple<string, bool, int>>();
            foreach (var item in properties)
            {
                int order = 999;
                var isVirtual = item.GetAccessors()[0].IsVirtual;

                if (item.Name == "LinkName") order = 1;
                if (item.Name == "LinkID") order = 2;
                if (item.Name == "LinkDescription") order = 3;
                if (item.Name == "LinkURL") order = 4;
                if (item.Name == "CategoryID") continue; // Không hiển thị cột này
                Tuple<string, bool, int> t = new Tuple<string, bool, int>(item.Name, isVirtual, order);
                list.Add(t);
            }

            // 2.1. Sắp xếp theo thứ tự ở trên
            list = list.OrderBy(x => x.Item3).ToList();

            foreach (var item in list)
            {
                // 2.2. Thuộc tính bình thường thì cho phép sắp xếp
                if (!item.Item2) // Item2 dùng để kiểm tra thuộc tính ảo hay không?
                {
                    // 2.3. So thuộc tính sortProperty và sortOrder để biết thuộc tính nào cần thay biểu tượng sắp giảm
                    if (sortOrder == "desc" && sortProperty == item.Item1)
                    {
                        ViewBag.Headings += "<th><a href='?sortProperty=" + item.Item1 + "&sortOrder=" +
                            ViewBag.SortOrder + "'>" + item.Item1 + "<i class='fa fa-fw fa-sort-desc'></i></th></a></th>";
                    }
                    else if (sortOrder == "asc" && sortProperty == item.Item1)
                    {
                        ViewBag.Headings += "<th><a href='?sortProperty=" + item.Item1 + "&sortOrder=" +
                            ViewBag.SortOrder + "'>" + item.Item1 + "<i class='fa fa-fw fa-sort-asc'></a></th>";
                    }
                    else
                    {
                        ViewBag.Headings += "<th><a href='?sortProperty=" + item.Item1 + "&sortOrder=" +
                           ViewBag.SortOrder + "'>" + item.Item1 + "<i class='fa fa-fw fa-sort'></a></th>";
                    }

                }
                // 2.4. Thuộc tính virtual (public virtual Category Category...) thì không sắp xếp được
                // cho nên không cần tạo liên kết
                else ViewBag.Headings += "<th>" + item.Item1 + "</th>";
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
