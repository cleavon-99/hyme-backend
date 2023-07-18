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
        public readonly List<Role> _roles = new();
        public IReadOnlyCollection<Role> Roles => _roles;
        public string Ref { get; private set; } = null!;

        public User(
            UserId id, 
            WalletAddress walletAddress,
            DateTime dateCreated)
        {
            Id = id;
            WalletAddress = walletAddress;
            DateCreated = dateCreated;
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
