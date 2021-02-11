using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Transporte.Enumerations;
using Transporte.Helpers;
using Transporte.Models;
using Transporte.ViewModel;
using PersonType = Transporte.Enumerations.PersonType;
using TransportType = Transporte.Models.TransportType;

using Person = Transporte.Models.Person;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using RazorEngine.Templating;
using RazorEngine;

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
            ViewBag.listaNotificaciones = new List<Notification>(db.Notifications.ToList());
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
                    Tuple<string, string> tuple = getNames(item.Persons.Where(x => x.PersonTypeId == (int)PersonType.Titular).ToList());
                    TransportIndexViewModel viewModel = new TransportIndexViewModel
                    {
                        Id = item.Id,
                        Dominio = item.Dominio,
                        Estado = estado,
                        Expediente = item.Expediente,
                        Marca = item.Marca,
                        Modelo = item.Modelo,
                        TransportType = item.TransportType.Descripcion,
                        FechaAlta = item.FechaAlta.ToString("dd/MM/yyyy HH:mm:ss"),
                        NombreTitular = tuple.Item1,
                        DniTitular = tuple.Item2

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

        private Tuple<string, string> getNames(List<Person> titulares)
        {
            string names = null;
            string dnis = null;
            int i = 0;
            foreach (var item in titulares)
            {
                if (i == 0)
                {
                    names = item.Apellido + " " + item.Nombre;
                    dnis = item.Dni;
                }

                else
                {
                    names = names + " / " + item.Apellido + " " + item.Nombre;
                    dnis = dnis + " / " + item.Dni;
                }

                i++;
            }
            return Tuple.Create(names, dnis);
        }




        public JsonResult SearchTransport(string Tipo, string Expediente, string Dominio, string Apellido, string DNI, string Estado, string FechaDesde, string FechaHasta, string Modelo)
        {


            List<TransportIndexViewModel> transports = new List<TransportIndexViewModel>();
            transports = ArmarConsulta(Tipo, Expediente, Dominio, Apellido, DNI, Estado, FechaDesde, FechaHasta, Modelo);

            ViewBag.Editar = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Ver = PermissionViewModel.TienePermisoAlta(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));
            ViewBag.Baja = PermissionViewModel.TienePermisoBaja(WindowHelper.GetWindowId(ModuleDescription, WindowDescription));

            return Json(transports, JsonRequestBehavior.AllowGet);

        }


        private List<TransportIndexViewModel> ArmarConsulta(string Tipo, string Expediente, string Dominio, string Apellido, string DNI, string Estado, string FechaDesde, string FechaHasta, string Modelo)
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
                .Where(x => !string.IsNullOrEmpty(Modelo) ? (x.Modelo == Modelo && x.Modelo != null) : true)
                .Where(x => !string.IsNullOrEmpty(Apellido) ? (x.Persons.Where(y => y.PersonTypeId == TitularId.Value).FirstOrDefault().Apellido == Apellido) : true)
                .Where(x => !string.IsNullOrEmpty(DNI) ? (x.Persons.Where(y => y.PersonTypeId == TitularId.Value).FirstOrDefault().Dni == DNI) : true)
                .Where(x => !string.IsNullOrEmpty(Expediente) ? (x.Expediente == Expediente && x.Expediente != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaDesde) ? (x.FechaAlta >= dtFechaDesde && x.FechaAlta != null) : true)
                .Where(x => !string.IsNullOrEmpty(FechaHasta) ? (x.FechaAlta <= dtFechaHasta && x.FechaAlta != null) : true)
                .Select(c => new { c.Id, c.TransportType, c.Dominio, c.Marca, c.Modelo, c.Expediente, c.VtoPoliza, c.VtoConstanciaAFIP, c.VtoMatafuego, c.VtoVTV, c.Persons, c.FechaAlta }).OrderByDescending(x => x.Id).Take(1000);
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
                    if (String.IsNullOrEmpty(Estado) || Estado == strEstado)
                    {
                        Tuple<string, string> tuple = getNames(item.Persons.Where(x => x.PersonTypeId == (int)PersonType.Titular).ToList());
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
                            FechaAlta = item.FechaAlta.ToString("dd/MM/yyyy HH:mm:ss"),
                            NombreTitular = tuple.Item1,
                            DniTitular = tuple.Item2

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
            int diasDeAviso = db.Settings.Where(x => x.Clave == "DIASANTESDEVENCER").Select(x => x.Numero1).FirstOrDefault() ?? 0;

            //Verifico si tiene algo vencido

            if ((VerificoVencimiento(transport.VtoPoliza, diasDeAviso) == Status.Vencido) || (VerificoVencimiento(transport.VtoVTV, diasDeAviso) == Status.Vencido) ||
                (VerificoVencimiento(transport.VtoMatafuego, diasDeAviso) == Status.Vencido) || (VerificoVencimiento(transport.VtoConstanciaAFIP, diasDeAviso) == Status.Vencido) || (VerificoVencimiento(transport.VtoPagoSeguro, diasDeAviso) == Status.Vencido) ||
                 (VerificoVencimientoPersona(transport.Persons, diasDeAviso) == Status.Vencido))
                return estadoVencido;



            if ((VerificoVencimiento(transport.VtoPoliza, diasDeAviso) == Status.PorVencer) || (VerificoVencimiento(transport.VtoVTV, diasDeAviso) == Status.PorVencer) ||
                (VerificoVencimiento(transport.VtoMatafuego, diasDeAviso) == Status.PorVencer) || (VerificoVencimiento(transport.VtoConstanciaAFIP, diasDeAviso) == Status.PorVencer) || (VerificoVencimiento(transport.VtoPagoSeguro, diasDeAviso) == Status.PorVencer) ||
                 (VerificoVencimientoPersona(transport.Persons, diasDeAviso) == Status.PorVencer))
                return estadoPorVencer;


            return estadoCorrecto;
        }

        private Tuple<string, string> GetFechasVencidas(Transport transport)
        {

            int diasDeAviso = db.Settings.Where(x => x.Clave == "DIASANTESDEVENCER").Select(x => x.Numero1).FirstOrDefault() ?? 0;
            string fechasVencidas = null;
            string fechasPorVencer = null;
            //Verifico si tiene algo vencido

            List<Tuple<string, string>> tuples = new List<Tuple<string, string>>();
            tuples.Add(getDescriptionVto(transport.VtoPoliza, diasDeAviso, "Poliza"));
            tuples.Add(getDescriptionVto(transport.VtoVTV, diasDeAviso, "VTV"));
            tuples.Add(getDescriptionVto(transport.VtoMatafuego, diasDeAviso, "Matafuego"));
            tuples.Add(getDescriptionVto(transport.VtoConstanciaAFIP, diasDeAviso, "Constancia AFIP"));
            tuples.Add(getDescriptionVto(transport.VtoPagoSeguro, diasDeAviso, "Pago de Seguro"));


            // (VerificoVencimientoPersona(transport.Persons, diasDeAviso) == Status.Vencido)
            //return estadoVencido;

            foreach (var item in transport.Persons)
            {
                tuples.Add(getDescriptionVto(item.VtoLibreta, diasDeAviso, "Libreta de " + item.Apellido + " " + item.Nombre));
                tuples.Add(getDescriptionVto(item.VtoLicencia, diasDeAviso, "Licencia de " + item.Apellido + " " + item.Nombre));
            }



            foreach (var item in tuples)
            {
                if (!string.IsNullOrEmpty(item.Item1))
                    fechasVencidas += item.Item1 + "[br]";
                if (!string.IsNullOrEmpty(item.Item2))
                    fechasPorVencer += item.Item2 + "[br]";
            }

            return Tuple.Create(fechasVencidas, fechasPorVencer);
        }

        private Tuple<string, string> getDescriptionVto(DateTime? date, int diasDeAviso, string descripcion)
        {
            string fechasVencidas = null;
            string fechasPorVencer = null;

            switch (VerificoVencimiento(date, diasDeAviso))
            {
                case Status.Vencido:
                    fechasVencidas = descripcion + ": " + date?.ToString("dd/MM/yyyy");
                    break;
                case Status.PorVencer:
                    fechasPorVencer = descripcion + ": " + date?.ToString("dd/MM/yyyy");
                    break;
            };

            return Tuple.Create(fechasVencidas, fechasPorVencer);
        }



        private Status VerificoVencimiento(DateTime? date, int diasDeAviso)
        {

            //DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59     


            if (date?.AddDays(1).AddTicks(-1) < endDateTime)
                return Status.Vencido;
            if (date <= (endDateTime.AddDays(diasDeAviso)))
                return Status.PorVencer;
            else
                return Status.Correcto;


        }



        private Status VerificoVencimientoPersona(ICollection<Person> persons, int diasDeAviso)
        {
            foreach (var item in persons)
            {
                if (VerificoVencimiento(item.VtoLibreta, diasDeAviso) == Status.Vencido || VerificoVencimiento(item.VtoLicencia, diasDeAviso) == Status.Vencido)
                    return Status.Vencido;
            }

            foreach (var item in persons)
            {
                if (VerificoVencimiento(item.VtoLibreta, diasDeAviso) == Status.PorVencer || VerificoVencimiento(item.VtoLicencia, diasDeAviso) == Status.PorVencer)
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
            ViewBag.DiasPorVencer = db.Settings.Where(x => x.Clave == "DIASANTESDEVENCER").Select(x => x.Numero1).FirstOrDefault() ?? 0;
            ViewBag.listaTipos = new List<TransportType>(db.TransportTypes.Where(x => x.Enable == true).ToList());
            ViewBag.listaAgencias = new List<Agency>(db.Agencies.Where(x => x.Enable == true).ToList());
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
                    VtoVTV = transportViewModel.VtoVTV,
                    ParadaNro = transportViewModel.ParadaNro,
                    PlazaNro = transportViewModel.PlazaNro,
                    AgencyId = transportViewModel.AgencyId,
                    SubType = transportViewModel.SubType,
                    VtoPagoSeguro = transportViewModel.VtoPagoSeguro

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
            ViewBag.DiasPorVencer = db.Settings.Where(x => x.Clave == "DIASANTESDEVENCER").Select(x => x.Numero1).FirstOrDefault() ?? 0;
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");


            Transport transport = db.Transports.Find(id);
            if (transport == null)
            {
                return HttpNotFound();
            }

            ViewBag.listaTipos = new List<TransportType>(db.TransportTypes.Where(x => x.Enable == true).ToList());
            ViewBag.listaAgencias = new List<Agency>(db.Agencies.Where(x => x.Enable == true).ToList());

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
                ParadaNro = transport.ParadaNro,
                PlazaNro = transport.PlazaNro,
                AgencyId = transport.AgencyId,
                SubType = transport.SubType,
                VtoPagoSeguro = transport.VtoPagoSeguro
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


        public ActionResult Details(int id)
        {
            ViewBag.DiasPorVencer = db.Settings.Where(x => x.Clave == "DIASANTESDEVENCER").Select(x => x.Numero1).FirstOrDefault() ?? 0;
            if (!PermissionViewModel.TienePermisoAcesso(WindowHelper.GetWindowId(ModuleDescription, WindowDescription)))
                return View("~/Views/Shared/AccessDenied.cshtml");


            Transport transport = db.Transports.Find(id);
            if (transport == null)
            {
                return HttpNotFound();
            }

            ViewBag.listaNotificaciones = new List<Notification>(db.Notifications.ToList());
            ViewBag.listaTipos = new List<TransportType>(db.TransportTypes.Where(x => x.Enable == true).ToList());
            ViewBag.listaAgencias = new List<Agency>(db.Agencies.Where(x => x.Enable == true).ToList());

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
                ParadaNro = transport.ParadaNro,
                PlazaNro = transport.PlazaNro,
                VtoPagoSeguro = transport.VtoPagoSeguro,
                SubType = transport.SubType,
                AgencyId = transport.AgencyId
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


                if (transportViewModel == null || transportViewModel.Id == 0)
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
                transport.ParadaNro = transportViewModel.ParadaNro;
                transport.PlazaNro = transportViewModel.PlazaNro;
                transport.VtoPagoSeguro = transportViewModel.VtoPagoSeguro;
                transport.SubType = transportViewModel.SubType;
                transport.AgencyId = transportViewModel.AgencyId;


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
                        existe = transportViewModel.Celadores == null ? false : transportViewModel.Celadores.Any(x => x.Id == item.Id);

                    //Lo borro de la lista original
                    if (!existe)
                    {
                        transport.Persons.Remove(item);
                        Models.Person person = db.People.Find(item.Id);
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
                                        item.Altura = titular.Altura;
                                        item.Piso = titular.Piso;
                                        item.Depto = titular.Depto;
                                        item.Barrio = titular.Barrio;
                                        item.Dni = titular.Dni;
                                        item.AlturaConstituido = titular.AlturaConstituido;
                                        item.PisoConstituido = titular.PisoConstituido;
                                        item.DeptoConstituido = titular.DeptoConstituido;
                                        item.BarrioConstituido = titular.BarrioConstituido;
                                        item.Dni = titular.Dni;
                                        item.Email = titular.Email;
                                        item.PartidoConstituido = titular.PartidoConstituido;
                                        item.PartidoReal = titular.PartidoReal;
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
                                        item.PartidoConstituido = chofer.PartidoConstituido;
                                        item.PartidoReal = chofer.PartidoReal;
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
                                        item.PartidoConstituido = celadora.PartidoConstituido;
                                        item.PartidoReal = celadora.PartidoReal;
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
            string code = "";

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




        public FileResult ReportArray(int[] TransportId, int NotificacionId)
        {

            List<string> contenidoText = new List<string>();
            Notification notification = db.Notifications.Find(NotificacionId);

            for (int i = 0; i < TransportId.Length; i++)
            {
                contenidoText.AddRange(ArmarHtml(TransportId[i], notification));
            }



            return Export(contenidoText, notification.Nombre);

        }

        public FileResult Report(int TransportId, int NotificacionId)
        {

            List<string> contenidoText = new List<string>();
            Notification notification = db.Notifications.Find(NotificacionId);


            contenidoText = ArmarHtml(TransportId, notification);


            return Export(contenidoText, notification.Nombre);

        }


        private List<string> ArmarHtml(int TransportId, Notification notification)
        {

            Transport transport = db.Transports.Find(TransportId);

            Tuple<string, string> tuple = GetFechasVencidas(transport);

            TransportReportViewModel reportViewModel = new TransportReportViewModel
            {
                Constatacion = transport.Constatacion?.ToString("dd/MM/yyyy"),
                Desinfeccion = transport.Desinfeccion?.ToString("dd/MM/yyyy"),
                Dominio = transport.Dominio,
                Expediente = transport.Expediente,
                FechaInscripcionInicial = transport.FechaInscripcionInicial?.ToString("dd/MM/yyyy"),
                Id = transport.Id,
                Marca = transport.Marca,
                Modelo = transport.Modelo,
                Observaciones = transport.Observaciones,
                ReciboPagoSeguro = transport.ReciboPagoSeguro,
                VtoConstanciaAFIP = transport.VtoConstanciaAFIP?.ToString("dd/MM/yyyy"),
                VtoMatafuego = transport.VtoMatafuego?.ToString("dd/MM/yyyy"),
                VtoPoliza = transport.VtoPoliza?.ToString("dd/MM/yyyy"),
                VtoVTV = transport.VtoVTV?.ToString("dd/MM/yyyy"),
                FechaAlta = transport.FechaAlta.ToString("dd/MM/yyyy"),
                TipoTransporte = transport.TransportType.Descripcion,
                ParadaNro = transport.ParadaNro,
                PlazaNro = transport.PlazaNro,
                NombreAgencia = transport.Agency?.Nombre,
                NroAgencia = transport.Agency?.NroAgencia,
                FechaHabilitacionAgencia = transport.Agency?.FechaHabilitacion?.ToString("dd/MM/yyyy"),
                SubTipoTransporte = transport.SubType,
                VtoPagoSeguro = transport.VtoPagoSeguro?.ToString("dd/MM/yyyy"),
                FechasVencidas = tuple.Item1,
                FechasPorVencer = tuple.Item2
            };

            reportViewModel.Titulares = new List<Person>();
            reportViewModel.Choferes = new List<Person>();
            foreach (var item in transport.Persons.Where(x => x.Enable == true))
            {

                switch (GetPersonTypeCode(item.PersonTypeId))
                {
                    case "TI":
                        {
                            reportViewModel.Titulares.Add(item);// = new List<Person> { item };
                            break;
                        }
                    case "CH":
                        {
                            reportViewModel.Choferes.Add(item);
                            break;
                        }
                    default:
                        break;
                }

            }

            int c = 0;
            foreach (var item in reportViewModel.Choferes)
            {
                if (c == 0)
                {
                    reportViewModel.ChoferNombre = item.Apellido;
                }
                else
                {
                    reportViewModel.ChoferNombre = reportViewModel.ChoferNombre + " y del Sr." + item.Apellido;
                }
                c = c + 1;
            }

            string templateText = notification.Documento;

            List<string> contenidoText = new List<string>();

            foreach (var item in reportViewModel.Titulares)
            {

                reportViewModel.NombreTitular = item.Nombre;
                reportViewModel.ApellidoTitular = item.Apellido;
                reportViewModel.CalleConstituidoTitular = item.CalleConstituido;
                reportViewModel.CalleRealTitular = item.CalleReal;
                reportViewModel.AlturaRealTitular = item.Altura;
                reportViewModel.PisoRealTitular = item.Piso;
                reportViewModel.BarrioRealTitular = item.Barrio;
                reportViewModel.DeptoRealTitular = item.Depto;
                reportViewModel.AlturaConstituidoTitular = item.AlturaConstituido;
                reportViewModel.PisoConstituidoTitular = item.PisoConstituido;
                reportViewModel.BarrioConstituidoTitular = item.BarrioConstituido;
                reportViewModel.DeptoConstituidoTitular = item.DeptoConstituido;
                reportViewModel.TelefonoTitular = item.Telefono;
                reportViewModel.EmailTitular = item.Email;
                reportViewModel.DniTitular = item.Dni;
                reportViewModel.PartidoConstituidoTitular = item.PartidoConstituido;
                reportViewModel.PartidoRealTitular = item.PartidoReal;




                if (Engine.Razor.IsTemplateCached(notification.Id.ToString(), null))
                {
                    contenidoText.Add(Engine.Razor.Run(notification.Id.ToString(), null, reportViewModel));
                }
                else
                {
                    contenidoText.Add(Engine.Razor.RunCompile(templateText, notification.Id.ToString(), null, reportViewModel));
                }
            }

            return contenidoText;
        }



        [HttpPost]
        public FileResult Export(List<string> GridHtml, string name)
        {

            using (MemoryStream stream = new System.IO.MemoryStream())
            {

                Document pdfDoc = new Document(PageSize.A4, 50f, 50f, 100f, 100f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                foreach (var item in GridHtml)
                {
                    string item2 = item.Replace("<br>", "<br/>").Replace("[br]", "<br/>");

                    StringReader sr = new StringReader(item2);
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    pdfDoc.NewPage();
                }

                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", name + ".pdf");
            }
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
