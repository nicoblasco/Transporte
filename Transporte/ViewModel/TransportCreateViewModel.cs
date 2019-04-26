using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Transporte.Models;
using TransportType = Transporte.Models.TransportType;

namespace Transporte.ViewModel
{
    public class TransportCreateViewModel
    {
        public int Id { get; set; }
        public int TransportTypeId { get; set; }
        public virtual TransportType TransportType { get; set; }
        public string SubType { get; set; }
        public string Expediente { get; set; }

        public virtual ICollection<Person> Titulares { get; set; }
        public virtual ICollection<Person> Choferes { get; set; }
        public virtual ICollection<Person> Celadores { get; set; }

        public string Dominio { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public DateTime? FechaInscripcionInicial { get; set; }
        public DateTime? Desinfeccion { get; set; }
        public DateTime? Constatacion { get; set; }

        public string Observaciones { get; set; }
        public DateTime? VtoPoliza { get; set; }
        public string ReciboPagoSeguro { get; set; }

        public DateTime? VtoVTV { get; set; }

        public DateTime? VtoMatafuego { get; set; }
        public DateTime? VtoConstanciaAFIP { get; set; }
        public DateTime? VtoPagoSeguro { get; set; }

        public int? AgencyId { get; set; }
        public virtual Agency Agency { get; set; }

        public DateTime FechaAlta { get; set; }

        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        public string ParadaNro { get; set; }
        public string PlazaNro { get; set; }

    }
}