using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Plan;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Queries.Plan;

public sealed class GetPlansQueryHandler : IQueryHandler<Query.GetPlansQuery, PagedResult<Response.PlanResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Plan, int> _planRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public GetPlansQueryHandler(
        IRepositoryBase<Domain.Entities.Plan, int> planRepository,
        IMapper mapper,
        ApplicationDbContext context)
    {
        _planRepository = planRepository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<Result<PagedResult<Response.PlanResponse>>> Handle(Query.GetPlansQuery request, CancellationToken cancellationToken)
    {
        if (request.SortColumnAndOrder.Any())
        {
            var PageIndex = request.PageIndex <= 0 ? PagedResult<Domain.Entities.Plan>.DefaultPageIndex : request.PageIndex;
            var PageSize = request.PageSize <= 0
                ? PagedResult<Domain.Entities.Plan>.DefaultPageSize
                : request.PageSize > PagedResult<Domain.Entities.Plan>.UpperPageSize
                ? PagedResult<Domain.Entities.Plan>.UpperPageSize : request.PageSize;

            var plansQuery = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? @$"SELECT * FROM {nameof(Domain.Entities.Plan)} ORDER BY "
                : @$"SELECT * FROM {nameof(Domain.Entities.Plan)}
                        WHERE {nameof(Domain.Entities.Plan.Code)} LIKE '%{request.SearchTerm}%'
                        ORDER BY ";

            foreach (var item in request.SortColumnAndOrder)
                plansQuery += item.Value == SortOrder.Descending
                    ? $"{item.Key} DESC, "
                    : $"{item.Key} ASC, ";

            plansQuery = plansQuery.Remove(plansQuery.Length - 2);
            plansQuery += $" OFFSET {(PageIndex - 1) * PageSize} ROWS FETCH NEXT {PageSize} ROWS ONLY";

            var plans = await _context.Plans.FromSqlRaw(plansQuery)
                .ToListAsync(cancellationToken: cancellationToken);

            var totalCount = await _context.Plans.CountAsync(cancellationToken);

            var planPagedResult = PagedResult<Domain.Entities.Plan>.Create(plans,
                PageIndex,
                PageSize,
                totalCount);

            var result = _mapper.Map<PagedResult<Response.PlanResponse>>(planPagedResult);

            return Result.Success(result);
        }
        else
        {
            var plansQuery = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? _planRepository.FindAll()
                : _planRepository.FindAll(x => x.Code.Contains(request.SearchTerm));

            plansQuery = request.SortOrder == SortOrder.Descending
                ? plansQuery.OrderByDescending(GetSortProperty(request))
                : plansQuery.OrderBy(GetSortProperty(request));

            var plans = await PagedResult<Domain.Entities.Plan>.CreateAsync(plansQuery,
                request.PageIndex,
                request.PageSize);

            var result = _mapper.Map<PagedResult<Response.PlanResponse>>(plans);
            return Result.Success(result);
        }
    }

    private static Expression<Func<Domain.Entities.Plan, object>> GetSortProperty(Query.GetPlansQuery request)
         => request.SortColumn?.ToLower() switch
         {
             "code" => plan => plan.Code,
             _ => plan => plan.CreatedAt
         };
}
