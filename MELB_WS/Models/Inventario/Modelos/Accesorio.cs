using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MELB_WS.Models.Inventario.Modelos
{
    public class Accesorio
    {
        public int ID_Instrumento { get; set; }
        [Required]
        public int ID_Accesorio { get; set; }
        [StringLength(15, MinimumLength = 1)]
        public string Nombre { get; set; }
        [StringLength(50, MinimumLength = 1)]
        public string Descripcion { get; set; }
    }
}