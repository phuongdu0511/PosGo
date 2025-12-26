//using System.Linq.Expressions;
//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using PosGo.Contract.Abstractions.Shared;
//using PosGo.Contract.Enumerations;
//using PosGo.Contract.Services.V1.User;
//using PosGo.Domain.Abstractions.Repositories;
//using PosGo.Persistence;

//namespace PosGo.Application.UserCases.V1.Queries.User;

//public sealed class GetUsersQueryHandler : IQueryHandler<Query.GetUsersQuery, PagedResult<Response.UserResponse>>
//{
//    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
//    private readonly IMapper _mapper;
//    private readonly ApplicationDbContext _context;

//    public GetUsersQueryHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository,
//        IMapper mapper,
//        ApplicationDbContext context)
//    {
//        _userRepository = userRepository;
//        _mapper = mapper;
//        _context = context;
//    }
//    public async Task<Result<PagedResult<Response.UserResponse>>> Handle(Query.GetUsersQuery request, CancellationToken cancellationToken)
//    {
//        if (request.SortColumnAndOrder.Any()) // =>>  Raw Query when order by multi column
//        {
//            var PageIndex = request.PageIndex <= 0 ? PagedResult<Domain.Entities.User>.DefaultPageIndex : request.PageIndex;
//            var PageSize = request.PageSize <= 0
//                ? PagedResult<Domain.Entities.User>.DefaultPageSize
//                : request.PageSize > PagedResult<Domain.Entities.User>.UpperPageSize
//                ? PagedResult<Domain.Entities.User>.UpperPageSize : request.PageSize;

//            // ============================================
//            var usersQuery = string.IsNullOrWhiteSpace(request.SearchTerm)
//                ? @$"SELECT * FROM {nameof(Domain.Entities.User)} ORDER BY "
//                : @$"SELECT * FROM {nameof(Domain.Entities.User)}
//                        WHERE {nameof(Domain.Entities.User.FullName)} LIKE '%{request.SearchTerm}%'
//                        ORDER BY ";

//            foreach (var item in request.SortColumnAndOrder)
//                usersQuery += item.Value == SortOrder.Descending
//                    ? $"{item.Key} DESC, "
//                    : $"{item.Key} ASC, ";

//            usersQuery = usersQuery.Remove(usersQuery.Length - 2);

//            usersQuery += $" OFFSET {(PageIndex - 1) * PageSize} ROWS FETCH NEXT {PageSize} ROWS ONLY";

//            var users = await _context.Users.FromSqlRaw(usersQuery)
//                .ToListAsync(cancellationToken: cancellationToken);

//            var totalCount = await _context.Users.CountAsync(cancellationToken);

//            var userPagedResult = PagedResult<Domain.Entities.User>.Create(users,
//                PageIndex,
//                PageSize,
//                totalCount);

//            var result = _mapper.Map<PagedResult<Response.UserResponse>>(userPagedResult);

//            return Result.Success(result);
//        }
//        else // =>> Entity Framework
//        {
//            var usersQuery = string.IsNullOrWhiteSpace(request.SearchTerm)
//            ? _userRepository.FindAll()
//            : _userRepository.FindAll(x => x.FullName.Contains(request.SearchTerm));

//            usersQuery = request.SortOrder == SortOrder.Descending
//            ? usersQuery.OrderByDescending(GetSortProperty(request))
//            : usersQuery.OrderBy(GetSortProperty(request));

//            var users = await PagedResult<Domain.Entities.User>.CreateAsync(usersQuery,
//                request.PageIndex,
//                request.PageSize);

//            var result = _mapper.Map<PagedResult<Response.UserResponse>>(users);
//            return Result.Success(result);
//        }
//    }

//    private static Expression<Func<Domain.Entities.User, object>> GetSortProperty(Query.GetUsersQuery request)
//         => request.SortColumn?.ToLower() switch
//         {
//             "fullName" => user => user.FullName,
//             _ => user => user.Id
//             //_ => product => product.CreatedDate // Default Sort Descending on CreatedDate column
//         };
//}
