using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Account;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Queries.account;

public sealed class GetAccountsQueryHandler : IQueryHandler<Query.GetAccountsQuery, PagedResult<Response.AccountResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public GetAccountsQueryHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository,
        IMapper mapper,
        ApplicationDbContext context)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _context = context;
    }
    public async Task<Result<PagedResult<Response.AccountResponse>>> Handle(Query.GetAccountsQuery request, CancellationToken cancellationToken)
    {
        if (request.SortColumnAndOrder.Any()) // =>>  Raw Query when order by multi column
        {
            var PageIndex = request.PageIndex <= 0 ? PagedResult<Domain.Entities.User>.DefaultPageIndex : request.PageIndex;
            var PageSize = request.PageSize <= 0
                ? PagedResult<Domain.Entities.User>.DefaultPageSize
                : request.PageSize > PagedResult<Domain.Entities.User>.UpperPageSize
                ? PagedResult<Domain.Entities.User>.UpperPageSize : request.PageSize;

            // ============================================
            var accountsQuery = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? @$"SELECT * FROM {nameof(Domain.Entities.User)} ORDER BY "
                : @$"SELECT * FROM {nameof(Domain.Entities.User)}
                        WHERE {nameof(Domain.Entities.User.FullName)} LIKE '%{request.SearchTerm}%'
                        ORDER BY ";

            foreach (var item in request.SortColumnAndOrder)
                accountsQuery += item.Value == SortOrder.Descending
                    ? $"{item.Key} DESC, "
                    : $"{item.Key} ASC, ";

            accountsQuery = accountsQuery.Remove(accountsQuery.Length - 2);

            accountsQuery += $" OFFSET {(PageIndex - 1) * PageSize} ROWS FETCH NEXT {PageSize} ROWS ONLY";

            var accounts = await _context.Users.FromSqlRaw(accountsQuery)
                .ToListAsync(cancellationToken: cancellationToken);

            var totalCount = await _context.Users.CountAsync(cancellationToken);

            var accountPagedResult = PagedResult<Domain.Entities.User>.Create(accounts,
                PageIndex,
                PageSize,
                totalCount);

            var result = _mapper.Map<PagedResult<Response.AccountResponse>>(accountPagedResult);

            return Result.Success(result);
        }
        else // =>> Entity Framework
        {
            var accountsQuery = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? _userRepository.FindAll()
            : _userRepository.FindAll(x => x.FullName.Contains(request.SearchTerm));

            accountsQuery = request.SortOrder == SortOrder.Descending
            ? accountsQuery.OrderByDescending(GetSortProperty(request))
            : accountsQuery.OrderBy(GetSortProperty(request));

            var accounts = await PagedResult<Domain.Entities.User>.CreateAsync(accountsQuery,
                request.PageIndex,
                request.PageSize);

            var result = _mapper.Map<PagedResult<Response.AccountResponse>>(accounts);
            return Result.Success(result);
        }
    }

    private static Expression<Func<Domain.Entities.User, object>> GetSortProperty(Query.GetAccountsQuery request)
         => request.SortColumn?.ToLower() switch
         {
             "fullName" => account => account.FullName,
             _ => account => account.Id
             //_ => product => product.CreatedDate // Default Sort Descending on CreatedDate column
         };
}
