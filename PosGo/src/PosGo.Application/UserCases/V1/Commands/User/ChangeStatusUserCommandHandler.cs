//using PosGo.Contract.Abstractions.Shared;
//using PosGo.Domain.Abstractions.Repositories;
//using PosGo.Domain.Exceptions;
//using PosGo.Contract.Services.V1.User;

//namespace PosGo.Application.UserCases.V1.Commands.User;

//public sealed class ChangeStatusUserCommandHandler : ICommandHandler<Command.ChangeStatusUserCommand>
//{
//    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;

//    public ChangeStatusUserCommandHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository)
//    {
//        _userRepository = userRepository;
//    }
//    public async Task<Result> Handle(Command.ChangeStatusUserCommand request, CancellationToken cancellationToken)
//    {
//        var user = await _userRepository.FindByIdAsync(request.Id) ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(User));
//        user.ChangeStatusUser(request.status);
//        return Result.Success();
//    }
//}
