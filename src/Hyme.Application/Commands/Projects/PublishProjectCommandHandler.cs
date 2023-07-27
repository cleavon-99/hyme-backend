using FluentResults;
using Hyme.Application.Errors;
using Hyme.Domain.Entities;
using Hyme.Domain.Repositories;
using Hyme.Domain.ValueObjects;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public class PublishProjectCommandHandler : IRequestHandler<PublishProjectCommand, Result>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PublishProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(PublishProjectCommand request, CancellationToken cancellationToken)
        {
            Project? project = await _projectRepository.GetByOwnerIdAsync(new UserId(request.UserId));
            if (project is null)
                return Result.Fail(new ProjectNotFoundError(request.UserId));

            project.Publish();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
