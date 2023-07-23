using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Queries.Whitelists
{
    public class CheckUserIfWhitelistedQueryHandler : IRequestHandler<CheckUserIfWhitelistedQuery, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IWhitelistRepository _whitelistRepository;

        public CheckUserIfWhitelistedQueryHandler(IUserRepository userRepository, IWhitelistRepository whitelistRepository)
        {
            _userRepository = userRepository;
            _whitelistRepository = whitelistRepository;
        }

        public async Task<bool> Handle(CheckUserIfWhitelistedQuery request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetByIdAsync(new UserId(request.Id));
            if (user is null)
                return false;

            Whitelist? whiteList = await _whitelistRepository.FindAsync(user.WalletAddress);
            if (whiteList is null)
                return false;
            return true;

        }
    }
}
