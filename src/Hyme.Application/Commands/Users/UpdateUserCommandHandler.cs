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
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            User? userProfile = await _userRepository.GetByIdAsync(new UserId(request.UserId));
            if (userProfile is null)
                return Result.Fail(new UserNotFoundError(request.UserId));

            userProfile.Update(request.Name);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
