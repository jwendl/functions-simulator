using Microsoft.Extensions.DependencyInjection;
using System;

namespace DeviceSimulationApp
{
    public static class ServiceLocator
    {
        private static IServiceProvider serviceProvider;

        public static void BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            if (serviceProvider == null) BuildServiceProvider();

            return serviceProvider.GetRequiredService<T>();
        }
    }
}
