namespace PosGo.Domain.Exceptions;

public static class CommonNotFoundException
{
    public class CommonException : NotFoundException
    {
        public CommonException(Guid id, string entity)
            : base($"The {entity} with the id {id} was not found.") { }
    }
}
