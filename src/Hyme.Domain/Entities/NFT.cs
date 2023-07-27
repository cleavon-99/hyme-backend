using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Entities
{
    public class NFT
    {
        public NFTId Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Image { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime DateModified { get; private set; }
        public DateTime DateDeleted { get; private set; }
        public ProjectId ProjectId { get; private set; }

        public NFT(NFTId id, ProjectId projectId, string title, string description, string image)
        {
            Id = id;
            ProjectId = projectId;
            Title = title;
            Description = description;
            Image = image;
        }

        public static NFT Create(NFTId id, ProjectId projectId, string title, string description, string image)
        {
            NFT nft = new(id, projectId, title, description, image) 
            {
                DateCreated = DateTime.UtcNow,
            };

            return nft;
        }

    }
}
