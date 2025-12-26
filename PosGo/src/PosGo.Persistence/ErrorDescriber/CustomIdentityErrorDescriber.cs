using Microsoft.AspNetCore.Identity;

namespace PosGo.Persistence.ErrorDescriber;

public sealed class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length)
        => new()
        {
            Code = nameof(PasswordTooShort),
            Description = $"Mật khẩu phải có ít nhất {length} ký tự."
        };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Mật khẩu phải có ít nhất 1 ký tự đặc biệt."
        };

    public override IdentityError PasswordRequiresDigit()
        => new()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Mật khẩu phải có ít nhất 1 chữ số (0-9)."
        };

    public override IdentityError PasswordRequiresLower()
        => new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Mật khẩu phải có ít nhất 1 chữ thường (a-z)."
        };

    public override IdentityError PasswordRequiresUpper()
        => new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Mật khẩu phải có ít nhất 1 chữ hoa (A-Z)."
        };

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        => new()
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = $"Mật khẩu phải có ít nhất {uniqueChars} ký tự khác nhau."
        };
}
