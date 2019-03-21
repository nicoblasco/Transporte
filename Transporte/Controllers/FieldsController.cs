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

namespace Transporte.Controllers
{
    public class FieldsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Configuración";
        public string WindowDescription = "Atributos";
        

        public ActionResult Index()
        {
            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));

            return View(db.Fields.ToList());
        }



        [HttpPost]
        public JsonResult Gets()
        {
            //var list = new List<LicenseClass>();
            try
            {
                var list = db.Fields.Select(c => new { c.Id, c.Descripcion, c.Referencia, c.IsArray }).ToList();

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
            Field model = new Field();
            try
            {
                model = db.Fields.Find(id);


                return Json(model, JsonRequestBehavior.AllowGet);
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
                var result = from c in db.Fields
                             where c.Id != id
                             && c.Referencia.ToUpper() == codigo.ToUpper()
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

        public JsonResult Create(Field clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.Fields.Add(clase);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", clase.Id.ToString(), "Fields", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult Edit(Field clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }
            db.Entry(clase).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", clase.Id.ToString(), "Fields", ModuleDescription, WindowDescription);

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

            Field clase = db.Fields.Find(id);
            db.Entry(clase).State = EntityState.Deleted;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "Fields", ModuleDescription, WindowDescription);

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
