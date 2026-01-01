using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Role;

namespace PosGo.Application.UserCases.V1.Commands.Role;

public sealed class CreateRoleCommandHandler : ICommandHandler<Command.CreateRole>
{
    private readonly RoleManager<Domain.Entities.Role> _roleManager;
    private readonly IMapper _mapper;
    public CreateRoleCommandHandler(RoleManager<Domain.Entities.Role> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.CreateRole request, CancellationToken cancellationToken)
    {
        if (await _roleManager.RoleExistsAsync(request.RoleCode))
        {
            return Result.Failure<Response.RoleResponse>(
                new Error("EXISTED", "Role đã tồn tại"));
        }

        var role = new Domain.Entities.Role()
        {
            Id = Guid.NewGuid(),
            Scope = request.Scope,
            RoleCode = request.RoleCode,
            Name = request.Name,
            Description = request.Description,
        };

        var createdResult = await _roleManager.CreateAsync(role);

        if (!createdResult.Succeeded)
        {
            var codes = createdResult.Errors
                .Select(e => e.Code)
                .Distinct()
                .ToList();

            return Result.Failure(new Error(
                code: JsonSerializer.Serialize(codes),
                message: "VALIDATION_FAILED"
            ));
        }

        var result = _mapper.Map<Response.RoleResponse>(role);

        return Result.Success(result);
    }
}
