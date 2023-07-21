using Hyme.Domain.Primitives;
using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Entities
{
    public class Project
    {
        public ProjectId Id { get; private set; }    
        public string Title { get; private set; }
        public string Logo { get; private set; }
        public string Banner { get; private set; }
        public string ShortDescription { get; private set; }
        public string ProjectDescription { get; private set; }
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
            UserId ownerId, 
            string title,
            string logo, 
            string banner, 
            string shortDescription, 
            string projectDescription, 
            DateTime dateCreated)
        {
            Id = id;
            OwnerId = ownerId;
            Title = title;
            Logo = logo;
            Banner = banner;
            ShortDescription = shortDescription;
            ProjectDescription = projectDescription;
            DateCreated = dateCreated;
        }

        public static Project Create(
            ProjectId id, 
            UserId ownerId, 
            string title, 
            string logo, 
            string banner, 
            string shortDescription, 
            string projectDescription)
        {
            return new(id, ownerId, title, logo, banner, shortDescription, projectDescription, DateTime.UtcNow)
            {
                Status = PublishStatus.InReview
            };
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
