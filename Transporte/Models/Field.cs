using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Transporte.Models
{
    public class Field
    {
        public int Id { get; set; }

        public string Referencia { get; set; }
        public string Descripcion { get; set; }

        public bool IsArray { get; set; }
    }
}