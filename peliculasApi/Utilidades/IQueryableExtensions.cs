using peliculasApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.Utilidades
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable,
            PaginacionViewModel paginacionVM)
        {
            return queryable
                .Skip((paginacionVM.Pagina - 1) * paginacionVM.RecordPorPagina)
                .Take(paginacionVM.RecordPorPagina);
        }
    }
}
