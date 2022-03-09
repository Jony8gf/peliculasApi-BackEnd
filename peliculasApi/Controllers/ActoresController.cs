using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using peliculasApi.Entidades;
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
    public class ActoresController: ControllerBase
    {
        private readonly ILogger<GenerosController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly string contenedor = "actores";
        private readonly IAlmacenadorArchivos almacenadorArchivos;

        public ActoresController(ILogger<GenerosController> logger,
           ApplicationDbContext context,
            IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionViewModel actorCreacionVM)
        {
            var actor = mapper.Map<Actor>(actorCreacionVM);

            if (actorCreacionVM.Foto != null)
            {
                actor.Foto = await almacenadorArchivos.GuardarArchivo(contenedor, actorCreacionVM.Foto);
            }

            context.Add(actor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        [HttpGet("list")]
        public async Task<ActionResult<List<ActorViewModel>>> Get([FromQuery] PaginacionViewModel paginacionViewModel)
        {
            var queryable = context.Actores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var actores = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionViewModel).ToListAsync();
            return mapper.Map<List<ActorViewModel>>(actores);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            //var existe = await context.Actores.AnyAsync(x => x.Id == Id);
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == Id);

            if (actor == null)
            {
                return NotFound();
            }

            context.Remove(actor);
            await context.SaveChangesAsync();
            await almacenadorArchivos.BorrarArchivo(actor.Foto, contenedor);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionViewModel actorCreacionVM)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            actor = mapper.Map(actorCreacionVM, actor);

            if (actorCreacionVM.Foto != null)
            {
                actor.Foto = await almacenadorArchivos.EditarArchivo(contenedor, actorCreacionVM.Foto, actor.Foto);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet("{Id:int}")]
        public async Task<ActionResult<ActorViewModel>> Get(int Id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == Id);

            if (actor == null)
                return NotFound();

            return mapper.Map<ActorViewModel>(actor);
        }



    }

}
