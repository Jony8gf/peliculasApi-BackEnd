using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using peliculasApi.Entidades;
using peliculasApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.Controllers
{
    [Route("api/ratings")]
    [ApiController]
    public class RatingsController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext context;

        public RatingsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] RatingViewModel ratingVM)
        {
            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var ratingActual = await context.Ratings
                .FirstOrDefaultAsync(x => x.PeliculaId == ratingVM.PeliculaId
                && x.UsuarioId == usuarioId);

            if(ratingActual == null)
            {
                var rating = new Rating();
                rating.PeliculaId = ratingVM.PeliculaId;
                rating.Puntuacion = ratingVM.Puntuacion;
                rating.UsuarioId = usuarioId;
                context.Add(rating);          
            }
            else
            {
                ratingActual.Puntuacion = ratingVM.Puntuacion;
            }

            await context.SaveChangesAsync();
            return NoContent();

        }
    }
}
