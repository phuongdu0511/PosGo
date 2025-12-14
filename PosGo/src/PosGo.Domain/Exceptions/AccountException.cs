namespace PosGo.Domain.Exceptions;

public static class AccountException
{
    public class AccountNotFoundException : NotFoundException
    {
        public AccountNotFoundException(Guid accountId)
            : base($"The Account with the id {accountId} was not found.") { }
    }
}
