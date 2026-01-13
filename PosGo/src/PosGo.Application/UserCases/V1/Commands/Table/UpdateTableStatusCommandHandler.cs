using AutoMapper;
using Microsoft.AspNetCore.Http;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Table;

public sealed class UpdateTableStatusCommandHandler : ICommandHandler<Command.UpdateTableStatusCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Table, int> _tableRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public UpdateTableStatusCommandHandler(
        IRepositoryBase<Domain.Entities.Table, int> tableRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _tableRepository = tableRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.UpdateTableStatusCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();
        
        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // TenantFilter đã tự động filter theo RestaurantId
        var table = await _tableRepository.FindByIdAsync(request.Id);
        if (table == null)
        {
            return Result.Failure(new Error("TABLE_NOT_FOUND", "Không tìm thấy bàn."));
        }

        table.UpdateStatus(request.isActive);

        return Result.Success();
    }
}