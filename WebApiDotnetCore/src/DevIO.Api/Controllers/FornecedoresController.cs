using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository fornecedorRepository;
        private readonly IFornecedorService fornecedorService;
        private readonly IMapper mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
            IFornecedorService fornecedorService,
            IMapper mapper)
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

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var fornecedor = mapper.Map<Fornecedor>(viewModel);
            var result = await fornecedorService.Adicionar(fornecedor);

            if (!result)
                return BadRequest();

            return Ok(fornecedor);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel viewModel)
        {
            if (id != viewModel.Id || !ModelState.IsValid)
                return BadRequest();

            var fornecedor = mapper.Map<Fornecedor>(viewModel);
            var result = await fornecedorService.Atualizar(fornecedor);

            if (!result)
                return BadRequest();

            return Ok(fornecedor);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor == null)
                return NotFound();

            var result = await fornecedorService.Remover(id);

            if (!result)
                return BadRequest();

            return Ok(fornecedor);

        }

        public async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            var fornecedor = await fornecedorRepository.ObterFornecedorProdutosEndereco(id);
            return mapper.Map<FornecedorViewModel>(fornecedor);
        }

        public async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            var fornecedor = await fornecedorRepository.ObterFornecedorEndereco(id);
            return mapper.Map<FornecedorViewModel>(fornecedor);
        }
    }
}
