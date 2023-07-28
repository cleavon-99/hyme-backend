using FluentResults;
using Hyme.Application.Services;
using Hyme.Domain.Repositories;
using MediatR;

namespace Hyme.Application.Commands.Projects
{
    public class DeleteLogoCommandHandler : IRequestHandler<DeleteLogoCommand, Result>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobService _blobService;

        public DeleteLogoCommandHandler(
            IProjectRepository projectRepository, 
            IUnitOfWork unitOfWork, 
            IBlobService blobService)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _blobService = blobService;
        }

        public Task<Result> Handle(DeleteLogoCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
