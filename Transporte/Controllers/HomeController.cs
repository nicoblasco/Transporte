using Transporte.Enumerations;
using Transporte.Models;
using Transporte.Tags;
using Transporte.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Transporte.Controllers
{
    [AutenticadoAttribute]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();

        }

        public ActionResult AccesoDenegado()
        {
            return View("AccesoDenegado");

        }

    }
}