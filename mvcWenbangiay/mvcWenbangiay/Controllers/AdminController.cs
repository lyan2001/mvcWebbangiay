    using mvcWenbangiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;
using PagedList.Mvc;
using System.IO;
using static mvcWenbangiay.Models.allow;

namespace mvcWenbangiay.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbQLbangiayDataContext db = new dbQLbangiayDataContext();
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                return View();
            }

        }
        public ActionResult Giay(int? page,string Tukhoa, string currentFilter)
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                var lstProduct = new List<GIAY>();
                if (Tukhoa != null)
                {
                    page = 1;
                }
                else
                {
                    Tukhoa = currentFilter;
                }
                if(!string.IsNullOrEmpty(Tukhoa))
                {
                    //lay ds theo tu khoa tim kiem
                    lstProduct = db.GIAYs.Where(n => n.Tengiay.Contains(Tukhoa)).ToList();
                }
                else
                {
                    //lay tat ca sp trong bang GIAY
                    lstProduct = db.GIAYs.ToList();
                }
                ViewBag.CurrentFilter = Tukhoa;
                int pageNumber = (page ?? 1);
                int pageSize = 5;
                //sap xep sp theo id sp, sp moi dua len dau
                return View(lstProduct.ToPagedList(pageNumber, pageSize));

            }               
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    //ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Giay", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        [HttpGet]
        
        public ActionResult Themmoigiay()
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                //Dua du lieu vao dropwnlist
                ViewBag.MaHG = new SelectList(db.HANGGIAYs.ToList().OrderBy(n => n.TenHG), "MaHG", "TenHG");
                ViewBag.MaLG = new SelectList(db.LOAIGIAYs.ToList().OrderBy(n => n.TenLoaiGiay), "MaLG", "TenLoaiGiay");
                return View();
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoigiay(GIAY giay, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload1, HttpPostedFileBase fileUpload2)
        {
            ViewBag.MaHG = new SelectList(db.HANGGIAYs.ToList().OrderBy(n => n.TenHG), "MaHG", "TenHG");
            ViewBag.MaLG = new SelectList(db.LOAIGIAYs.ToList().OrderBy(n => n.TenLoaiGiay), "MaLG", "TenLoaiGiay");
            //ktra duong dan file
            if (fileUpload == null || fileUpload1 == null || fileUpload2 == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }

            //the vao csdl
            else
            {
                if (ModelState.IsValid)
                {
                    //ảnh chính
                    //luu ten file
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName);


                    // ảnh phụ 1
                    var fileName1 = Path.GetFileName(fileUpload1.FileName);
                    var path1 = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName1);

                    // ảnh phụ 2
                    var fileName2 = Path.GetFileName(fileUpload2.FileName);
                    var path2 = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName2);

                    //ktra hinh anh da ton tai chua
                    //ảnh chính
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    //ảnh phụ 1
                    if (System.IO.File.Exists(path1))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //luu hinh anh vao duong dan
                        fileUpload1.SaveAs(path1);
                    }
                    // ảnh phụ 2
                    if (System.IO.File.Exists(path2))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //luu hinh anh vao duong dan
                        fileUpload2.SaveAs(path2);
                    }


                    giay.Anhbia = fileName;
                    giay.Anhbia1 = fileName1;
                    giay.Anhbia2 = fileName2;
                    //luu vao csdl
                    db.GIAYs.InsertOnSubmit(giay);
                    db.SubmitChanges();
                }
                return RedirectToAction("Giay");
            }
        }
        public ActionResult Chitietgiay(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                GIAY giay = db.GIAYs.SingleOrDefault(n => n.Magiay == id);
                ViewBag.Magiay = giay.Magiay;
                if (giay == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(giay);
            }

        }
        [HttpGet]
        public ActionResult Xoagiay(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                GIAY giay = db.GIAYs.SingleOrDefault(n => n.Magiay == id);
                ViewBag.Magiay = giay.Magiay;
                if (giay == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(giay);
            }

        }
        [HttpPost, ActionName("Xoagiay")]
        public ActionResult Xacnhanxoa(int id)
        {
            GIAY giay = db.GIAYs.SingleOrDefault(n => n.Magiay == id);
            ViewBag.Magiay = giay.Magiay;
            if (giay == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.GIAYs.DeleteOnSubmit(giay);
            db.SubmitChanges();
            return RedirectToAction("Giay");
        }
        [HttpGet]
        public ActionResult Suagiay(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                GIAY giay = db.GIAYs.SingleOrDefault(n => n.Magiay == id);

                if (giay == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewBag.MaHG = new SelectList(db.HANGGIAYs.ToList().OrderBy(n => n.TenHG), "MaHG", "TenHG");
                ViewBag.MaLG = new SelectList(db.LOAIGIAYs.ToList().OrderBy(n => n.TenLoaiGiay), "MaLG", "TenLoaiGiay");
                return View(giay);
            }
        }
        [HttpPost, ActionName("Suagiay")]
        public ActionResult Capnhatgiay(int id)
        {
            GIAY giay = db.GIAYs.Where(n => n.Magiay == id).SingleOrDefault();
            UpdateModel(giay); //Cap nhât
            db.SubmitChanges();
            return RedirectToAction("Giay");
        }
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult Suagiay(GIAY giay, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload1, HttpPostedFileBase fileUpload2)
        //{
        //    ViewBag.MaHG = new SelectList(db.HANGGIAYs.ToList().OrderBy(n => n.TenHG), "MaHG", "TenHG");
        //    ViewBag.MaLG = new SelectList(db.LOAIGIAYs.ToList().OrderBy(n => n.TenLoaiGiay), "MaLG", "TenLoaiGiay");
        //    //ktra duong dan file
        //    if (fileUpload == null || fileUpload1 == null || fileUpload2 == null)
        //    {
        //        ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
        //        return View();
        //    }

        //    //the vao csdl
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            //ảnh chính
        //            //luu ten file
        //            var fileName = Path.GetFileName(fileUpload.FileName);
        //            //luu duong dan cua file
        //            var path = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName);


        //            // ảnh phụ 1
        //            var fileName1 = Path.GetFileName(fileUpload1.FileName);
        //            var path1 = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName1);

        //            // ảnh phụ 2
        //            var fileName2 = Path.GetFileName(fileUpload2.FileName);
        //            var path2 = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName2);

        //            //ktra hinh anh da ton tai chua
        //            //ảnh chính
        //            if (System.IO.File.Exists(path))
        //                ViewBag.Thongbao = "Hình ảnh đã tồn tại";
        //            else
        //            {
        //                //luu hinh anh vao duong dan
        //                fileUpload.SaveAs(path);
        //            }
        //            //ảnh phụ 1
        //            if (System.IO.File.Exists(path1))
        //                ViewBag.Thongbao = "Hình ảnh đã tồn tại";
        //            else
        //            {
        //                //luu hinh anh vao duong dan
        //                fileUpload1.SaveAs(path1);
        //            }
        //            // ảnh phụ 2
        //            if (System.IO.File.Exists(path2))
        //                ViewBag.Thongbao = "Hình ảnh đã tồn tại";
        //            else
        //            {
        //                //luu hinh anh vao duong dan
        //                fileUpload2.SaveAs(path2);
        //            }


        //            giay.Anhbia = fileName;
        //            giay.Anhbia1 = fileName1;
        //            giay.Anhbia2 = fileName2;
        //            //luu vao csdl
        //            UpdateModel(giay);
        //            db.SubmitChanges();
        //        }
        //        return RedirectToAction("Giay");
        //    }
        //}

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Admin");
        }

