using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.ViewModels
{
    public class PeliculasPutGetViewModel
    {
        public PeliculaViewModel Pelicula { get; set; }
        public List<GeneroViewModel> GenerosSeleccionados { get; set; }
        public List<GeneroViewModel> GenerosNoSeleccionados { get; set; }
        public List<CineViewModel> CinesNoSeleccionados { get; set; }
        public List<CineViewModel> CinesSeleccionados { get; set; }
        public List<PeliculaActorViewModel> Actores { get; set; }
    }
}
