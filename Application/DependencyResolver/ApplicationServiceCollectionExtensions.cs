using Application.UseCases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DependencyResolver
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationDependency(
        this IServiceCollection services)
        {
            services.AddMediatR(typeof(GetUserQuery).Assembly);
            services.AddMediatR(typeof(GetUserQueryHandler).Assembly);
            return services;
        }
    }
}
