namespace AvalWebBackend.Application.Common.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string resource, object key)
        : base($"{resource} with id '{key}' was not found.") { }
}