using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly AppSettings appSettings;

        public AuthController(INotificador notificador,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> appSettings) : base(notificador)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(RegisterUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var user = new IdentityUser()
            {
                UserName = viewModel.Email,
                Email = viewModel.Email,
                EmailConfirmed = true
            };

            var resultado = await userManager.CreateAsync(user, viewModel.Password);

            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(user, false);
                return CustomResponse(GerarJwt());
            }

            foreach (var error in resultado.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse(viewModel);
        }

        [HttpPost("entrar")]
        public async Task<ActionResult> Login(LoginUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var resultado = await signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, false, true);

            if (resultado.Succeeded)
            {
                return CustomResponse(GerarJwt());
            }

            if (resultado.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse(viewModel);
            }

            NotificarErro("Usuário ou Senha incorretos");
            return CustomResponse(viewModel);
        }

        private string GerarJwt()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor() 
            { 
                Issuer = appSettings.Emissor,
                Audience = appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);
            return encodedToken;
        }
    }
}
