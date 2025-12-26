//using PosGo.Contract.Abstractions.Shared;
//using PosGo.Domain.Abstractions.Repositories;
//using PosGo.Domain.Exceptions;
//using PosGo.Contract.Services.V1.User;
//using AutoMapper;

//namespace PosGo.Application.UserCases.V1.Commands.User;

//public sealed class UpdateUserCommandHandler : ICommandHandler<Command.UpdateUserCommand>
//{
//    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
//    private readonly IMapper _mapper;

//    public UpdateUserCommandHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository, IMapper mapper)
//    {
//        _userRepository = userRepository;
//        _mapper = mapper;
//    }
//    public async Task<Result> Handle(Command.UpdateUserCommand request, CancellationToken cancellationToken)
//    {
//        var user = await _userRepository.FindByIdAsync(request.Id) ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(User));
//        var existedUserName = await _userRepository.FindSingleAsync(x => x.UserName.Equals(request.UserName) && x.Id != request.Id);
//        if (existedUserName is not null)
//        {
//            return Result.Failure(
//                new Error("EXISTED", "UserName đã tồn tại"));
//        }
//        user.UpdateUser(request.UserName, request.FullName, request.Phone);
//        var result = _mapper.Map<Response.UpdateUserResponse>(user);
//        return Result.Success(result);
//    }
//}
