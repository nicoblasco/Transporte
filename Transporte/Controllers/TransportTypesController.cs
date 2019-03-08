using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Transporte.Helpers;
using Transporte.Models;
using Transporte.ViewModel;
using TransportType = Transporte.Models.TransportType;

namespace Transporte.Controllers
{
    public class TransportTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Tipos de Transporte";

        // GET: TransportTypes
        public ActionResult Index()
        {
            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            return View(db.TransportTypes.ToList());
        }

        

        [HttpPost]
        public JsonResult Gets()
        {
            //var list = new List<LicenseClass>();
            try
            {
                var list = db.TransportTypes.Where(x => x.Enable == true).Select(c => new { c.Id, c.Descripcion }).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        [HttpPost]
        public JsonResult Get(int id)
        {
            TransportType types = new TransportType();
            try
            {
                types = db.TransportTypes.Find(id);


                return Json(types, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        public JsonResult GetDuplicates(int id, string codigo)
        {

            try
            {
                var result = from c in db.TransportTypes
                             where c.Id != id
                             && c.Descripcion.ToUpper() == codigo.ToUpper() && c.Enable == true
                             select c;

                var responseObject = new
                {
                    responseCode = result.Count()
                };

                return Json(responseObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult Create(TransportType clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }

            clase.Enable = true;
            db.TransportTypes.Add(clase);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", clase.Id.ToString(), "TransportTypes", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult Edit(TransportType clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }
            clase.Enable = true;
            db.Entry(clase).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", clase.Id.ToString(), "TransportTypes", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);

        }

        public JsonResult Delete(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            TransportType  clase = db.TransportTypes.Find(id);
            clase.Enable = false;
            db.Entry(clase).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "LicenseClass", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


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
