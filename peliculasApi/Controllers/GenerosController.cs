using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using peliculasApi.Entidades;
//using peliculasApi.Entidades.Repositorio;
using peliculasApi.Filtros;
using peliculasApi.Utilidades;
using peliculasApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class GenerosController: ControllerBase
    {
        private readonly ILogger<GenerosController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(ILogger<GenerosController> logger, 
            ApplicationDbContext context,
             IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [HttpGet("list")]
        //[ResponseCache(Duration = 60)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[ServiceFilter(typeof(MiFiltroDeAccion))]
        public async Task<ActionResult<List<GeneroViewModel>>> Get([FromQuery] PaginacionViewModel paginacionVM)
        {
            var queryable = context.Generos.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var generos = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionVM).ToListAsync();
            return mapper.Map<List<GeneroViewModel>>(generos);
        }

        [HttpGet("todos")]
        [AllowAnonymous]
        public async Task<ActionResult<List<GeneroViewModel>>> Todos()
        {
            var generos = await context.Generos.OrderBy(x => x.Nombre).ToListAsync();
            return mapper.Map<List<GeneroViewModel>>(generos);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionViewModel generoCreacionVM)
        {
            var genero = mapper.Map<Genero>(generoCreacionVM);
            context.Add(genero);
            var lineasAfectadas = await context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int Id, [FromBody] GeneroCreacionViewModel generoCreacionVM)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == Id);

            if (genero == null)
                return NotFound();

            genero = mapper.Map(generoCreacionVM, genero);
            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == Id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Genero() { Id = Id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        //public async Task<ActionResult<Genero>> Get(int Id, [BindRequired] string nombre)
        //public async Task<ActionResult<Genero>> Get(int Id, [FromHeader] string nombre)

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<GeneroViewModel>> Get(int Id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == Id);

            if (genero == null)
                return NotFound();

            return mapper.Map<GeneroViewModel>(genero);
        }


    }
}
