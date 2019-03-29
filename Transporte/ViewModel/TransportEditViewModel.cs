using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transporte.Models;

namespace Transporte.ViewModel
{
    public class TransportEditViewModel
    {
        public int Id { get; set; }
        public int TransportTypeId { get; set; }
        public virtual TransportType TransportType { get; set; }
        public string Expediente { get; set; }

        public  List<Person> Titulares { get; set; }
        public List<Person> Choferes { get; set; }
        public List<Person> Celadores { get; set; }

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

        public DateTime FechaAlta { get; set; }

        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        public int NotificationId { get; set; }

        public string ParadaNro { get; set; }
        public string PlazaNro { get; set; }


    }
}