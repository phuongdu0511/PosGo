using PosGo.Contract.Abstractions.Shared;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Abstractions;
using PosGo.Domain.Exceptions;
using PosGo.Contract.Services.V1.Product;

namespace PosGo.Application.UserCases.V1.Commands.Product;

public sealed class UpdateProductCommandHandler : ICommandHandler<Command.UpdateProductCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRepository;

    public UpdateProductCommandHandler(IRepositoryBase<Domain.Entities.Product, Guid> productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<Result> Handle(Command.UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.Id) ?? throw new ProductException.ProductNotFoundException(request.Id);
        product.Update(request.Name, request.Price, request.Description);
        return Result.Success(product);
    }
}
