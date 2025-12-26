//using PosGo.Contract.Abstractions.Shared;
//using PosGo.Contract.Abstractions.Shared.CommonServices;
//using PosGo.Contract.Services.V1.User;
//using PosGo.Domain.Abstractions.Repositories;
//using PosGo.Domain.Entities;
//using PosGo.Domain.Exceptions;
//using PosGo.Persistence;

//namespace PosGo.Application.UserCases.V1.Commands.User;

//public sealed class UpdateUserRolesCommandHandler : ICommandHandler<Command.UpdateUserRolesCommand>
//{
//    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
//    private readonly IRepositoryBase<Role, Guid> _roleRepository;
//    private readonly ICurrentUserService _currentUserService;
//    private readonly ApplicationDbContext _dbContext;

//    public UpdateUserRolesCommandHandler(
//        IRepositoryBase<Domain.Entities.User, Guid> userRepository,
//        ICurrentUserService currentUserService,
//        IRepositoryBase<Role, Guid> roleRepository,
//        ApplicationDbContext dbContext)
//    {
//        _userRepository = userRepository;
//        _currentUserService = currentUserService;
//        _roleRepository = roleRepository;
//        _dbContext = dbContext;
//    }

//    public async Task<Result> Handle(Command.UpdateUserRolesCommand request, CancellationToken cancellationToken)
//    {
//        if (_currentUserService.UserId is null)
//        {
//            return Result.Failure<Response.UserResponse>(
//                new Error("UNAUTHORIZED", "User is not authenticated."));
//        }

//        var user = await _userRepository.FindByIdAsync(request.Id) ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(User));

//        //var roleCodes = (request.RoleCodes ?? Array.Empty<string>())
//        //    .Select(rc => rc.Trim())
//        //    .Where(rc => !string.IsNullOrWhiteSpace(rc))
//        //    .Distinct(StringComparer.OrdinalIgnoreCase)
//        //    .ToList();

//        //var systemRoles = new List<Domain.Entities.Identity.Role>();

//        //if (roleCodes.Count > 0)
//        //{
//        //    systemRoles = await _roleRepository
//        //        .FindAll(r =>
//        //            roleCodes.Contains(r.Code) &&
//        //            r.Scope == SystemConstants.Scope.SYSTEM &&
//        //            r.IsActive)
//        //        .ToListAsync(cancellationToken);

//        //    // Nếu có code không tồn tại
//        //    if (systemRoles.Count != roleCodes.Count)
//        //    {
//        //        var foundCodes = systemRoles.Select(r => r.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
//        //        var missing = roleCodes.Where(rc => !foundCodes.Contains(rc)).ToList();

//        //        return Result.Failure<Response.UserResponse>(
//        //            new Error("ROLE_NOT_FOUND",
//        //                $"Các quyền không hợp lệ: {string.Join(", ", missing)}"));
//        //    }
//        //}

//        //// 5. Xoá toàn bộ system-roles hiện tại của user
//        //var currentSystemRoles = await (
//        //    from usr in _dbContext.UserSystemRoles
//        //    join r in _dbContext.Roles on usr.RoleId equals r.Id
//        //    where usr.UserId == user.Id && r.Scope == SystemConstants.Scope.SYSTEM
//        //    select usr
//        //).ToListAsync(cancellationToken);

//        //_dbContext.UserSystemRoles.RemoveRange(currentSystemRoles);

//        //// 6. Thêm lại theo danh sách mới
//        //foreach (var role in systemRoles)
//        //{
//        //    var newRole = new UserSystemRole(user.Id, role.Id);
//        //    _dbContext.UserSystemRoles.Add(newRole);
//        //}

//        return Result.Success();
//    }
//}
