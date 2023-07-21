using Hyme.Domain.Entities;
using Hyme.Domain.ValueObjects;

namespace Hyme.Application.DTOs.Response
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Logo { get; set; } = null!;
        public string Banner { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public string ProjectDescription { get; set; } = null!;
        public DateTime DateCreated { get; set; }
        
    }
}
