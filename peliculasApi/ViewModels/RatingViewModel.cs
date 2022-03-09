using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.ViewModels
{
    public class RatingViewModel
    {
        [Range(1, 5)]
        public int Puntuacion { get; set; }
        public int PeliculaId { get; set; }
    }
}
