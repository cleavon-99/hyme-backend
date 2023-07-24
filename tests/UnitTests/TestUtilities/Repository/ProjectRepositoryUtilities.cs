using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;

namespace TestUtilities.Repository
{
    public static class ProjectRepositoryUtilities
    {
        public static Project CreateProject() => Project.Create(Constants.Constants.Project.ProjectId, Constants.Constants.User.UserId);
        public static List<Project> CreateProjects() => new()
        {
            Project.Create(new ProjectId(Guid.NewGuid()), new UserId(Guid.NewGuid())),
            Project.Create(new ProjectId(Guid.NewGuid()), new UserId(Guid.NewGuid())),
            Project.Create(new ProjectId(Guid.NewGuid()), new UserId(Guid.NewGuid()))
        };
    }
}
