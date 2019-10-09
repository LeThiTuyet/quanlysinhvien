using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace WebApplication1.Controllers
{
    public class MonHocController : Controller
    {
        // GET: MonHoc
        QuanLySinhVienEntities db = new QuanLySinhVienEntities();
        public ActionResult Index(string ten, string maMH, int? page)
        {
            var sv = from l in db.MonHocs // lấy toàn bộ liên kết
                     select l;

            if (!String.IsNullOrEmpty(ten)) // kiểm tra chuỗi tìm kiếm có rỗng/null hay không
            {
                sv = sv.Where(s => s.Ten.Contains(ten)); //lọc theo chuỗi tìm kiếm
            }


            if (!String.IsNullOrEmpty(maMH))
            {
                sv = sv.Where(s => s.MonHocID.ToString().Contains(maMH));
            }
            return View(sv.OrderByDescending(m => m.MonHocID).ToPagedList(page ?? 1, 10));
        }

        //Them mon hoc
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(MonHoc monHoc)
        {
            if (ModelState.IsValid)
            {
                if (db.MonHocs.Any(model=>model.MonHocID==monHoc.MonHocID))
                {
                    TempData["message"] = "Mã môn học đã tồn tại.";
                
                }
                else
                {
                    db.MonHocs.Add(monHoc);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                
            }
            return View(monHoc);
        }


        //Sua mon hoc
        [HttpGet]
        public ActionResult Edit(int id)
        {

            MonHoc monHoc = db.MonHocs.Find(id);          
            return View(monHoc);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "MonHocID,Ten,SoTinChi")] MonHoc monHoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monHoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        //xoa

        public ActionResult Delete(int id)
        {
            List<Diem> mh = db.Diems.Where(s => s.MonHocID == id).ToList();
            if (mh.Count != 0)
            {
                TempData["message"] = "Môn học đang có trong bảng điểm, không thể xóa.";
                return RedirectToAction("Index");
            }
            else
            {
                MonHoc monHoc= db.MonHocs.Find(id);
                return View(monHoc);
            }
        }

        // POST: PersonalDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MonHoc monHoc = db.MonHocs.Find(id);
            db.MonHocs.Remove(monHoc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}