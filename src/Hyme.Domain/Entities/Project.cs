using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Entities
{
    public class Project
    {
        public ProjectId Id { get; private set; }
        public string Logo { get; private set; }
        public string Banner { get; private set; }
        public string ShortDescription { get; private set; }
        public string ProjectDescription { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateModified { get; private set; }
        public DateTime? DateDeleted { get; private set; }
        public DateTime? DateLived { get; private set; }
        public UserId OwnerId { get; private set; }
        public User Owner { get; private set; } = null!;

        public Project(ProjectId id, UserId ownerId, string logo, string banner, string shortDescription, string projectDescription, DateTime dateCreated)
        {
            Id = id;
            OwnerId = ownerId;
            Logo = logo;
            Banner = banner;
            ShortDescription = shortDescription;
            ProjectDescription = projectDescription;
            DateCreated = dateCreated;
        }
    }
}