// 2. Them hang giay     
        public ActionResult Hanggiay()
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                return View(db.HANGGIAYs.ToList());
            }
        }

        [HttpGet]
        public ActionResult Themhanggiay()
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
                return View();
        }
        [HttpPost]
        public ActionResult Themhanggiay(HANGGIAY hanggiay)
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                db.HANGGIAYs.InsertOnSubmit(hanggiay);
                db.SubmitChanges();
                return RedirectToAction("Hanggiay", "Admin");
            }
        }
        //XOA HANG GIAY
        [HttpGet]
        public ActionResult Xoahang(int id)
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                HANGGIAY hanggiay = db.HANGGIAYs.SingleOrDefault(n => n.MaHG == id);
                ViewBag.MaHG = hanggiay.MaHG;
                if (hanggiay == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(hanggiay);
            }
        }
        [HttpPost, ActionName("Xoahang")]
        public ActionResult Xacnhanxoahang(int id)
        {
            HANGGIAY hanggiay = db.HANGGIAYs.SingleOrDefault(n => n.MaHG == id);
            ViewBag.MaHG = hanggiay.MaHG;
            if (hanggiay == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.HANGGIAYs.DeleteOnSubmit(hanggiay);
            db.SubmitChanges();
            return RedirectToAction("Hanggiay");
        }
        [HttpGet]
        public ActionResult Suahang(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                HANGGIAY hanggiay = db.HANGGIAYs.SingleOrDefault(n => n.MaHG == id);

                if (hanggiay == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(hanggiay);
            }
        }
        [HttpPost, ActionName("Suahang")]
        public ActionResult Capnhat( int id)
        {
            HANGGIAY hanggiay = db.HANGGIAYs.Where(n => n.MaHG == id).SingleOrDefault();
            UpdateModel(hanggiay); //Cap nhât
            db.SubmitChanges();
            return RedirectToAction( "Hanggiay");
        }
        //=================================LOẠI GIÀY=======================================================
        public ActionResult Loaigiay()
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                return View(db.LOAIGIAYs.ToList());
            }
        }
        [HttpGet]
        public ActionResult Themloaigiay()
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
                return View();
        }
        [HttpPost]
        public ActionResult Themloaigiay(LOAIGIAY loaigiay)
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                db.LOAIGIAYs.InsertOnSubmit(loaigiay);
                db.SubmitChanges();
                return RedirectToAction("Loaigiay", "Admin");
            }
        }

        [HttpGet]
        public ActionResult Xoaloai(int id)
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                LOAIGIAY loaigiay = db.LOAIGIAYs.SingleOrDefault(n => n.MaLG == id);
                ViewBag.MaLG = loaigiay.MaLG;
                if (loaigiay == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(loaigiay);
            }
        }
        [HttpPost, ActionName("Xoaloai")]
        public ActionResult Xacnhanxoaloai(int id)
        {
            LOAIGIAY loaigiay = db.LOAIGIAYs.SingleOrDefault(n => n.MaLG == id);
            ViewBag.MaLG = loaigiay.MaLG;
            if (loaigiay == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.LOAIGIAYs.DeleteOnSubmit(loaigiay);
            db.SubmitChanges();
            return RedirectToAction("Loaigiay");
        }
        [HttpGet]
        public ActionResult Sualoai(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                LOAIGIAY loaigiay = db.LOAIGIAYs.SingleOrDefault(n => n.MaLG == id);

                if (loaigiay == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(loaigiay);
            }
        }
        [HttpPost, ActionName("Sualoai")]
        public ActionResult Capnhatloai(int id)
        {
            LOAIGIAY loaigiay = db.LOAIGIAYs.SingleOrDefault(n => n.MaLG == id);
            UpdateModel(loaigiay); //Cap nhât
            db.SubmitChanges();
            return RedirectToAction("Loaigiay");
        }

        //=============================================================================================
        //3. Khach hang
        public ActionResult Khachhang()
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                return View(db.KHACHHANGs.ToList());
            }
        }
        [HttpGet]
        public ActionResult Xoakhachhang(int id)
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                KHACHHANG khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
                ViewBag.MaKH = khachhang.MaKH;
                if (khachhang == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(khachhang);
            }
        }
        [HttpPost, ActionName("Xoakhachhang")]
        public ActionResult Xacnhanxoakhachhang(int id)
        {
            KHACHHANG khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = khachhang.MaKH;
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.KHACHHANGs.DeleteOnSubmit(khachhang);
            db.SubmitChanges();
            return RedirectToAction("Khachhang");
        }

        public ActionResult Chitietkhachhang(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                KHACHHANG khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
                ViewBag.MaKH = khachhang.MaKH;
                if (khachhang == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(khachhang);
            }

        }
        //=============================================================================================
        //4. Đơn hàng
        //private List<DONDATHANG> Laydonhangmoi(int count) 
        //{
        //    return db.DONDATHANGs.OrderByDescending(a => a.Ngaydat).Take(count).ToList();
        //}
        public ActionResult Donhang(int? page, string Tukhoaa, string currentFilter)
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                var lstProduct = new List<DONDATHANG>();
                if (Tukhoaa != null)
                {
                    page = 1;
                }
                else
                {
                    Tukhoaa = currentFilter;
                }
                if (!string.IsNullOrEmpty(Tukhoaa))
                {
                    //lay ds theo tu khoa tim kiem
                    lstProduct = db.DONDATHANGs.Where(n => n.KHACHHANG.HoTen.Contains(Tukhoaa)).ToList();
                }
                else
                {
                    //lay tat ca sp trong bang GIAY
                    lstProduct = db.DONDATHANGs.ToList();
                }
                ViewBag.CurrentFilter = Tukhoaa;
                int pageNumber = (page ?? 1);
                int pageSize = 5;
                //sap xep sp theo id sp, sp moi dua len dau
                return View(lstProduct.ToPagedList(pageNumber, pageSize));

                ViewBag.PT_ThanhToan = new SelectList(db.Phuongthucs.ToList().OrderBy(n => n.PhuongThucThanhToan), "PT_ThanhToan", "PhuongThucThanhToan");
                ViewBag.Trangthai = new SelectList(db.TrangThais.ToList().OrderBy(n => n.TenTrangthai), "MaTrangthai", "TenTrangthai");

                return View(lstProduct.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult Chitietdonhang(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                //var sach = from s in data.SACHes where s.Masach == id select s;
                var ctdh = from g in db.CHITIETDONTHANGs where g.MaDonHang == id select g;
                return View(ctdh.SingleOrDefault());
            }
        }
        [HttpGet]
        public ActionResult Xoadonhang(int id)
        {
            if (Session["TaikhoanAdmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                DONDATHANG ddh = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
                return View(ddh);
            }
        }
        [HttpPost, ActionName("Xoadonhang")]
        public ActionResult Xacnhanxoadonhang(int id)
        {
            DONDATHANG ddh = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            db.DONDATHANGs.DeleteOnSubmit(ddh);
            db.SubmitChanges();
            return RedirectToAction("Donhang");
        }

        [HttpGet]
        public ActionResult Suadonhang(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                ViewBag.MaHG = new SelectList(db.HANGGIAYs.ToList().OrderBy(n => n.TenHG), "MaHG", "TenHG");
                ViewBag.PT_ThanhToan = new SelectList(db.Phuongthucs.ToList().OrderBy(n => n.PhuongThucThanhToan), "PT_ThanhToan", "PhuongThucThanhToan");

                ViewBag.Trangthai = new SelectList(db.TrangThais.ToList().OrderBy(n => n.TenTrangthai), "MaTrangthai", "TenTrangthai");
                DONDATHANG ddh = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);

                if (ddh == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(ddh);
            }
        }
        [HttpPost, ActionName("Suadonhang")]
        public ActionResult Capnhatdh(int id)
        {
            DONDATHANG ddh = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            UpdateModel(ddh); //Cap nhât
            db.SubmitChanges();
            return RedirectToAction("Donhang");
        }


    }
}
