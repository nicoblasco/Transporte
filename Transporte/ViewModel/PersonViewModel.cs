﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Transporte.ViewModel
{
    public class PersonViewModel
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string FechaDeNacimiento { get; set; }

        public string Domicilio { get; set; }
        public int? CalleId { get; set; }

        public string DomicilioNro { get; set; }
        public int? Nacionalidad { get; set; }
        public string Tel_Particular { get; set; }

        public string Tel_Celular { get; set; }

        public string Email { get; set; }
    }
}