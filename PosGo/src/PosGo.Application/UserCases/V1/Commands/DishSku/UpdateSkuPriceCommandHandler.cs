using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishSku;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;

namespace PosGo.Application.UserCases.V1.Commands.DishSku;

public sealed class UpdateSkuPriceCommandHandler : ICommandHandler<Command.UpdateSkuPriceCommand>
{
    private readonly IRepositoryBase<Domain.Entities.DishSku, int> _skuRepository;

    public UpdateSkuPriceCommandHandler(IRepositoryBase<Domain.Entities.DishSku, int> skuRepository)
    {
        _skuRepository = skuRepository;
    }

    public async Task<Result> Handle(Command.UpdateSkuPriceCommand request, CancellationToken cancellationToken)
    {
        // Find SKU
        var sku = await _skuRepository.FindByIdAsync(request.Id, cancellationToken);
        if (sku == null)
        {
            return Result.Failure(new Error("SKU_NOT_FOUND", "Không tìm thấy SKU."));
        }

        // Update price
        sku.UpdatePrice(request.Price);

        return Result.Success();
    }
}