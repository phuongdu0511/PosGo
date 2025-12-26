//using Microsoft.AspNetCore.Identity;
//using PosGo.Application.Abstractions;
//using PosGo.Contract.Abstractions.Shared;
//using PosGo.Contract.Services.V1.User;
//using PosGo.Domain.Abstractions.Repositories;
//using PosGo.Domain.Exceptions;

//namespace PosGo.Application.UserCases.V1.Commands.User;

//public sealed class ChangePasswordUserCommandHandler : ICommandHandler<Command.ChangePasswordUserCommand>
//{
//    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
//    private readonly IPasswordHasher<Domain.Entities.User> _passwordHasher;
//    private readonly ICacheService _cacheService;
//    public ChangePasswordUserCommandHandler(
//        IRepositoryBase<Domain.Entities.User, Guid> userRepository,
//        IPasswordHasher<Domain.Entities.User> passwordHasher,
//        ICacheService cacheService)
//    {
//        _userRepository = userRepository;
//        _passwordHasher = passwordHasher;
//        _cacheService = cacheService;
//    }

//    public async Task<Result> Handle(Command.ChangePasswordUserCommand request, CancellationToken cancellationToken)
//    {
//        if (request.NewPassword != request.ConfirmNewPassword)
//        {
//            return Result.Failure(new Error(
//                "PASSWORD_CONFIRM_NOT_MATCH",
//                "Mật khẩu mới và xác nhận mật khẩu không trùng nhau."));
//        }

//        var user = await _userRepository.FindByIdAsync(request.Id) ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(User));

//        var newHash = _passwordHasher.HashPassword(user, request.NewPassword);
//        user.ChangePassword(newHash);

//        await _cacheService.RemoveAsync(user.UserName, cancellationToken);

//        return Result.Success();
//    }
//}
