using FluentResults;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.Users
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IUserRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(
            IUserRepository userProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            User? userProfile = await _userProfileRepository.GetByIdAsync(new UserId(request.UserProfileId));
            if (userProfile is null)
                return Result.Fail(new UserNotFoundError(request.UserProfileId));

            userProfile.Update(request.Name);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
