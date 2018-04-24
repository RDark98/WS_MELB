using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MELB_WS.Models.PaNoJoderElResto.Modelos
{
    public class Aula
    {
        [Required]
        public int ID_Aula { get; set; }
        [Required]
        public int Numero { get; set; }
        [Required]
        public int Piso { get; set; }
    }
}