using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.ViewModels
{
    public class PeliculasPostGetViewModel
    {
        public List<GeneroViewModel> Generos { get; set; }
        public List<CineViewModel> Cines { get; set; }
    }
}
