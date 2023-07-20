using FluentResults;
using FluentValidation;
using Hyme.Application.Behaviors;
using Hyme.Application.Commands.Authentication;
using Hyme.Application.DTOs.Response;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Hyme.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMediatR(g => g.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped(typeof(IPipelineBehavior<,>) , typeof(ValidationBehavior<,>));        
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
