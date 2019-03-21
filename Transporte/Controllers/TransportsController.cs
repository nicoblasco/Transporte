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
using PersonType = Transporte.Enumerations.PersonType;
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

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));

            ViewBag.listaTipos = new List<TransportType>(db.TransportTypes.Where(x => x.Enable == true).ToList());

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
                        TransportType = item.TransportType.Descripcion,
                        FechaAlta = item.FechaAlta.ToString("dd/MM/yyyy HH:mm:ss")
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


        public JsonResult SearchTransport(string Tipo, string Expediente, string Dominio, string Apellido, string DNI, string Estado, string FechaDesde, string FechaHasta)
        {


            List<TransportIndexViewModel> transports = new List<TransportIndexViewModel>();
            transports = ArmarConsulta(Tipo, Expediente, Dominio, Apellido, DNI, Estado, FechaDesde, FechaHasta);

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));

            return Json(transports, JsonRequestBehavior.AllowGet);

        }


        private List<TransportIndexViewModel> ArmarConsulta(string Tipo, string Expediente, string Dominio, string Apellido, string DNI, string Estado, string FechaDesde, string FechaHasta)
        {
            int TipoId = 0;

            int? TitularId = GetPersonIdByCode("TI");

            if (!String.IsNullOrEmpty(Tipo))
                TipoId = Convert.ToInt32(Tipo);




            List<TransportIndexViewModel> transports = new List<TransportIndexViewModel>();
            DateTime? dtFechaDesde = null;
            DateTime? dtFechaHasta = null;

            if (!String.IsNullOrEmpty(FechaDesde))
                dtFechaDesde = Convert.ToDateTime(FechaDesde);

            if (!String.IsNullOrEmpty(FechaHasta))
                dtFechaHasta = Convert.ToDateTime(FechaHasta).AddDays(1).AddTicks(-1);


            try
            {
           
                    var lista = db.Transports
                    .Where(x => x.Enable == true)
                    .Where(x => !string.IsNullOrEmpty(Tipo) ? (x.TransportType.Id == TipoId && x.TransportType.Descripcion != null) : true)
                    .Where(x => !string.IsNullOrEmpty(Expediente) ? (x.Expediente == Expediente && x.Expediente != null) : true)
                    .Where(x => !string.IsNullOrEmpty(Dominio) ? (x.Dominio == Dominio && x.Dominio != null) : true)
                    .Where(x => !string.IsNullOrEmpty(Apellido) ? (x.Persons.Where(y=>y.PersonTypeId==TitularId.Value).FirstOrDefault().Apellido == Apellido) : true)
                    .Where(x => !string.IsNullOrEmpty(DNI) ? (x.Persons.Where(y => y.PersonTypeId == TitularId.Value).FirstOrDefault().Dni == DNI) : true)
                    .Where(x => !string.IsNullOrEmpty(Expediente) ? (x.Expediente == Expediente && x.Expediente != null) : true)
                    .Where(x => !string.IsNullOrEmpty(FechaDesde) ? (x.FechaAlta >= dtFechaDesde && x.FechaAlta != null) : true)
                    .Where(x => !string.IsNullOrEmpty(FechaHasta) ? (x.FechaAlta <= dtFechaHasta && x.FechaAlta != null) : true)
                    .Select(c => new { c.Id, c.TransportType, c.Dominio, c.Marca, c.Modelo, c.Expediente, c.VtoPoliza, c.VtoConstanciaAFIP, c.VtoMatafuego, c.VtoVTV, c.Persons, c.FechaAlta  }).OrderByDescending(x => x.Id).Take(1000);
                    ;

                foreach (var item in lista)
                {
                    Transport t = new Transport
                    {
                        Id = item.Id,
                        VtoConstanciaAFIP = item.VtoConstanciaAFIP,
                        VtoMatafuego = item.VtoMatafuego,
                        VtoPoliza = item.VtoPoliza,
                        VtoVTV = item.VtoVTV,
                        Persons = item.Persons
                    };


                    string strEstado = GetEstado(t);
                    if (String.IsNullOrEmpty(Estado) || Estado==strEstado)
                    {
                        //Si filtro por estado, solo agrega el que coincide
                        //Si no filtro por estado que agregue todo
                        TransportIndexViewModel transport = new TransportIndexViewModel
                        {
                            Id = item.Id,
                            Dominio = item.Dominio,
                            Expediente = item.Expediente,
                            Marca = item.Marca,
                            Estado = strEstado,
                            Modelo = item.Modelo,
                            TransportType = item.TransportType.Descripcion,
                            FechaAlta = item.FechaAlta.ToString("dd/MM/yyyy HH:mm:ss")
                            
                        };

                        transports.Add(transport);
                    }


                }

            }
            catch (Exception e)
            {

                throw;
            }

            return transports;
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

            if (date < endDateTime)
                return Status.Vencido;
            if (date <= (endDateTime.AddDays(diasDeAviso)))
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

        public JsonResult CreateTransport(TransportCreateViewModel transportViewModel)
        {

            try
            {


                if (transportViewModel == null)
                {
                    return Json(new { responseCode = "-10" });
                }


                Transport transport = new Transport
                {
                    Constatacion = transportViewModel.Constatacion,
                    Desinfeccion = transportViewModel.Desinfeccion,
                    Dominio = transportViewModel.Dominio,
                    Enable = true,
                    Expediente = transportViewModel.Expediente,
                    FechaAlta = DateTime.Now,
                    FechaInscripcionInicial = transportViewModel.FechaInscripcionInicial,
                    Marca = transportViewModel.Marca,
                    Modelo = transportViewModel.Modelo,
                    Observaciones = transportViewModel.Observaciones,
                    ReciboPagoSeguro = transportViewModel.ReciboPagoSeguro,
                    TransportTypeId = transportViewModel.TransportTypeId,
                    UsuarioId = SessionHelper.GetUser(),
                    VtoConstanciaAFIP = transportViewModel.VtoConstanciaAFIP,
                    VtoMatafuego = transportViewModel.VtoMatafuego,
                    VtoPoliza = transportViewModel.VtoPoliza,
                    VtoVTV = transportViewModel.VtoVTV

                };



                //Ahora Cargo los titulares, choferes y celadoras
                transport.Persons = new List<Person>();

                foreach (var titular in transportViewModel.Titulares)
                {
                    if (!String.IsNullOrEmpty(titular.Dni))
                    {
                        titular.PersonTypeId = GetPersonTypeId(PersonType.Titular);
                        titular.Enable = true;
                        transport.Persons.Add(titular);
                    }
                }

                foreach (var chofer in transportViewModel.Choferes)
                {
                    if (!String.IsNullOrEmpty(chofer.Dni))
                    {
                        chofer.PersonTypeId = GetPersonTypeId(PersonType.Chofer);
                        chofer.Enable = true;
                        transport.Persons.Add(chofer);
                        
                    }
                }

                foreach (var celadora in transportViewModel.Celadores)
                {
                    if (!String.IsNullOrEmpty(celadora.Dni))
                    {
                        celadora.PersonTypeId = GetPersonTypeId(PersonType.Celadora);
                        celadora.Enable = true;
                        transport.Persons.Add(celadora);
                        }
                }

                db.Transports.Add(transport);
                db.SaveChanges();


                var responseObject = new
                {
                    responseCode = 0
                };

                return Json(responseObject);
                }
            catch (Exception)
            {

                return Json(new { responseCode = "-10" });
            }
        }


        public ActionResult Edit(int id)
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");


            Transport transport = db.Transports.Find(id);
            if (transport == null)
            {
                return HttpNotFound();
            }

            ViewBag.listaTipos = new List<TransportType>(db.TransportTypes.Where(x => x.Enable == true).ToList());

            TransportEditViewModel editViewModel = new TransportEditViewModel
            {
                Constatacion = transport.Constatacion,
                Desinfeccion = transport.Desinfeccion,
                Dominio = transport.Dominio,
                Expediente = transport.Expediente,
                FechaInscripcionInicial = transport.FechaInscripcionInicial,
                Id= transport.Id,
                Marca = transport.Marca,
                Modelo = transport.Modelo,
                Observaciones = transport.Observaciones,
                ReciboPagoSeguro = transport.ReciboPagoSeguro,
                TransportTypeId = transport.TransportTypeId,
                VtoConstanciaAFIP = transport.VtoConstanciaAFIP,
                VtoMatafuego = transport.VtoMatafuego,
                VtoPoliza = transport.VtoPoliza,
                VtoVTV = transport.VtoVTV,
            };

            editViewModel.Titulares = new List<Person>();
            editViewModel.Choferes = new List<Person>();
            editViewModel.Celadores = new List<Person>();
            foreach (var item in transport.Persons.Where(x=>x.Enable==true))
            {

                switch (GetPersonTypeCode(item.PersonTypeId))
                {
                    case "TI":
                        {
                            editViewModel.Titulares.Add(item);// = new List<Person> { item };
                            break;
                        }
                    case "CH":
                        {
                            editViewModel.Choferes.Add(item);
                            break;
                        }
                    case "CE":
                        {
                            editViewModel.Celadores.Add(item);
                            break;
                        }
                    default:
                        break;
                }

            }


            return View(editViewModel);
        }


        public ActionResult Details(int id)
        {
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");


            Transport transport = db.Transports.Find(id);
            if (transport == null)
            {
                return HttpNotFound();
            }

            ViewBag.listaTipos = new List<TransportType>(db.TransportTypes.Where(x => x.Enable == true).ToList());

            TransportEditViewModel editViewModel = new TransportEditViewModel
            {
                Constatacion = transport.Constatacion,
                Desinfeccion = transport.Desinfeccion,
                Dominio = transport.Dominio,
                Expediente = transport.Expediente,
                FechaInscripcionInicial = transport.FechaInscripcionInicial,
                Id = transport.Id,
                Marca = transport.Marca,
                Modelo = transport.Modelo,
                Observaciones = transport.Observaciones,
                ReciboPagoSeguro = transport.ReciboPagoSeguro,
                TransportTypeId = transport.TransportTypeId,
                VtoConstanciaAFIP = transport.VtoConstanciaAFIP,
                VtoMatafuego = transport.VtoMatafuego,
                VtoPoliza = transport.VtoPoliza,
                VtoVTV = transport.VtoVTV,
            };

            editViewModel.Titulares = new List<Person>();
            editViewModel.Choferes = new List<Person>();
            editViewModel.Celadores = new List<Person>();
            foreach (var item in transport.Persons.Where(x => x.Enable == true))
            {

                switch (GetPersonTypeCode(item.PersonTypeId))
                {
                    case "TI":
                        {
                            editViewModel.Titulares.Add(item);// = new List<Person> { item };
                            break;
                        }
                    case "CH":
                        {
                            editViewModel.Choferes.Add(item);
                            break;
                        }
                    case "CE":
                        {
                            editViewModel.Celadores.Add(item);
                            break;
                        }
                    default:
                        break;
                }

            }


            return View(editViewModel);
        }

        public JsonResult EditTransport(TransportEditViewModel transportViewModel)
        {

            try
            {


                if (transportViewModel == null || transportViewModel.Id ==0)
                {
                    return Json(new { responseCode = "-10" });
                }

                Transport transport = db.Transports.Find(transportViewModel.Id);

                transport.Constatacion = transportViewModel.Constatacion;
                transport.Desinfeccion = transportViewModel.Desinfeccion;
                transport.Dominio = transportViewModel.Dominio;
                transport.Expediente = transportViewModel.Expediente;
                transport.FechaInscripcionInicial = transportViewModel.FechaInscripcionInicial;
                transport.Marca = transportViewModel.Marca;
                transport.Modelo = transportViewModel.Modelo;
                transport.Observaciones = transportViewModel.Observaciones;
                transport.ReciboPagoSeguro = transportViewModel.ReciboPagoSeguro;
                transport.TransportTypeId = transportViewModel.TransportTypeId;
                transport.VtoConstanciaAFIP = transportViewModel.VtoConstanciaAFIP;
                transport.VtoMatafuego = transportViewModel.VtoMatafuego;
                transport.VtoPoliza = transportViewModel.VtoPoliza;
                transport.VtoVTV = transportViewModel.VtoVTV;


                //Borro los que ya no estan
                    //Creo una lista auxiliar
                transport.Persons = new List<Person>();
                List<Person> auxPerson = new List<Person>();
                auxPerson.AddRange(transport.Persons);

                foreach (var item in auxPerson)
                {
                    //Me fijo si vino en cada una de las lista, sino vino es porque se borro
                    bool existe;
                    existe = transportViewModel.Titulares == null ? false : transportViewModel.Titulares.Any(x => x.Id == item.Id);
                    if (!existe)
                        existe = transportViewModel.Choferes == null ? false : transportViewModel.Choferes.Any(x => x.Id == item.Id);
                    if (!existe)
                        existe = transportViewModel.Celadores==null?false:transportViewModel.Celadores.Any(x => x.Id == item.Id);

                    //Lo borro de la lista original
                    if (!existe)
                    {
                        transport.Persons.Remove(item);
                        Person person = db.People.Find(item.Id);
                        db.People.Remove(person);
                    }
                        
                }


                //Ahora Cargo o modifico los titulares, choferes y celadoras
                if (transportViewModel.Titulares != null)
                {
                    foreach (var titular in transportViewModel.Titulares)
                    {
                        if (titular.Id > 0)
                        {
                            //Si ya existia
                            if (titular.Enable)
                            {
                                //Se modifica
                                foreach (var item in transport.Persons)
                                {
                                    if (item.Id == titular.Id)
                                    {
                                        item.Nombre = titular.Nombre;
                                        item.Telefono = titular.Telefono;
                                        item.VtoLibreta = titular.VtoLibreta;
                                        item.VtoLicencia = titular.VtoLicencia;
                                        item.Apellido = titular.Apellido;
                                        item.CalleConstituido = titular.CalleConstituido;
                                        item.CalleReal = titular.CalleReal;
                                        item.Dni = titular.Dni;
                                        item.Email = titular.Email;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Se agrega
                            if (!String.IsNullOrEmpty(titular.Dni))
                            {
                                titular.PersonTypeId = GetPersonTypeId(PersonType.Titular);
                                titular.Enable = true;
                                transport.Persons.Add(titular);
                            }
                        }
                    }
                }
                
                if (transportViewModel.Choferes != null)
                {
                    foreach (var chofer in transportViewModel.Choferes)
                    {
                        if (chofer.Id > 0)
                        {
                            //Si ya existia
                            if (chofer.Enable)
                            {
                                //Se modifica
                                foreach (var item in transport.Persons)
                                {
                                    if (item.Id == chofer.Id)
                                    {
                                        item.Nombre = chofer.Nombre;
                                        item.Telefono = chofer.Telefono;
                                        item.VtoLibreta = chofer.VtoLibreta;
                                        item.VtoLicencia = chofer.VtoLicencia;
                                        item.Apellido = chofer.Apellido;
                                        item.CalleConstituido = chofer.CalleConstituido;
                                        item.CalleReal = chofer.CalleReal;
                                        item.Dni = chofer.Dni;
                                        item.Email = chofer.Email;
                                    }
                                }
                            }
                            //else
                            //{
                            //    //Se borra
                            //    transport.Persons.Remove(chofer);
                            //}
                        }
                        else
                        {
                            //Se agrega
                            if (!String.IsNullOrEmpty(chofer.Dni))
                            {
                                chofer.PersonTypeId = GetPersonTypeId(PersonType.Chofer);
                                chofer.Enable = true;
                                transport.Persons.Add(chofer);
                            }
                        }
                    }
                }


                if (transportViewModel.Celadores != null)
                {
                    foreach (var celadora in transportViewModel.Celadores)
                    {
                        if (celadora.Id > 0)
                        {
                            //Si ya existia
                            if (celadora.Enable)
                            {
                                //Se modifica
                                foreach (var item in transport.Persons)
                                {
                                    if (item.Id == celadora.Id)
                                    {
                                        item.Nombre = celadora.Nombre;
                                        item.Telefono = celadora.Telefono;
                                        item.VtoLibreta = celadora.VtoLibreta;
                                        item.VtoLicencia = celadora.VtoLicencia;
                                        item.Apellido = celadora.Apellido;
                                        item.CalleConstituido = celadora.CalleConstituido;
                                        item.CalleReal = celadora.CalleReal;
                                        item.Dni = celadora.Dni;
                                        item.Email = celadora.Email;
                                    }
                                }
                            }
                            //else
                            //{
                            //    //Se borra
                            //    transport.Persons.Remove(celadora);
                            //}
                        }
                        else
                        {
                            //Se agrega
                            if (!String.IsNullOrEmpty(celadora.Dni))
                            {
                                celadora.PersonTypeId = GetPersonTypeId(PersonType.Celadora);
                                celadora.Enable = true;
                                transport.Persons.Add(celadora);
                            }
                        }
                    }
                }

                db.Entry(transport).State = EntityState.Modified;
                db.SaveChanges();


                var responseObject = new
                {
                    responseCode = 0
                };

                return Json(responseObject);
            }
            catch (Exception e)
            {

                return Json(new { responseCode = "-10" });
            }
        }

        private int GetPersonTypeId(PersonType personType)
        {
            string code="";

            switch (personType)
            {
                case PersonType.Titular:
                    {
                        code = "TI";
                        break;
                    }
                case PersonType.Chofer:
                    {
                        code = "CH";
                        break;
                    }
                case PersonType.Celadora:
                    {
                        code = "CE";
                        break;
                    }

                default: break;
            }


            return db.PersonTypes.Where(x => x.Code == code).Select(x => x.Id).FirstOrDefault();
        }

        private int? GetPersonIdByCode(string code)
        {
            return db.PersonTypes.Where(x => x.Code == code).Select(x => x.Id).FirstOrDefault();
        }

        private string GetPersonTypeCode(int Id)
        {
            return db.PersonTypes.Find(Id).Code;
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
