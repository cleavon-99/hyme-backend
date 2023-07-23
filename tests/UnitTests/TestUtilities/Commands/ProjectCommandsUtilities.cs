using Hyme.Application.Commands.Projects;
using Hyme.Domain.Entities;

namespace TestUtilities.Commands
{
    public static class ProjectCommandsUtilities
    {
        public static ApproveProjectCommand ApproveProjectCommand() => new(Constants.Constants.Project.ProjectId.Value);
        public static RejectProjectCommand RejectProjectCommand() => new(Constants.Constants.Project.ProjectId.Value);
        public static Project CreateProject() => Project.Create(Constants.Constants.Project.ProjectId, Constants.Constants.User.UserId);
    }
}
