using Hyme.Application.Commands.Projects;
using Hyme.Domain.Entities;

namespace TestUtilities.Commands
{
    public static class ProjectCommandsUtilities
    {
        public static ApproveProjectCommand ApproveProjectCommand() => new(Constants.Constants.Project.ProjectId.Value);
        public static RejectProjectCommand RejectProjectCommand() => new(Constants.Constants.Project.ProjectId.Value);
        public static UpdateProjectInfoCommand UpdateProjectInfoCommand() => new(
            Constants.Constants.Project.ProjectId.Value,
            Constants.Constants.Project.Title,
            Constants.Constants.Project.ShortDescription,
            Constants.Constants.Project.ProjectDescription);
    }
}
