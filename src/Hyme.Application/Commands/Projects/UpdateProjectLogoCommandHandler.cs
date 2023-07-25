using FluentResults;
using Hyme.Application.Errors;
using Hyme.Application.Services;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public class UpdateProjectLogoCommandHandler : IRequestHandler<UpdateProjectLogoCommand, Result>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IBlobService _blobService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProjectLogoCommandHandler(
            IProjectRepository projectRepository,
            IBlobService blobService,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _blobService = blobService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateProjectLogoCommand request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetByIdAsync(new ProjectId(request.ProjectId));
            if (project is null)
                return Result.Fail(new ProjectNotFoundError(request.ProjectId));

            if (!string.IsNullOrEmpty(project.Logo))
                await _blobService.DeleteImageAsync(project.Logo);
            
            project.UpdateLogo(request.FileName);
            await _blobService.UploadImageAsync(request.Logo, request.FileName);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
