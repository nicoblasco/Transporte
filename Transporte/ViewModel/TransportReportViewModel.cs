
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transporte.Models;

namespace Transporte.ViewModel
{
    public class TransportReportViewModel
    {
        public int Id { get; set; }
        public string TipoTransporte { get; set; }
        public string Expediente { get; set; }

        public List<Person> Titulares { get; set; }
        public List<Person> Choferes { get; set; }
        public List<Person> Celadores { get; set; }

        public string Dominio { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string FechaInscripcionInicial { get; set; }
        public string Desinfeccion { get; set; }
        public string Constatacion { get; set; }

        public string Observaciones { get; set; }
        public string VtoPoliza { get; set; }
        public string ReciboPagoSeguro { get; set; }

        public string VtoVTV { get; set; }

        public string VtoMatafuego { get; set; }
        public string VtoConstanciaAFIP { get; set; }

        public string FechaAlta { get; set; }

        public string NombreTitular { get; set; }
        public string ApellidoTitular { get; set; }
        public string DniTitular { get; set; }

        public string CalleRealTitular { get; set; }


        public string CalleConstituidoTitular { get; set; }
        public string PartidoConstituidoTitular { get; set; }
        public string PartidoRealTitular { get; set; }

        public string TelefonoTitular { get; set; }
        public string EmailTitular { get; set; }

        public string ParadaNro { get; set; }
        public string PlazaNro { get; set; }
        public string ChoferNombre { get; set; }

        public string VtoPagoSeguro { get; set; }
        public string SubTipoTransporte { get; set; }
        public string NroAgencia { get; set; }

        public string NombreAgencia { get; set; }

        public string FechaHabilitacionAgencia { get; set; }
        public string FechasVencidas { get; set; }
        public string FechasPorVencer { get; set; }

    }
}