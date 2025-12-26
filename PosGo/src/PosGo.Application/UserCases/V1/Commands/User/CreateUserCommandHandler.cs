//using AutoMapper;
//using Microsoft.AspNetCore.Identity;
//using PosGo.Contract.Abstractions.Shared;
//using PosGo.Contract.Services.V1.User;
//using PosGo.Domain.Abstractions.Repositories;

//namespace PosGo.Application.UserCases.V1.Commands.User;

//public sealed class CreateUserCommandHandler : ICommandHandler<Command.CreateUserCommand>
//{
//    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
//    private readonly IMapper _mapper;
//    private readonly IPasswordHasher<Domain.Entities.User> _passwordHasher;

//    public CreateUserCommandHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository, IMapper mapper, IPasswordHasher<Domain.Entities.User> passwordHasher)
//    {
//        _userRepository = userRepository;
//        _mapper = mapper;
//        _passwordHasher = passwordHasher;
//    }

//    public async Task<Result> Handle(Command.CreateUserCommand request, CancellationToken cancellationToken)
//    {
//        var userExisted = await _userRepository.FindSingleAsync(x => x.UserName.Equals(request.UserName));
//        if (userExisted is not null)
//        {
//            return Result.Failure<Response.UserResponse>(
//                new Error("EXISTED", "UserName đã tồn tại"));
//        }

//        if (request.Password != request.ConfirmPassword)
//        {
//            return Result.Failure<Response.UserResponse>(
//                new Error("PASSWORD_CONFIRM_NOT_MATCH", "Mật khẩu và xác nhận mật khẩu không trùng nhau."));
//        }

//        var user = Domain.Entities.User.CreateUser(Guid.NewGuid(), request.UserName, request.Password, request.FullName, request.Phone);

//        var passwordHash = _passwordHasher.HashPassword(user, request.Password);
//        user.ChangePassword(passwordHash);

//        _userRepository.Add(user);

//        var result = _mapper.Map<Response.UserResponse>(user);

//        return Result.Success(result);
//    }
//}
