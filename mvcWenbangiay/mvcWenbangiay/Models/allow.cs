using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvcWenbangiay.Models
{
    public class allow
    {
        public class PersonModel
        {
            [AllowHtml]
            public string Mota { get; set; }
        }
    }
}