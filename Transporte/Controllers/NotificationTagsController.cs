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
    public class NotificationTagsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Tags";
        

        public ActionResult Index()
        {
            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            List<ObjectViewModel> objectView = new List<ObjectViewModel>();
            var names = typeof(TransportReportViewModel).GetProperties()
                        .Where(x => x.PropertyType.Name == "String")
                        .Select(property => property.Name)
                        .ToArray();

            foreach (var item in names)
            {
                ObjectViewModel ob = new ObjectViewModel
                {
                    Code = "@Model." + item,
                    Name = item

                };
                objectView.Add(ob);
            }

            ViewBag.listaAtributos = objectView.OrderBy(x=>x.Code).ToList();

            return View(db.NotificationTags.ToList());
        }



        [HttpPost]
        public JsonResult Gets()
        {
            //var list = new List<LicenseClass>();
            try
            {
                var list = db.NotificationTags.Select(c => new { c.Id, c.NombreAtributo, c.Tag }).ToList();

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
            NotificationTag model = new NotificationTag();
            try
            {
                model = db.NotificationTags.Find(id);


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
                var result = from c in db.NotificationTags
                             where c.Id != id
                             && c.Tag.ToUpper() == codigo.ToUpper()
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

        public JsonResult Create(NotificationTag clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }

            db.NotificationTags.Add(clase);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", clase.Id.ToString(), "NotificationTags", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult Edit(NotificationTag clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }
            db.Entry(clase).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", clase.Id.ToString(), "NotificationTags", ModuleDescription, WindowDescription);

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

            NotificationTag clase = db.NotificationTags.Find(id);
            db.Entry(clase).State = EntityState.Deleted;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "NotificationTags", ModuleDescription, WindowDescription);

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
