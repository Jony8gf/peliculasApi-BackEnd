using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.ViewModels
{
    public class GeneroCreacionViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(maximumLength: 10)]
        public string Nombre { get; set; }
    }
}
