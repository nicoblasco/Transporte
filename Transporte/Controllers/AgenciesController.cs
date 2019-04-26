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
    public class AgenciesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "ABM Maestros";
        public string WindowDescription = "Agencias";


        public ActionResult Index()
        {
            //Verifico los Permisos
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");
            ViewBag.AltaModificacion = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));


            


            return View();
        }



        [HttpPost]
        public JsonResult Gets()
        {
            //var list = new List<LicenseClass>();
            try
            {
                //var list = db.Agencies.Where(x=>x.Enable==true).ToList();
                List<AgencyViewModel> list = new List<AgencyViewModel>();

                foreach (var item in db.Agencies.Where(x => x.Enable == true).ToList())
                {
                    AgencyViewModel agencyViewModel = new AgencyViewModel
                    {
                        Id = item.Id,
                        FechaHabilitacion = item.FechaHabilitacion?.ToString("dd/MM/yyyy"),
                        Nombre = item.Nombre,
                        NroAgencia = item.NroAgencia
                    };

                    list.Add(agencyViewModel);
                }

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
            Agency model = new Agency();
            try
            {
                model = db.Agencies.Find(id);

                AgencyViewModel agencyViewModel = new AgencyViewModel
                {
                    Id = model.Id,
                    FechaHabilitacion = model.FechaHabilitacion?.ToString("dd/MM/yyyy"),
                    Nombre = model.Nombre,
                    NroAgencia = model.NroAgencia
                };



                return Json(agencyViewModel, JsonRequestBehavior.AllowGet);
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
                var result = from c in db.Agencies
                             where c.Id != id
                             && c.NroAgencia.ToUpper() == codigo.ToUpper() && c.Enable==true
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

        public JsonResult Create(Agency clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }

            clase.Enable = true;
            db.Agencies.Add(clase);
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Alta", clase.Id.ToString(), "Agencia", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);
        }


        public JsonResult Edit(Agency clase)
        {
            if (clase == null)
            {
                return Json(new { responseCode = "-10" });
            }


            Agency agency = db.Agencies.Find(clase.Id);

            agency.Nombre = clase.Nombre;
            agency.NroAgencia = clase.NroAgencia;
            agency.FechaHabilitacion = clase.FechaHabilitacion;

            db.Entry(agency).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Modificacion", clase.Id.ToString(), "Agencia", ModuleDescription, WindowDescription);

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

            Agency clase = db.Agencies.Find(id);
            clase.Enable = false;
            db.Entry(clase).State = EntityState.Modified;
            db.SaveChanges();

            //Audito
            AuditHelper.Auditar("Baja", id.ToString(), "Agencia", ModuleDescription, WindowDescription);

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
