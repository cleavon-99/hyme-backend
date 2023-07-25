using Azure.Storage.Blobs;
using Hyme.Application.Services;
using Hyme.Domain.Repositories;
using Hyme.Infrastructure.Data;
using Hyme.Infrastructure.Data.Repositories;
using Hyme.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hyme.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfractructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSingleton<IWalletValidationService, WalletValidationService>();
            services.AddSingleton<ITokenGenerator, TokenGenerator>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IWhitelistRepository, WhiteListRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton(option => new BlobServiceClient(configuration["AzureBlobStorageConnection"]));
            services.AddSingleton<IBlobService, BlobService>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}
