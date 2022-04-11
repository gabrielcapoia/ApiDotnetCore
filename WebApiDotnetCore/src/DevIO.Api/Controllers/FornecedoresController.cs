using AutoMapper;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository fornecedorRepository;
        private readonly IEnderecoRepository enderecoRepository;
        private readonly IFornecedorService fornecedorService;
        private readonly IMapper mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
            IFornecedorService fornecedorService,
            INotificador notificador,
            IMapper mapper) : base(notificador)
        {
            this.fornecedorRepository = fornecedorRepository;
            this.fornecedorService = fornecedorService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
        {
            var fornecedores = await fornecedorRepository.ObterTodos();
            return mapper.Map<IEnumerable<FornecedorViewModel>>(fornecedores);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {
            var fornecedor = await ObterFornecedorProdutosEndereco(id);

            if (fornecedor == null)
                return NotFound();

            return fornecedor;
        }

        [ClaimsAuthorize("Fornecedor","Adicionar")]
        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            await fornecedorService.Adicionar(mapper.Map<Fornecedor>(viewModel));

            return CustomResponse(viewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            await fornecedorService.Atualizar(mapper.Map<Fornecedor>(viewModel));

            return CustomResponse(viewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null)
                return NotFound();

            await fornecedorService.Remover(id);

            return CustomResponse();

        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpGet("obter-endereco/{id:guid}")]
        public async Task<EnderecoViewModel> ObterEnderecoPorId(Guid id)
        {
            return mapper.Map<EnderecoViewModel>(await enderecoRepository.ObterPorId(id));
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("atualizar-endereco/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await fornecedorService.AtualizarEndereco(mapper.Map<Endereco>(viewModel));

            return CustomResponse(viewModel);
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return mapper.Map<FornecedorViewModel>(await fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return mapper.Map<FornecedorViewModel>(await fornecedorRepository.ObterFornecedorEndereco(id));
        }
    }
}
