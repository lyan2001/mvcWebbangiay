using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvcWenbangiay.Models;
using System.IO;
using Org.BouncyCastle.Ocsp;
using Common;
using ThanhToanTrucTuyen.Others;

namespace mvcWenbangiay.Controllers
{
    public class GiohangController : Controller
    {
        // GET: Giohang
        dbQLbangiayDataContext data = new dbQLbangiayDataContext();
        //lay gio hang
        public List<Giohang>Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if(lstGiohang==null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        //them vao gio hang
        public ActionResult Themgiohang(int iMagiay,String strURL)
        {
            //lay ra tu ss gio hang
            List<Giohang> lstGiohang = Laygiohang();
            //ktra sp da ton tai trong ss gio hang chua
            Giohang sanpham = lstGiohang.Find(n => n.iMagiay == iMagiay);
            if(sanpham==null)
            {
                sanpham = new Giohang(iMagiay);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }    
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        //tong sl
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if(lstGiohang!=null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
        //tong tien
        private int TongTien()
        {
            int iTongTien = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if(lstGiohang.Count==0)
            {
                return RedirectToAction("Index", "ShoesStore");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        //xoa gio hang
        public ActionResult XoaGiohang(int iMaSP)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMagiay == iMaSP);
            if(sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMagiay == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "ShoesStore");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int iMaSP,FormCollection f)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMagiay == iMaSP);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "ShoesStore");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if(Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "ShoesStore");
            }
            //lay gio hang tu ss
            ViewBag.PT_ThanhToan= new SelectList(data.Phuongthucs.ToList().OrderBy(n => n.PhuongThucThanhToan), "PT_ThanhToan", "PhuongThucThanhToan");
            List<Giohang> lstGiohang = Laygiohang();
            

            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }
        public ActionResult DatHang(DONDATHANG ddh, FormCollection collection)
        {
            //them don hang
           
            //DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            ViewBag.PT_ThanhToan = new SelectList(data.Phuongthucs.ToList().OrderBy(n => n.PhuongThucThanhToan), "PT_ThanhToan", "PhuongThucThanhToan");
            List<Giohang> gh = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            ddh.Trangthai = 1;
            ddh.PT_ThanhToan = 1;
            var ngaygiao = String.Format("{0:dd/MM/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang
            foreach(var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.Magiay = item.iMagiay;
                ctdh.Soluong = item.iSoluong;
                //ctdh.Size = item.iSize;
                ctdh.Dongia = (decimal)item.dDongia;
                data.CHITIETDONTHANGs.InsertOnSubmit(ctdh);

            }
            //send mail cho kh
            var strSanpham = "";
            var thanhtien = decimal.Zero;
            var Tongtien = decimal.Zero;
            foreach (var item in gh)
            {

                strSanpham += "Tên sản phẩm : " + item.sTengiay + "<br />";
                strSanpham +=  "Số lượng :"+ item.iSoluong + "<br />"; ;
                strSanpham += " Nguyên giá : " + String.Format("{0:n}", (decimal)item.dDongia) + "<br />";

                thanhtien += (decimal)item.dDongia * item.iSoluong;
            }
            Tongtien = thanhtien;
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/templateSendmail/send2.html"));
            contentCustomer = contentCustomer.Replace("{{MaDonHang}}",ddh.MaDonHang.ToString());
            contentCustomer = contentCustomer.Replace("{{Ngaydat}}", DateTime.Now.ToString());
            contentCustomer = contentCustomer.Replace("{{TenSP}}", strSanpham);
            contentCustomer = contentCustomer.Replace("{{Tenkhachhang}}", kh.HoTen);
            contentCustomer = contentCustomer.Replace("{{Phone}}", kh.DienthoaiKH);
            contentCustomer = contentCustomer.Replace("{{Email}}", kh.Email);
            contentCustomer = contentCustomer.Replace("{{Address}}", kh.DiachiKH);
            
            contentCustomer = contentCustomer.Replace("{{Tongtien}}", String.Format("{0:n}", Tongtien));
            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            new MailHelper().SendMail(kh.Email, "Đơn hàng mới", contentCustomer);
            new MailHelper().SendMail(toEmail, "Đơn hàng mới", contentCustomer);


            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        public ActionResult Payment()
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
            ViewBag.PT_ThanhToan = new SelectList(data.Phuongthucs.ToList().OrderBy(n => n.PhuongThucThanhToan), "PT_ThanhToan", "PhuongThucThanhToan");
            string totals = (TongTien() * 100).ToString(); //total là tổng của session giỏ hàng

            XuLy pay = new XuLy();

            //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Version", "2.1.0");

            //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_Command", "pay");

            //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_TmnCode", tmnCode);

            //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            //totals là value đã ép kiểu sang kiểu chuỗi ở phía trên
            pay.AddRequestData("vnp_Amount", totals);

            //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_BankCode", "");

            //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_CurrCode", "VND");

            //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_IpAddr", ChuyenDoi.GetIpAddress());

            //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_Locale", "vn");

            //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng");

            //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_OrderType", "other");

            //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);

            //mã hóa đơn
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            return Redirect(paymentUrl);

        }
        public ActionResult XacNhanThanhToan(DONDATHANG ddh, FormCollection collection)
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                XuLy pay = new XuLy();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //mã hóa đơn
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef"));

                //mã giao dịch tại hệ thống VNPAY
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo"));

                //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
                //hash của dữ liệu trả về
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                //check chữ ký đúng hay không?
                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret);

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                        ddh.PT_ThanhToan = 2;
                        ddh.Trangthai = 2;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        ddh.Trangthai = 1;
                    }
                    KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
                    ddh.MaKH = kh.MaKH;
                    ddh.Ngaydat = DateTime.Now;
                    ddh.Trangthai = 1;
                    var ngaygiao = String.Format("{0:dd/MM/yyyy}", collection["Ngaygiao"]);
                    ddh.PT_ThanhToan = 2;
                    ddh.Diachi = kh.DiachiKH;
                    List<Giohang> gh = Laygiohang();
                    ViewBag.TongTien = TongTien();
                    data.DONDATHANGs.InsertOnSubmit(ddh);
                    data.SubmitChanges();
                    foreach (var item in gh)
                    {
                        CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                        ctdh.MaDonHang = ddh.MaDonHang;
                        ctdh.Magiay = item.iMagiay;
                        ctdh.Soluong = item.iSoluong;
                        //ctdh.Size = item.iSize;
                        ctdh.Dongia = (long)(decimal)item.dDongia;
                        data.CHITIETDONTHANGs.InsertOnSubmit(ctdh);
                    }
                    //send mail cho kh
                    var strSanpham = "";
                    var thanhtien = decimal.Zero;
                    var Tongtien = decimal.Zero;
                    foreach (var item in gh)
                    {

                        strSanpham += "Tên sản phẩm : " + item.sTengiay + "<br />";
                        strSanpham += "Số lượng :" + item.iSoluong + "<br />"; ;
                        strSanpham += " Nguyên giá : " + String.Format("{0:n}", (decimal)item.dDongia) + "<br />";

                        thanhtien += (decimal)item.dDongia * item.iSoluong;
                    }
                    Tongtien = thanhtien;
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/templateSendmail/send2.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDonHang}}", ddh.MaDonHang.ToString());
                    contentCustomer = contentCustomer.Replace("{{Ngaydat}}", DateTime.Now.ToString());
                    contentCustomer = contentCustomer.Replace("{{TenSP}}", strSanpham);
                    contentCustomer = contentCustomer.Replace("{{Tenkhachhang}}", kh.HoTen);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", kh.DienthoaiKH);
                    contentCustomer = contentCustomer.Replace("{{Email}}", kh.Email);
                    contentCustomer = contentCustomer.Replace("{{Address}}", kh.DiachiKH);

                    contentCustomer = contentCustomer.Replace("{{Tongtien}}", String.Format("{0:n}", Tongtien));
                    var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                    new MailHelper().SendMail(kh.Email, "Đơn hàng mới", contentCustomer);
                    new MailHelper().SendMail(toEmail, "Đơn hàng mới", contentCustomer);


                    data.SubmitChanges();
                    Session["Giohang"] = null;
                    return RedirectToAction("Xacnhandonhang", "Giohang");
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }
            return View();
        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }

    }
}