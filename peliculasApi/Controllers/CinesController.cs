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
    public class CinesController : ControllerBase
    {
        private readonly ILogger<CinesController> logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CinesController(ILogger<CinesController> logger,
            ApplicationDbContext context,
             IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CineCreacionViewModel cineCreacionVM)
        {
            var cine = mapper.Map<Cine>(cineCreacionVM);
            context.Add(cine);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        [HttpGet("list")]
        public async Task<ActionResult<List<CineViewModel>>> Get([FromQuery] PaginacionViewModel paginacionVM)
        {
            var queryable = context.Cines.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var cines = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionVM).ToListAsync();
            return mapper.Map<List<CineViewModel>>(cines);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var existe = await context.Cines.AnyAsync(x => x.Id == Id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Cine() { Id = Id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpGet("{Id:int}")]
        public async Task<ActionResult<CineViewModel>> Get(int Id)
        {
            var cine = await context.Cines.FirstOrDefaultAsync(x => x.Id == Id);

            if (cine == null)
                return NotFound();

            return mapper.Map<CineViewModel>(cine);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int Id, [FromBody] CineCreacionViewModel cineCreacionVM)
        {
            var cine = await context.Cines.FirstOrDefaultAsync(x => x.Id == Id);

            if (cine == null)
                return NotFound();

            cine = mapper.Map(cineCreacionVM, cine);
            await context.SaveChangesAsync();
            return NoContent();

        }


    }
}
