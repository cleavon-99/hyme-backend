using FluentResults;

namespace Hyme.Domain.Errors
{
    public class ProjectAlreadyCreatedError : Error
    {
        public ProjectAlreadyCreatedError() : base("Your account already has a project created")
        {
            
        }
    }
}
