using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Transporte.ViewModel
{
    public class TransportIndexViewModel
    {
        public int Id { get; set; }
        public string TransportType { get; set; }

        public string FechaAlta { get; set; }

        public string Dominio { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Expediente { get; set; }
      
        public string Estado { get; set; }

        public string NombreTitular { get; set; }
        public string DniTitular { get; set; }
    }
}