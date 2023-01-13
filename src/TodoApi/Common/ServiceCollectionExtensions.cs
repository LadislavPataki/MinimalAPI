using System.Reflection;

namespace TodoApi.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpoints = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(IEndpoint).IsAssignableFrom(x) 
                        && x is { IsInterface: false, IsAbstract: false });

        foreach (var endpoint in endpoints)
        {
            services.AddScoped(typeof(IEndpoint), endpoint);
        }

        return services;
    }
    
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        var modules = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(IModule).IsAssignableFrom(x) 
                        && x is { IsInterface: false, IsAbstract: false });

        foreach (var module in modules)
        {
            var instance = Activator.CreateInstance(module) as IModule;
            instance?.AddModule(services, configuration);
            
            // using static method
            // var addModuleName = nameof(IModule.AddModule);
            // var methodInfo = module.GetMethod(addModuleName, BindingFlags.Public | BindingFlags.Static);
            // methodInfo?.Invoke(null, new object?[] { services, configuration });
        }

        return services;
    }
}