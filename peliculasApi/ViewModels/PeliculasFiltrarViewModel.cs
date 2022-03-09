using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.ViewModels
{
    public class PeliculasFiltrarViewModel
    {
        public int Pagina { get; set; }
        public int RecordsPorPagina { get; set; }
        public PaginacionViewModel PaginacionView
        {
            get { return new PaginacionViewModel() { Pagina = Pagina, recordsPorPagina = RecordsPorPagina }; }
        }
        public string Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosEstrenos { get; set; }

    }
}
