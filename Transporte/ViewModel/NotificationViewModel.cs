using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transporte.ViewModel
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Requerido")]
        public string Nombre { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Requerido")]
        public string Descripcion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Requerido")]
        public string Documento { get; set; }
        //public bool DocumentoBorrado { get; set; }

        //public string FileName { get; set; }
        //public HttpPostedFileBase File { set; get; }
    }
}