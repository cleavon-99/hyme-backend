using Hyme.Application.Queries.Whitelists;

namespace TestUtilities.Queries
{
    public static class WhitelistUtitlities
    {
        public static CheckUserIfWhitelistedQuery CheckUserIfWhitelistedQuery() => new(Constants.Constants.User.UserId.Value);
    }
}
