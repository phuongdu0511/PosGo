using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Table;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Queries.Table;

public sealed class GetTableByQrCodeQueryHandler : IQueryHandler<Query.GetTableByQrCodeQuery, Response.TableResponse>
{
    private readonly IRepositoryBase<Domain.Entities.Table, Guid> _tableRepository;
    private readonly IMapper _mapper;

    public GetTableByQrCodeQueryHandler(
        IRepositoryBase<Domain.Entities.Table, Guid> tableRepository,
        IMapper mapper)
    {
        _tableRepository = tableRepository;
        _mapper = mapper;
    }

    public async Task<Result<Response.TableResponse>> Handle(Query.GetTableByQrCodeQuery request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.FindAll(x => x.QrCodeToken == request.QrCodeToken)
            .Include(x => x.Area)
            .Include(x => x.Restaurant)
            .FirstOrDefaultAsync(cancellationToken);

        if (table == null)
        {
            return Result.Failure<Response.TableResponse>(
                new Error("TABLE_NOT_FOUND", "Không tìm thấy bàn với mã QR này."));
        }

        var result = _mapper.Map<Response.TableResponse>(table);
        return Result.Success(result);
    }
}