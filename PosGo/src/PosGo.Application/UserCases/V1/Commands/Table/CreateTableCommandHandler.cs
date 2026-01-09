using AutoMapper;
using Microsoft.AspNetCore.Http;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Table;

public sealed class CreateTableCommandHandler : ICommandHandler<Command.CreateTableCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Table, Guid> _tableRepository;
    private readonly IRepositoryBase<TableArea, Guid> _tableAreaRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public CreateTableCommandHandler(
        IRepositoryBase<Domain.Entities.Table, Guid> tableRepository,
        IRepositoryBase<TableArea, Guid> tableAreaRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _tableRepository = tableRepository;
        _tableAreaRepository = tableAreaRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.CreateTableCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();
        
        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // Kiểm tra khu vực (nếu có) - TenantFilter đã tự động filter theo RestaurantId
        if (request.AreaId.HasValue)
        {
            var area = await _tableAreaRepository.FindByIdAsync(request.AreaId.Value);
            if (area == null)
            {
                return Result.Failure(new Error("AREA_NOT_FOUND", "Không tìm thấy khu vực."));
            }
        }

        // Kiểm tra trùng mã bàn - TenantFilter đã tự động filter theo RestaurantId
        var existingTable = await _tableRepository.FindSingleAsync(
            x => x.Name.ToLower() == request.Name.ToLower());

        if (existingTable != null)
        {
            return Result.Failure(new Error("TABLE_CODE_EXISTS", "Mã bàn đã tồn tại."));
        }

        // Tạo QR Code Token unique

        var table = Domain.Entities.Table.Create(
            Guid.NewGuid(),
            restaurantId.Value,
            request.Name,
            request.QrCodeToken,
            request.AreaId,
            request.Seats,
            request.DoNotAllowOrder,
            true,
            request.MinOrderAmount);

        _tableRepository.Add(table);

        var result = _mapper.Map<Response.TableResponse>(table);
        return Result.Success(result);
    }
}