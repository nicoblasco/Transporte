using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Transporte.Models
{
    public class Person
    {
        public int Id { get; set; }

        public int TransportId { get; set; }
        public Transport Transport { get; set; }
        public int PersonTypeId { get; set; }
        public PersonType PersonType { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }

        public string CalleReal { get; set; }
        public string Altura { get; set; }
        public string Piso { get; set; }
        public string Depto { get; set; }

        public string Barrio { get; set; }

        public string PartidoReal{ get; set; }


        public string CalleConstituido { get; set; }
        public string AlturaConstituido { get; set; }
        public string PisoConstituido { get; set; }
        public string DeptoConstituido { get; set; }

        public string BarrioConstituido { get; set; }

        public string PartidoConstituido { get; set; }

        public string Telefono { get; set; }
        public string Email { get; set; }       

        public DateTime? VtoLicencia { get; set; }
        public DateTime? VtoLibreta { get; set; }

        public bool Enable { get; set; }




    }
}