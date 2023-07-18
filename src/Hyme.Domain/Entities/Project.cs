using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Entities
{
    public class Project
    {
        public ProjectId Id { get; private set; }
        public string Logo { get; private set; }
        public string Banner { get; private set; }
        public string ShortDescription { get; private set; }
        public string ProjectDescription { get; set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateModified { get; private set; }
        public DateTime? DateDeleted { get; set; }
        public DateTime? DateLived { get; set; }


        public Project(ProjectId id, string logo, string banner, string shortDescription, string projectDescription, DateTime dateCreated)
        {
            Id = id;
            Logo = logo;
            Banner = banner;
            ShortDescription = shortDescription;
            ProjectDescription = projectDescription;
            DateCreated = dateCreated;
        }
    }
}
