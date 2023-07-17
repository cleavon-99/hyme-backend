using Hyme.Domain.ValueObjects;

namespace Hyme.Domain.Entities
{
    public class Role
    {
        public RoleId Id { get; set; }
        public string Name { get; set; }      
        public readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users;

        public Role(RoleId id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
