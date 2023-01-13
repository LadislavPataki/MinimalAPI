namespace TodoApi.Common;

public interface IModule
{
    IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration);
}