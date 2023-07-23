using FluentResults;
using Hyme.Domain.Errors;
using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Entities
{
    public class User
    {
        public UserId Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public WalletAddress WalletAddress { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime? DateLastLogin { get; private set; }
        public DateTime? DateLastLogout { get; private set; }
        public DateTime? DateLastUpdated { get; private set; }
        public DateTime? DateDeleted { get; private set; }
        public Project Project { get; private set; } = null!;
        
        private readonly List<Role> _roles = new();
        public IReadOnlyCollection<Role> Roles => _roles;
       
        private User(
            UserId id, 
            WalletAddress walletAddress)
        {
            Id = id;
            WalletAddress = walletAddress;
        }

        public static User Create(UserId id, WalletAddress walletAddress)
        {
            User user = new(id, walletAddress)
            {
                DateCreated = DateTime.UtcNow,
                Project = Project.Create(new ProjectId(Guid.NewGuid()), id)
            };

            return user;
        }

        public void Update(string name)
        {
            Name = name;  
        }

        public void AddRole(Role role)
        {
            _roles.Add(role);
        }
    }
}
