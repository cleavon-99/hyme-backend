using Hyme.Domain.ValueObjects;

namespace Hyme.Application.Tests.TestUtilities.Constants
{
    public static partial class Constants
    {
        public static class Project
        {
            public static readonly ProjectId Id = new(Guid.NewGuid());
            public const string Title = "Crypto project";
            public const string ShortDescription = "This is a short description";
            public const string ProjectDescription = "This is the project description";
        }
    }
}
