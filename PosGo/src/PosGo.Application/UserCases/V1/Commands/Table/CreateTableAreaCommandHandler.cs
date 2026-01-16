using AutoMapper;
using Microsoft.AspNetCore.Http;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Table;

public sealed class CreateTableAreaCommandHandler : ICommandHandler<Command.CreateTableAreaCommand>
{
    private readonly IRepositoryBase<TableArea, int> _tableAreaRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public CreateTableAreaCommandHandler(
        IRepositoryBase<TableArea, int> tableAreaRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _tableAreaRepository = tableAreaRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.CreateTableAreaCommand request, CancellationToken cancellationToken)
    {
        var restaurantId = _httpContextAccessor.HttpContext.GetRestaurantId();
        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // Kiểm tra trùng tên trong cùng nhà hàng
        var existingArea = await _tableAreaRepository.FindSingleAsync(
            x => x.RestaurantId == restaurantId.Value &&
                 x.Name.Trim().ToLower() == request.Name.Trim().ToLower());

        if (existingArea != null)
        {
            return Result.Failure(new Error("AREA_NAME_EXISTS", "Tên khu vực đã tồn tại."));
        }

        var tableArea = Domain.Entities.TableArea.Create(
            restaurantId.Value,
            request.Name,
            request.SortOrder,
            request.IsActive
        );

        _tableAreaRepository.Add(tableArea);
        var result = _mapper.Map<Response.TableAreaResponse>(tableArea);
        return Result.Success(result);
    }
}