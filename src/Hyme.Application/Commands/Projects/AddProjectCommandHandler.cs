using AutoMapper;
using FluentResults;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public class AddProjectCommandHandler : IRequestHandler<AddProjectCommand, Result<ProjectResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddProjectCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProjectResponse>> Handle(AddProjectCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetByIdWithProjectAsync(new UserId(request.UserId));
            if (user is null)
                return Result.Fail(new UserNotFoundError(request.UserId));

            //var result = user.CreateProject(request.Title, "Logo", "Banner", request.ShortDescription, request.ProjectDescription);
            var result = Result.Ok(new ProjectResponse());
            if (result.IsFailed)
                return Result.Fail(result.Errors);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ProjectResponse>(result.Value);
        }
    }
}
