using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {
        private readonly ILogger<TesteController> logger;

        public TesteController(INotificador notificador, 
            IUser appUser, ILogger<TesteController> logger) : base(notificador, appUser)
        {
            this.logger = logger;
        }

        [HttpGet]
        public string Valor()
        {
            throw new Exception("Error");

            //logger.LogTrace("Log de Trace");
            //logger.LogDebug("Log de Debug");
            //logger.LogInformation("Log de Informação");
            //logger.LogWarning("Log de Aviso");
            //logger.LogError("Log de Erro");
            //logger.LogCritical("Log de Problema Critico");

            return "Api versão 2.0";
        }
    }
}
