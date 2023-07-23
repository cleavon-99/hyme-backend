using Hyme.Application.Queries.Projects;

namespace TestUtilities.Queries
{
    public static class ProjectQueriesUtilities
    {
        public static GetProjectByIdQuery GetProjectByIdQuery() => new(Constants.Constants.Project.ProjectId.Value);
    }
}
