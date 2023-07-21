using FluentResults;

namespace Hyme.Application.Errors
{
    public class ProjectNotFoundError : Error
    {
        public ProjectNotFoundError(Guid id) : base($"Project with id {id} not found")
        {
            
        }

    }
}
