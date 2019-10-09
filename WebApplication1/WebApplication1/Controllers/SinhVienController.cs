using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace WebApplication1.Controllers
{
    public class SinhVienController : Controller
    {
        QuanLySinhVienEntities db = new QuanLySinhVienEntities();
        public ActionResult Index(string ten,string maSV, int? page)
        {
            var sv = from l in db.SinhViens // lấy toàn bộ liên kết
                        select l;

            if (!String.IsNullOrEmpty(ten)) // kiểm tra chuỗi tìm kiếm có rỗng/null hay không
            {
                sv = sv.Where(s => s.HoTen.Contains(ten)); //lọc theo chuỗi tìm kiếm
            }
  
         
            if (!String.IsNullOrEmpty(maSV))
            {
                sv = sv.Where(s => s.SinhVienID.ToString().Contains(maSV));
            }
            return View(sv.OrderByDescending(m => m.SinhVienID).ToPagedList(page ?? 1, 10));
        }

        //Them mon hoc
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                if (db.SinhViens.Any(model => model.SinhVienID == sinhVien.SinhVienID))
                {
                    TempData["message"] = "Mã sinh viên đã tồn tại.";

                }
                else
                {
                    db.SinhViens.Add(sinhVien);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            return View(sinhVien);
        }


        //Sua mon hoc
        [HttpGet]
        public ActionResult Edit(int id)
        {

            SinhVien sinhVien = db.SinhViens.Find(id);
            return View(sinhVien);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "SinhVienID,Lop,HoTen")] SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sinhVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        //xoa

        public ActionResult Delete(int id)
        {
            List<Diem> sv = db.Diems.Where(s => s.SinhVienID == id).ToList();
            if (sv.Count != 0)
            {
                TempData["message"] = "Sinh viên đang có trong bảng điểm, không thể xóa.";
                return RedirectToAction("Index");
            }
            else
            {
                SinhVien sinhVien = db.SinhViens.Find(id);
                return View(sinhVien);
            }
            
        }

        // POST: PersonalDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SinhVien sinhVien = db.SinhViens.Find(id);
            db.SinhViens.Remove(sinhVien);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //Xem điểm
        [HttpGet]
        public ActionResult Score(int id)
        {
            var sv = db.SinhViens.Where(s => s.SinhVienID == id).FirstOrDefault();
            //Diem
            var diemCuaSinhVien = db.Diems.Where(s => s.SinhVienID == id).ToList();
            //MonHoc
            var monHocCuaSinhVien = db.MonHocs.ToList();
            ViewBag.Message = sv.HoTen;
            var sinhVienRecord = from d in diemCuaSinhVien
                                 join m in monHocCuaSinhVien on d.MonHocID equals m.MonHocID
                                 select new BangDiem{
                                     ten = m.Ten,
                                     diemTongKet = d.DiemTongKet,
                                     soTinChi= m.SoTinChi,
                                     diemTinChi = d.DiemTongKet*m.SoTinChi
                                    
                                 };
            return View(sinhVienRecord);
        }
    }
}