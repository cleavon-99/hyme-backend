using FluentResults;
using Hyme.Application.Errors;
using Hyme.Application.Services;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, Result>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobService _blobService;

        public DeleteBannerCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IBlobService blobService
            )
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _blobService = blobService;
        }

        public async Task<Result> Handle(DeleteBannerCommand request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetByIdAsync(new ProjectId(request.ProjectId));
            if (project is null)
                return Result.Fail(new ProjectNotFoundError(request.ProjectId));

            await _blobService.DeleteImageAsync(project.Banner);
            project.DeleteBanner();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
