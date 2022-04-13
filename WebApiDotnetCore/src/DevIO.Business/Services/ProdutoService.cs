using System;
using System.Threading.Tasks;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using DevIO.Business.Models.Validations;

namespace DevIO.Business.Services
{
    public class ProdutoService : BaseService, IProdutoService
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IUser user;

        public ProdutoService(IProdutoRepository produtoRepository,
                              INotificador notificador, IUser user) : base(notificador)
        {
            this.produtoRepository = produtoRepository;
            this.user = user;
        }

        public async Task Adicionar(Produto produto)
        {
            if (!ExecutarValidacao(new ProdutoValidation(), produto)) return;


            var loggedUser = user.GetUserId();

            await produtoRepository.Adicionar(produto);
        }

        public async Task Atualizar(Produto produto)
        {
            if (!ExecutarValidacao(new ProdutoValidation(), produto)) return;

            await produtoRepository.Atualizar(produto);
        }

        public async Task Remover(Guid id)
        {
            await produtoRepository.Remover(id);
        }

        public void Dispose()
        {
            produtoRepository?.Dispose();
        }
    }
}