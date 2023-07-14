using FluentResults;

namespace Hyme.Shared.Errors
{
    public class UserProfileNotFoundError : Error
    {
        public UserProfileNotFoundError(Guid id) : base($"User profile with id {id} not found")
        {
            
        }
    }
}
