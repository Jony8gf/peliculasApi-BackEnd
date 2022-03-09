using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.ViewModels
{
    public class LandingPageViewModel
    {
        public List<PeliculaViewModel> EnCines { get; set; }
        public List<PeliculaViewModel> ProximosEstrenos { get; set; }
    }
}
