using Hyme.Domain.Primitives;
using Hyme.Domain.ValueObjects;
using Microsoft.VisualBasic;

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
        public ProjectStatus Status { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateModified { get; private set; }
        public DateTime? DateApproved { get; private set; }
        public DateTime? DateRejected { get; private set; }
        public DateTime? DateDeleted { get; private set; }
        public DateTime? DateLived { get; private set; }
        public UserId OwnerId { get; private set; }
        public User Owner { get; private set; } = null!;

        private readonly List<NFT> _nfts = new();
        public IReadOnlyCollection<NFT> NFTs => _nfts;

        private Project(
            ProjectId id, 
            UserId ownerId
            )
        {
            Id = id;
            OwnerId = ownerId;  
        }
       

        public NFT AddNFT(string title, string description, string image)
        {
            NFT nft = new(new NFTId(Guid.NewGuid()), Id, title, description, image);
            _nfts.Add(nft);
            return nft;
        }

        public static Project Create(
            ProjectId id, 
            UserId ownerId)
        {
            return new(id, ownerId)
            {
                Status = ProjectStatus.Empty,
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
            DateModified = DateTime.UtcNow;
        }

        public void UpdateBanner(string bannerName)
        {
            Banner = bannerName;
            DateModified = DateTime.UtcNow;
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
            Status = ProjectStatus.Deleted;
            DateDeleted = DateTime.UtcNow;
        }

        public void Approve()
        {
            Status = ProjectStatus.Approved;
            DateApproved = DateTime.UtcNow;
        }

        public void Reject()
        {
            Status = ProjectStatus.Rejected;
            DateRejected = DateTime.UtcNow;
        }

        public void DeleteLogo()
        {
            Logo = string.Empty;
            DateModified = DateTime.UtcNow;
        }
        
        public void DeleteBanner()
        {
            Banner = string.Empty;
            DateModified = DateTime.UtcNow;
        }

        public void DeleteTrailer()
        {
            Trailer = string.Empty;
            DateModified = DateTime.UtcNow; 
        }

        public void Publish()
        {
            Status = ProjectStatus.InReview;
        }
    }
}
