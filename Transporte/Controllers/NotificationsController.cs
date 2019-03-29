using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Transporte.Helpers;
using Transporte.Models;
using Transporte.ViewModel;


namespace Transporte.Controllers
{
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Notificaciones";

        public string header = "<!DOCTYPE html><html><head><meta name='viewport' content='width=device-width'/><title></title></head><body><div>";
        public string footer = "</div></body></html>";

        // GET: TransportTypes
        public ActionResult Index()
        {            

            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Atributos = db.NotificationTags.Select(c => new { c.Tag, c.NombreAtributo }).OrderBy(x=>x.NombreAtributo).ToList();
            var list = db.Notifications.Select(c => new { c.Id, c.Nombre, c.Descripcion, c.Documento }).ToList();
            List<NotificationViewModel> notificationViewModels = new List<NotificationViewModel>();
            foreach (var item in list)
            {
                NotificationViewModel notification = new NotificationViewModel
                {
                    Descripcion = item.Descripcion,
                    Documento = item.Documento,
                    Id = item.Id,
                    Nombre = item.Nombre
                };

                notificationViewModels.Add(notification);
            }

            return View(notificationViewModels);
        }



        [HttpPost]
        public JsonResult Gets()
        {
            //var list = new List<LicenseClass>();
            try
            {
                var list = db.Notifications.Select(c => new { c.Id,c.Nombre, c.Descripcion, c.Documento }).ToList();
                List<NotificationViewModel> notificationViewModels = new List<NotificationViewModel>();
                foreach (var item in list)
                {
                    NotificationViewModel notification = new NotificationViewModel
                    {
                        Descripcion = item.Descripcion,
                        Documento = item.Documento,                        
                        Id = item.Id,
                        Nombre = item.Nombre
                    };

                    notificationViewModels.Add(notification);
                }

                return Json(notificationViewModels, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;

            }
        }


        [HttpPost]
        public JsonResult Get(int id)
        {
            Notification clase = new Notification();
            try
            {
                clase = db.Notifications.Find(id);

                NotificationViewModel notification = new NotificationViewModel
                {
                    Descripcion = clase.Descripcion,
                    Documento = clase.Documento,
                    Id = clase.Id,
                    Nombre = clase.Nombre
                };
                return Json(notification, JsonRequestBehavior.AllowGet);
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
                var result = from c in db.Notifications
                             where c.Id != id
                             && c.Nombre.ToUpper() == codigo.ToUpper() 
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
        [HttpPost]
        public JsonResult Create(NotificationViewModel clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }




            Notification notification = new Notification
            {
                Id = clase.Id,
                Descripcion = clase.Descripcion,
                Documento = clase.Documento,
                Nombre = clase.Nombre
            };

            notification.Documento = header + clase.Documento + footer;

            db.Notifications.Add(notification);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", clase.Id.ToString(), "Notifications", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        [HttpPost]
        public JsonResult Edit(NotificationViewModel clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });

            }

            Notification notification = db.Notifications.Find(clase.Id);

            notification.Nombre = clase.Nombre;
            notification.Descripcion = clase.Descripcion;
            notification.Documento = clase.Documento;



            db.Entry(notification).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", clase.Id.ToString(), "Notifications", ModuleDescription, WindowDescription);

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

            Notification clase = db.Notifications.Find(id);

            db.Entry(clase).State = EntityState.Deleted;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "Notifications", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


        }



        public FileResult Download(string filePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@filePath);
            string fileName = Path.GetFileName(filePath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        private string SaveFile(HttpPostedFileBase fileUpload,  string folder, string nombre)
        {

            var fileName = Path.GetFileName(fileUpload.FileName);
            var path = Path.Combine(Server.MapPath("~/App_Data/uploads/" + folder + "/"+ nombre + "/"), fileName);
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            file.Directory.Create(); 
            fileUpload.SaveAs(path);
            return path;
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
