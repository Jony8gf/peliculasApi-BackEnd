using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.ViewModels
{
    public class PeliculaViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string Trailer { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Poster { get; set; }
        public List<GeneroViewModel> Generos { get; set; }
        public List<PeliculaActorViewModel> Actores { get; set; }
        public List<CineViewModel> Cines { get; set; }
        public int VotoUsuario { get; set; }
        public double Promedio { get; set; }
    }
}
