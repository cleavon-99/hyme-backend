using FluentResults;
using Hyme.Application.Errors;
using Hyme.Application.Services;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;
using System.Runtime.CompilerServices;

namespace Hyme.Application.Commands.Projects
{
    public class UpdateProjectTrailerCommandHandler : IRequestHandler<UpdateProjectTrailerCommand, Result>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobService _blobService;

        public UpdateProjectTrailerCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork, IBlobService blobService)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _blobService = blobService;
        }

        public async Task<Result> Handle(UpdateProjectTrailerCommand request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetByIdAsync(new ProjectId(request.ProjectId));
            if (project is null)
                return Result.Fail(new ProjectNotFoundError(request.ProjectId));

            if(!string.IsNullOrWhiteSpace(project.Trailer))
                await _blobService.DeleteVideoAsync(project.Trailer);

            project.UpdateTrailer(request.FileName);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _blobService.UploadVideoAsync(request.Trailer, request.FileName);
            return Result.Ok();
        }
    }
}
