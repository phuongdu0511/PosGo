using MediatR;

namespace PosGo.Contract.Abstractions.Shared;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{ }
