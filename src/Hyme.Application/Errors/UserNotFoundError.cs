using FluentResults;

namespace Hyme.Application.Errors
{
    public class UserNotFoundError : Error
    {
        public UserNotFoundError(Guid id) : base($"User profile with id {id} not found")
        {

        }
    }
}
