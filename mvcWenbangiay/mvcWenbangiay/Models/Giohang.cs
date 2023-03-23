using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvcWenbangiay.Models
{
    public class Giohang
    {
        dbQLbangiayDataContext data = new dbQLbangiayDataContext();
        public int iMagiay { set; get; }
        public string sTengiay { set; get; }
        public string sAnhbia { set; get; }
        public string sHoten { set; get; }
        public int dDongia { set; get; }
        public int iSoluong { set; get; }
        public int iSize { set; get; }
        public int dThanhtien
        {
            get { return iSoluong * dDongia; }
        }
        public Giohang(int Magiay)
        {
            iMagiay = Magiay;
            GIAY giay = data.GIAYs.Single(n => n.Magiay == iMagiay);
            sTengiay = giay.Tengiay;
            sAnhbia = giay.Anhbia;
            iSize = int.Parse(giay.Size.ToString());
            dDongia = int.Parse(giay.Giaban.ToString());
            iSoluong = 1;
        }
    }
}