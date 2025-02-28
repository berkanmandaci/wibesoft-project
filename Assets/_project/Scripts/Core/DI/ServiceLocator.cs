using System.Collections.Generic;

namespace WibeSoft.Core.DI
{
    /// <summary>
    /// Service Locator sınıfı - DI Container'a kolay erişim sağlar
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// Servisi al
        /// </summary>
        public static T GetService<T>() where T : class
        {
            return DIContainer.Instance.GetService<T>();
        }

        /// <summary>
        /// Interface'in tüm implementasyonlarını al
        /// </summary>
        public static IEnumerable<T> GetImplementations<T>() where T : class
        {
            return DIContainer.Instance.GetImplementations<T>();
        }

        /// <summary>
        /// Servisi kaydet
        /// </summary>
        public static void RegisterService<T>(T service) where T : class
        {
            DIContainer.Instance.RegisterService(service);
        }

        /// <summary>
        /// Servisi kaldır
        /// </summary>
        public static void UnregisterService<T>() where T : class
        {
            DIContainer.Instance.UnregisterService<T>();
        }
    }
} 