using Microsoft.Extensions.DependencyInjection;

namespace OptimizeLocationSearch
{
    public static class InjectLocationServiceManager
    {
        public static void AsSingleton<T>(this IServiceCollection services) where T : class
        {
            services.AddSingleton<ILocationServiceManager<T>, LocationServiceManager<T>>();
        }
    }
}