using FluentResults;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public class UpdateProjectInfoCommandHandler : IRequestHandler<UpdateProjectInfoCommand, Result>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProjectInfoCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProjectInfoCommand request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetByIdAsync(new ProjectId(request.ProjectId));
            if (project is null)
                return Result.Fail(new ProjectNotFoundError(request.ProjectId));

            project.UpdateInfo(request.Title, request.ShortDescription, request.ProjectDescription);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
