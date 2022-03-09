using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using peliculasApi.Utilidades;
using peliculasApi.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace peliculasApi.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    public class CuentasController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IMapper mapper;

        public CuentasController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            ApplicationDbContext context, 
            SignInManager<IdentityUser> signInManager,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.context = context;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }


        [HttpGet("listadoUsuarios")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult<List<UsuarioViewModel>>> ListadoUsuarios([FromQuery] PaginacionViewModel paginacionVM)
        {
            var queryable = context.Users.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var usuarios = await queryable.OrderBy(x => x.Email).Paginar(paginacionVM).ToListAsync();
            return mapper.Map<List<UsuarioViewModel>>(usuarios);
        }

        [HttpPost("HacerAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> HacerAdmin([FromBody] string usuarioId)
        {
            var usuario = await userManager.FindByIdAsync(usuarioId);
            await userManager.AddClaimAsync(usuario, new Claim("role", "admin"));
            return NoContent();
        }

        [HttpPost("RemoveAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> RemoveAdmin([FromBody] string usuarioId)
        {
            var usuario = await userManager.FindByIdAsync(usuarioId);
            await userManager.RemoveClaimAsync(usuario, new Claim("role", "admin"));
            return NoContent();
        }


        [HttpPost("crear")]
        public async Task<ActionResult<RespuestaAutenticacion>> Crear([FromBody] CredencialesUsuario credenciales)
        {
            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var resultado = await userManager.CreateAsync(usuario, credenciales.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login([FromBody] CredencialesUsuario credenciales)
        {

            var resultado = await signInManager.PasswordSignInAsync(credenciales.Email, credenciales.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }

        private async Task<RespuestaAutenticacion> ConstruirToken (CredencialesUsuario credenciales)
        {
            var claimss = new List<Claim>()
            {
                //new Claim(ClaimTypes.)
                //new Claim("username", credenciales.Email),
                new Claim("email", credenciales.Email)        
            };


            var usuario = await userManager.FindByEmailAsync(credenciales.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claimss.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claimss, expires: expiracion, signingCredentials: creds);


            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion

            };

        }



    }
}
