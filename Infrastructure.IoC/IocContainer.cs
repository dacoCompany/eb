using Microsoft.Practices.Unity.Configuration;
using Unity;

namespace Infrastructure.IoC
{
    public class IocContainer
    {
        private static IUnityContainer container;

        static IocContainer()
        {
            if (container == null)
            {
                container = new UnityContainer();
                container.LoadConfiguration();
            }
        }

        public static IUnityContainer GetContainer()
        {
            return container;
        }

        public static T GetInstance<T>()
        {
            return container.Resolve<T>();
        }
    }
}
