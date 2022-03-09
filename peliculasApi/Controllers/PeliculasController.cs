using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class PeliculasController : ControllerBase
    {
        private readonly ILogger<GenerosController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly string contenedor = "peliculas";
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly UserManager<IdentityUser> userManager;

        public PeliculasController(ILogger<GenerosController> logger,
           ApplicationDbContext context,
            IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos,
            UserManager<IdentityUser> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.userManager = userManager;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<LandingPageViewModel>> Get()
        {
            var top = 6;
            var hoy = DateTime.Today;

            var proximosExtrenos = await context.Peliculas
                .Where(x => x.FechaLanzamiento > hoy)
                .OrderBy(x => x.FechaLanzamiento)
                .Take(top)
                .ToListAsync();

            var enCartelera = await context.Peliculas
                .Where(x => x.EnCines)
                .OrderBy(x => x.EnCines)
                .Take(top)
                .ToListAsync();

            var resultado = new LandingPageViewModel();
            resultado.ProximosEstrenos = mapper.Map<List<PeliculaViewModel>>(proximosExtrenos);
            resultado.EnCines = mapper.Map<List<PeliculaViewModel>>(enCartelera);


            return resultado;
        }



        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionViewModel peliculaCreacionVM)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionVM);

            if (peliculaCreacionVM.Poster != null)
            {
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenedor, peliculaCreacionVM.Poster);
            }

            EscribirOdenActores(pelicula);

            context.Add(pelicula);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("PostGet")]
        public async Task<ActionResult<PeliculasPostGetViewModel>> PostGet()
        {
            var cines = await context.Cines.ToListAsync();
            var generos = await context.Generos.ToListAsync();

            var cinesVM = mapper.Map<List<CineViewModel>>(cines);
            var generosVM = mapper.Map<List<GeneroViewModel>>(generos);

            return new PeliculasPostGetViewModel() { Cines = cinesVM, Generos = generosVM };
        }


        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PeliculaViewModel>>> Filtrar([FromQuery] PeliculasFiltrarViewModel peliculasFiltrarVM)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();
            if (!string.IsNullOrEmpty(peliculasFiltrarVM.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(peliculasFiltrarVM.Titulo));
            }

            if (peliculasFiltrarVM.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines );
            }

            if (peliculasFiltrarVM.ProximosEstrenos)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaLanzamiento > DateTime.Today);
            }

            if(peliculasFiltrarVM.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                    .Contains(peliculasFiltrarVM.GeneroId));
            }

            await HttpContext.InsertarParametrosPaginacionEnCabecera(peliculasQueryable);
            var peliculas = await peliculasQueryable.Paginar(peliculasFiltrarVM.PaginacionView).ToListAsync();
            return mapper.Map<List<PeliculaViewModel>>(peliculas);


        }

        [HttpGet("PutGet/{id:int}")]
        public async Task<ActionResult<PeliculasPutGetViewModel>> PutGet(int id)
        {
            var peliculaActionResult = await Get(id);
            if (peliculaActionResult.Result is NotFoundResult) { return NotFound(); }

            var pelicula = peliculaActionResult.Value;

            var generosSeleccionadosIds = pelicula.Generos.Select(x => x.Id).ToList();
            var generosNoSeleccionados = await context.Generos
                .Where(x => !generosSeleccionadosIds.Contains(x.Id))
                .ToListAsync();

            var cinesSeleccionadosIds = pelicula.Cines.Select(x => x.Id).ToList();
            var cinesNoSeleccionados = await context.Cines
                .Where(x => !cinesSeleccionadosIds.Contains(x.Id))
                .ToListAsync();

            var generosNoSeleccionadosVM = mapper.Map<List<GeneroViewModel>>(generosNoSeleccionados);
            var cinesNoSeleccionadosVM = mapper.Map<List<CineViewModel>>(cinesNoSeleccionados);

            var respuesta = new PeliculasPutGetViewModel();
            respuesta.Pelicula = pelicula;
            respuesta.GenerosSeleccionados = pelicula.Generos;
            respuesta.GenerosNoSeleccionados = generosNoSeleccionadosVM;
            respuesta.CinesSeleccionados = pelicula.Cines;
            respuesta.CinesNoSeleccionados = cinesNoSeleccionadosVM;
            respuesta.Actores = pelicula.Actores;

            return respuesta;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionViewModel peliculaCreacionViewModel)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasActores)
                .Include(x => x.PeliculasGeneros)
                .Include(x => x.PeliculasCines)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
                return NotFound();

            pelicula = mapper.Map(peliculaCreacionViewModel, pelicula);

            if(peliculaCreacionViewModel.Poster != null)
            {
                pelicula.Poster = await almacenadorArchivos.EditarArchivo(contenedor, peliculaCreacionViewModel.Poster, pelicula.Poster);
            }

            EscribirOdenActores(pelicula);

            await context.SaveChangesAsync();
            return NoContent();

        }


        [HttpGet("buscarPorNombre/{nombre}")]
        public async Task<ActionResult<List<PeliculaActorViewModel>>> BuscarPorNombre(string nombre = "")
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return new List<PeliculaActorViewModel>();

            return await context.Actores
                .Where(x => x.Nombre.Contains(nombre))
                .OrderBy(x => x.Nombre)
                .Select(x => new PeliculaActorViewModel { Id = x.Id, Nombre = x.Nombre, Foto = x.Foto})
                .Take(5)
                .ToListAsync();
        }


        [HttpGet("{Id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PeliculaViewModel>> Get(int Id)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
                .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                .Include(x => x.PeliculasCines).ThenInclude(x => x.Cine)
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (pelicula == null)
                return NotFound();

            var promedioVoto = 0.0;
            var votoUsuario = 0;

            if (await context.Ratings.AnyAsync(x => x.PeliculaId == Id))
            {
                promedioVoto = await context.Ratings.Where(x => x.PeliculaId == Id).AverageAsync(x => x.Puntuacion);
                
                if(HttpContext.User.Identity.IsAuthenticated)
                {
                    var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
                    var usuario = await userManager.FindByEmailAsync(email);
                    var usuarioId = usuario.Id;

                    var ratingDB = await context.Ratings.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.PeliculaId == Id);

                    if (ratingDB != null)
                        votoUsuario = ratingDB.Puntuacion;

                }
            
            }

            var vm = mapper.Map<PeliculaViewModel>(pelicula);
            vm.Promedio = promedioVoto;
            vm.VotoUsuario = votoUsuario;
            vm.Actores = vm.Actores.OrderBy(x => x.Orden).ToList();
            return vm;
        }

        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {

            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == Id);
            if (pelicula == null)
                return NotFound();

            context.Remove(pelicula);
            await context.SaveChangesAsync();
            await almacenadorArchivos.BorrarArchivo(pelicula.Poster, contenedor);
            return NoContent();

        }


        private void EscribirOdenActores(Pelicula pelicula)
        {
            if(pelicula.PeliculasActores != null)
            {
                for(int i = 0; i <  pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }
    }

    
}
