using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public AuthController(INotificador notificador,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager) : base(notificador)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
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
                return CustomResponse(viewModel);
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
                return CustomResponse(viewModel);
            }

            if (resultado.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse(viewModel);
            }

            NotificarErro("Usuário ou Senha incorretos");
            return CustomResponse(viewModel);
        }
    }
}
