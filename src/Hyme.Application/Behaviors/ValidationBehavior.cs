using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Hyme.Application.Commands.Authentication;
using Hyme.Application.DTOs.Response;
using Hyme.Application.Errors;
using MediatR;

namespace Hyme.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
        where TResponse : ResultBase<TResponse>
    {
        private readonly IValidator<TRequest>? _validator;

        public ValidationBehavior(IValidator<TRequest>? validator = null)
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            if (_validator is null)
                return await next();

            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsValid)
                return await next();

            var errors = validationResult.Errors.ConvertAll(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return (dynamic)Result.Fail(errors);
        }
    }
}
