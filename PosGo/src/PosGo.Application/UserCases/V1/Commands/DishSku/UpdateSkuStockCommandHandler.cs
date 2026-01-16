using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishSku;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.DishSku;

public sealed class UpdateSkuStockCommandHandler : ICommandHandler<Command.UpdateSkuStockCommand>
{
    private readonly IRepositoryBase<Domain.Entities.DishSku, int> _skuRepository;

    public UpdateSkuStockCommandHandler(IRepositoryBase<Domain.Entities.DishSku, int> skuRepository)
    {
        _skuRepository = skuRepository;
    }

    public async Task<Result> Handle(Command.UpdateSkuStockCommand request, CancellationToken cancellationToken)
    {
        // Find SKU
        var sku = await _skuRepository.FindByIdAsync(request.Id, cancellationToken);
        if (sku == null)
        {
            return Result.Failure(new Error("SKU_NOT_FOUND", "Không tìm thấy SKU."));
        }

        // Update stock
        sku.UpdateStock(request.StockQuantity);

        return Result.Success();
    }
}