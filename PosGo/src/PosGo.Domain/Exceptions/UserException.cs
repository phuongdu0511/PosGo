namespace PosGo.Domain.Exceptions;

public static class UserException
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string userName)
            : base($"The user with the {userName} was not found.") { }
    }

    public class UserExistedException : NotFoundException
    {
        public UserExistedException(string userName)
            : base($"The user with the {userName} was existed.") { }
    }

    public class UserFieldException : NotFoundException
    {
        public UserFieldException(string userField)
            : base($"The user with the field {userField} is not correct.") { }
    }

    public class UserPasswordException : DomainException
    {
        public UserPasswordException()
            : base(nameof(UserPasswordException), $"Password is not correct!") { }
    }
}
