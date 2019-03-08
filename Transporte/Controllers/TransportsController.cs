using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Transporte.Enumerations;
using Transporte.Helpers;
using Transporte.Models;
using Transporte.ViewModel;
using TransportType = Transporte.Models.TransportType;

namespace Transporte.Controllers
{
    public class TransportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string ModuleDescription = "Menu Principal";
        public string WindowDescription = "Transportes";

        // GET: Transports
        public ActionResult Index()
        {

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Turnos", "Editar"));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId("Turnos", "Ver"));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId("Turnos", "Baja"));
            return View();
        }

        [HttpPost]
        public JsonResult GetsSearch()
        {
            try
            {

                List<TransportIndexViewModel> viewModels = new List<TransportIndexViewModel>();
                List<Transport> tabla = db.Transports.Take(1000).Where(x => x.Enable == true).ToList();

                foreach (var item in tabla)
                {
                    string estado = GetEstado(item);
                    TransportIndexViewModel viewModel = new TransportIndexViewModel
                    {
                        Id = item.Id,
                        Dominio = item.Dominio,
                        Estado = estado,
                        Expediente = item.Expediente,
                        Marca = item.Marca,
                        Modelo = item.Modelo,
                        TransportType = item.TransportType.Descripcion
                    };

                    viewModels.Add(viewModel);
                }

                return Json(viewModels.OrderByDescending(x => x.Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private string GetEstado(Transport transport)
        {
            string estadoCorrecto = "CORRECTO";
            string estadoPorVencer = "POR VENCER";
            string estadoVencido = "VENCIDO";

            //Verifico si tiene algo vencido

            if ((VerificoVencimiento(transport.VtoPoliza) == Status.Vencido) || (VerificoVencimiento(transport.VtoVTV) == Status.Vencido) ||
                (VerificoVencimiento(transport.VtoMatafuego) == Status.Vencido) || (VerificoVencimiento(transport.VtoConstanciaAFIP) == Status.Vencido) ||
                 (VerificoVencimientoPersona(transport.Persons) == Status.Vencido))
                return estadoVencido;



            if ((VerificoVencimiento(transport.VtoPoliza) == Status.PorVencer) || (VerificoVencimiento(transport.VtoVTV) == Status.PorVencer) ||
                (VerificoVencimiento(transport.VtoMatafuego) == Status.PorVencer) || (VerificoVencimiento(transport.VtoConstanciaAFIP) == Status.PorVencer) ||
                 (VerificoVencimientoPersona(transport.Persons) == Status.PorVencer))
                return estadoPorVencer;

           
            return estadoCorrecto;
        }
        private Status VerificoVencimiento(DateTime? date)
        {

            //DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59     
            int diasDeAviso = db.Settings.Where(x => x.Clave == "DIASANTESDEVENCER").Select(x => x.Numero1).FirstOrDefault() ?? 0;

            if (date > endDateTime)
                return Status.Vencido;
            if ((date.Value.AddDays(diasDeAviso)) > endDateTime)
                return Status.PorVencer;
            else
                return Status.Correcto;


        }

        private Status VerificoVencimientoPersona(ICollection<Person> persons)
        {
            foreach (var item in persons)
            {
                if (VerificoVencimiento(item.VtoLibreta) == Status.Vencido || VerificoVencimiento(item.VtoLicencia) == Status.Vencido)
                    return Status.Vencido;
            }

            foreach (var item in persons)
            {
                if (VerificoVencimiento(item.VtoLibreta) == Status.PorVencer || VerificoVencimiento(item.VtoLicencia) == Status.PorVencer)
                    return Status.PorVencer;
            }

            return Status.Correcto;
        }



        [HttpPost]
        public JsonResult GetSearchBy(string Codigo)
        {
            try
            {

                List<TransportIndexViewModel> viewModels = new List<TransportIndexViewModel>();
                List<Transport> models = db.Transports.Where(x => x.Enable == true && x.Dominio == Codigo).ToList();

                foreach (var item in models)
                {
                    string estado = GetEstado(item);
                    TransportIndexViewModel viewModel = new TransportIndexViewModel
                    {
                        Id = item.Id,
                        Dominio = item.Dominio,
                        Estado = estado,
                        Expediente = item.Expediente,
                        Marca = item.Marca,
                        Modelo = item.Modelo,
                        TransportType = item.TransportType.Descripcion
                    };

                    viewModels.Add(viewModel);
                }

                return Json(viewModels.OrderByDescending(x => x.Id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                throw;
            }
        }


        public JsonResult Delete(int id)
        {
            if (id == 0)
            {
                return Json(new { responseCode = "-10" });
            }

            Transport model = db.Transports.Find(id);
            model.Enable = false;
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();

            AuditHelper.Auditar("Baja", "Id -" + model.Id.ToString() + " / Dominio -" + model.Dominio, "Transports", ModuleDescription, WindowDescription);

            var responseObject = new
            {
                responseCode = 0
            };

            return Json(responseObject);


        }


        public ActionResult Create()
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");

            ViewBag.listaTipos = new List<TransportType>(db.TransportTypes.Where(x=>x.Enable==true).ToList());
            return View();
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
