using mvcWenbangiay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;
using PagedList.Mvc;

namespace mvcWenbangiay.Controllers
{
    public class ShoesStoreController : Controller
    {
        // GET: ShoesStore

        dbQLbangiayDataContext data = new dbQLbangiayDataContext();
        private List<GIAY> Laygiaymoi(int count)
        {
            return data.GIAYs.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        public ActionResult Index(int ? page, string Tukhoa, string currentFilter)
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
            if (!string.IsNullOrEmpty(Tukhoa))
            {
                //lay ds theo tu khoa tim kiem
                lstProduct = data.GIAYs.Where(n => n.Tengiay.Contains(Tukhoa)).ToList();
            }
            else
            {
                //lay tat ca sp trong bang GIAY
                lstProduct = data.GIAYs.ToList();
            }
            ViewBag.CurrentFilter = Tukhoa;
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            //sap xep sp theo id sp, sp moi dua len dau
            return View(lstProduct.ToPagedList(pageNumber, pageSize));



            ////tao  biến số sp trên mỗi trang
            //int pageSize = 10;
            ////tao bien so trang
            //int pageNum = (page ?? 1);

            ////Lay top 10 sp ban chay nhat

            //var giaymoi = Laygiaymoi(20);
            //return View(giaymoi.ToPagedList(pageNum,pageSize));
        }
        public ActionResult Loaigiay()
        {
            var loaigiay = from lg in data.LOAIGIAYs select lg;
            return PartialView(loaigiay);
        }
        public ActionResult Hanggiay()
        {
            var hanggiay = from hg in data.HANGGIAYs select hg;
            return PartialView(hanggiay);
        }
        public ActionResult SPTheoHanggiay(int id)
        {

            var giay = from g in data.GIAYs where g.MaHG == id select g;
            return View(giay);
        }
        public ActionResult SPTheoLoaigiay(int id)
        {
            var giay = from g in data.GIAYs where g.MaLG == id select g;
            return View(giay);
        }
        public ActionResult Details(int id)
        {
            var giay = from g in data.GIAYs
                       where g.Magiay == id
                       select g;
            return View(giay.Single());
        }
        public ActionResult Lienhe()
        {
            return View();
        }

    }
}