using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Transporte.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public bool IsAdmin { get; set; }

        public int? WindowId { get; set; }
        public virtual Window Window { get; set; }
    }
}