using Microsoft.Extensions.DependencyInjection;
using System;

namespace DeviceSimulation.Lexer
{
    public static class ServiceLocator
    {
        private static IServiceProvider serviceProvider;

        public static void BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IStateService, StateService>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            return serviceProvider.GetRequiredService<T>();
        }
    }
}
