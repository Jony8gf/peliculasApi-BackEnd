using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.ViewModels
{
    public class PaginacionViewModel
    {
        public int Pagina { get; set; } = 1;
        public int recordsPorPagina { get; set; } = 10;
        private readonly int cantidadMaximaRecordsPorPagina = 50;

        public int RecordPorPagina
        {
            get
            {
                return recordsPorPagina;
            }
            set
            {
                recordsPorPagina = (value > cantidadMaximaRecordsPorPagina) ? cantidadMaximaRecordsPorPagina : value;
            }
        }
    }
}
