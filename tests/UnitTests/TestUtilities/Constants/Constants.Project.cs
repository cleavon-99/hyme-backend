using Hyme.Domain.ValueObjects;

namespace TestUtilities.Constants
{

    public static partial class Constants
    {
        public static class Project
        {
            public static readonly ProjectId ProjectId = new(Guid.NewGuid());
            public const string Title = "Crypto project";
            public const string ShortDescription = "This is a short description";
            public const string ProjectDescription = "This is the project description";
        }
    }
   
}
