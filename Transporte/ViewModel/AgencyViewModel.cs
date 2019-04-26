using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Transporte.ViewModel
{
    public class AgencyViewModel
    {
        public int Id { get; set; }
        public string NroAgencia { get; set; }

        public string Nombre { get; set; }

        public string FechaHabilitacion { get; set; }
    }
}