using StudentFee.Core.Interfaces;
using StudentFee.Infrastructure.Data;
using StudentFee.Infrastructure.Repositories;

namespace StudentFee.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<DbConnectionFactory>();
        services.AddScoped<IStudentRepository,StudentRepository>();
        services.AddScoped<IFeeRepository,FeeRepository>();

        return services;
    }
}