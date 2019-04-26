using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Transporte.Models
{
    public class Agency
    {
        public int Id { get; set; }
        public string NroAgencia { get; set; }

        public string Nombre { get; set; }

        public DateTime? FechaHabilitacion { get; set; }

        public bool Enable { get; set; }
        [NotMapped]
        public string Fullname { get { return string.Format("{0} {1}", NroAgencia, Nombre); } }
    }
}