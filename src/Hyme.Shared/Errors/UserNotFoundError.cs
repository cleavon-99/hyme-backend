using FluentResults;

namespace Hyme.Shared.Errors
{
    public class UserNotFoundError : Error
    {
        public UserNotFoundError(Guid id) : base($"User profile with id {id} not found")
        {
            
        }
    }
}
