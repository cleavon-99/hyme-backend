using Hyme.Domain.Primitives;
using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Entities
{
    public class Project
    {
        public ProjectId Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Logo { get; private set; } = string.Empty;
        public string Banner { get; private set; } = string.Empty;
        public string Trailer { get; private set; } = string.Empty;
        public string ShortDescription { get; private set; } = string.Empty;
        public string ProjectDescription { get; private set; } = string.Empty;
        public PublishStatus Status { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateModified { get; private set; }
        public DateTime? DateApproved { get; private set; }
        public DateTime? DateRejected { get; private set; }
        public DateTime? DateDeleted { get; private set; }
        public DateTime? DateLived { get; private set; }
        public UserId OwnerId { get; private set; }
        public User Owner { get; private set; } = null!;

        private Project(
            ProjectId id, 
            UserId ownerId
            )
        {
            Id = id;
            OwnerId = ownerId;  
        }

        public static Project Create(
            ProjectId id, 
            UserId ownerId)
        {
            return new(id, ownerId)
            {
                Status = PublishStatus.Empty,
                DateCreated = DateTime.UtcNow
            };
        }

        public void UpdateLogo(string logoName)
        {
            Logo = logoName;
            DateModified = DateTime.UtcNow;
        }

        public void UpdateTrailer(string trailerName)
        {
            Trailer = trailerName;
        }

        public void UpdateInfo(
            string title, 
            string shortDescription,
            string projectDescription)
        {
            Title = title;
            ShortDescription = shortDescription;
            ProjectDescription = projectDescription;
            DateModified = DateTime.UtcNow;
        }

        public void Delete()
        {
            Status = PublishStatus.Deleted;
            DateDeleted = DateTime.UtcNow;
        }

        public void Approve()
        {
            Status = PublishStatus.Approved;
            DateApproved = DateTime.UtcNow;
        }

        public void Reject()
        {
            Status = PublishStatus.Rejected;
            DateRejected = DateTime.UtcNow;
        }
    }
}
