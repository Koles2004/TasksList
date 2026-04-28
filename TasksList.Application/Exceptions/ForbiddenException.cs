namespace TasksList.Application.Exceptions;

public class ForbiddenException(string message) : Exception(message)
{
}
