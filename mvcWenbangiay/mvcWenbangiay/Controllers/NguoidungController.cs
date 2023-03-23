using mvcWenbangiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvcWenbangiay.Controllers
{
    public class NguoidungController : Controller
    {
        dbQLbangiayDataContext db = new dbQLbangiayDataContext();
        // GET: Nguoidung
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KHACHHANG kh)
        {
            var hoten = collection["HoTen"];
            var tendn = collection["Taikhoan"];
            var matkhau = Mahoa.GetMD5(collection["Matkhau"]); 
             var matkhaunhaplai = Mahoa.GetMD5(collection["Matkhaunhaplai"]);
            var diachi = collection["DiachiKH"];
            var email = collection["Email"];
            var dienthoai = collection["DienthoaiKH"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);
            var check_username = db.KHACHHANGs.FirstOrDefault(n => n.Taikhoan == tendn);
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được bỏ trống";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewData["Loi4"] = "Phải nhập lại mật khẩu";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Phải nhập email";
            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi6"] = "Phải nhập địa chỉ";
            }
            else if(String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi7"] = "Phải nhập sđt";
            }
            else if (check_username != null)
            {
                ViewBag.Warning1 = "Username này đã tồn tại!!";
            }
            else if (matkhau != matkhaunhaplai)
            {
                ViewBag.Warning = "Mật Khẩu Nhập lại không trùng khớp!!";
            }
            else
            {
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = Mahoa.GetMD5(matkhau);
                kh.Email = email;
                kh.DiachiKH = diachi;
                kh.DienthoaiKH = dienthoai;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                db.KHACHHANGs.InsertOnSubmit(kh);
                db.SubmitChanges();
                return RedirectToAction("Dangnhap");
            }
            return this.Dangky();
        }
        [HttpGet]
        public ActionResult Dangnhap()
        {
           return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["Taikhoan"];
            var matkhau = Mahoa.GetMD5(collection["Matkhau"]);
            if(String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if(String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == Mahoa.GetMD5(matkhau));
                if(kh!=null)
                {
                    //ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoan"] = kh;
                    return RedirectToAction("Index", "ShoesStore");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Thongtin()
        {
            KHACHHANG kh = new KHACHHANG();

            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }


            else
            {
                kh = (KHACHHANG)Session["Taikhoan"];


            }
            return View(kh);
        }

        [HttpGet]
        public ActionResult Suathongtin(int id)
        {
            KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = kh.MaKH;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Suathongtin(int id, FormCollection collection)
        {
            //Tạo 1 biến khachhang với đối tương id = id truyền vào
            var khachhang = db.KHACHHANGs.First(n => n.MaKH == id);
            var hoten = collection["HoTen"];
            //var tendn = collection["Taikhoan"];
            //var matkhau = Mahoa.GetMD5(collection["Matkhau"]);
            //var matkhaunhaplai = collection["Matkhaunhaplai"];
            var diachi = collection["DiachiKH"];
            var dienthoai = collection["DienthoaiKH"];
            var email = collection["Email"];
            khachhang.MaKH = id;


            if (string.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Chưa nhập họ tên!";
            }
            if (string.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi2"] = "Chưa nhập số điện thoại!";
            }
            if (string.IsNullOrEmpty(diachi))
            {
                ViewData["Loi3"] = "Chưa nhập địa chỉ!";
            }
            //if (string.IsNullOrEmpty(tendn))
            //{
            //    ViewData["Loi4"] = "Chưa nhập tài khoản!";
            //}
            //if (string.IsNullOrEmpty(matkhau))
            //{
            //    ViewData["Loi4"] = "Chưa nhập PassWord!";
            //}
            else
            {
                khachhang.HoTen = hoten;
                khachhang.DienthoaiKH = dienthoai;
                khachhang.DiachiKH = diachi;
                khachhang.Email = email;
                //khachhang.Taikhoan = tendn;
                //khachhang.Matkhau = Mahoa.GetMD5(matkhau);
                //Update trong CSDL
                UpdateModel(khachhang);
                db.SubmitChanges();
                return RedirectToAction("Dangnhap");
            }
            return this.Suathongtin(id);
        }
        public ActionResult DonHangCaNhan()
        {
            KHACHHANG kh = new KHACHHANG();
            if (Session["Taikhoan"] != null)
                kh = (KHACHHANG)Session["Taikhoan"];
            else
                return RedirectToAction("Dangnhap", "Nguoidung");

            var dsPDP = db.DONDATHANGs.Where(t => t.MaKH == kh.MaKH).ToList();
            return View(dsPDP);
        }
        public ActionResult CTDonHang(int id)
        {

            CHITIETDONTHANG ct = db.CHITIETDONTHANGs.SingleOrDefault(n => n.MaDonHang == id);
            return View(ct);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "ShoesStore");
        }
    }
}