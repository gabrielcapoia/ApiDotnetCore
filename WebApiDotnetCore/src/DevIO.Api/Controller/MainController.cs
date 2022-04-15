using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace DevIO.Api.Controllers
{
    [ApiController]    
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador notificador;
        public readonly IUser appUser;

        protected Guid UsuarioId { get; set; }
        protected bool UsuarioAutenticado { get; set; }

        public MainController(INotificador notificador, 
            IUser appUser)
        {
            this.notificador = notificador;
            this.appUser = appUser;

            if (appUser.IsAuthenticated())
            {
                UsuarioId = appUser.GetUserId();
                UsuarioAutenticado = true;
            }
        }

        protected bool OperacaoValida()
        {
            return !notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(new 
                { 
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = notificador.ObterNotificacoes().Select(n => n.Mensagem)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
                NotificarErroModelInvalida(modelState);

            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);

            foreach (var erro in erros)
            {
                var errorMsg = erro?.Exception?.Message ?? erro.ErrorMessage;
                NotificarErro(errorMsg);
            }
        }

        protected void NotificarErro(string errorMsg)
        {
            notificador.Handle(new Notificacao(errorMsg));
        }
    }
}
