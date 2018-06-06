using System;
using System.Collections.Generic;
using UnityEngine;

namespace CheatModule.API.Services
{
    public static class ServiceManager
    {
        #region Public Properties

        public static Dictionary<Type, IService> Services { get; private set; } = new Dictionary<Type, IService>();

        #endregion Public Properties

        #region Public Functions

        public static bool IsService(Type service) =>
            typeof(IService).IsAssignableFrom(service) && service != typeof(IService);

        public static bool IsService<T>() where T : IService =>
            IsService(typeof(T));

        public static bool IsLoaded(Type service) =>
            Services.ContainsKey(service);

        public static bool IsLoaded<T>() where T : IService =>
            IsLoaded(typeof(T));

        public static bool IsMonoBehaviour(Type service) =>
            typeof(MonoBehaviour).IsAssignableFrom(service) && service != typeof(MonoBehaviour);

        public static bool IsMonoBehaviour<T>() where T : IService =>
            IsMonoBehaviour(typeof(T));

        public static IService LoadClass(Type service)
        {
            if (!IsService(service) || IsLoaded(service))
                return null;

            try
            {
                Logging.Log("Loading service " + service.Name + "...");

                IService instance;
                if (IsMonoBehaviour(service))
                    instance = (IService)CheatLoader.MainGameObject.AddComponent(service);
                else
                    instance = (IService)Activator.CreateInstance(service);

                instance.Load();
                Services.Add(service, instance);

                Logging.Log("Loaded service " + service.Name + "!");
                return instance;
            }
            catch (Exception ex)
            {
                Logging.LogError("Error loading service " + service.Name, ex);
            }

            return null;
        }

        public static T LoadClass<T>() where T : IService =>
            (T)LoadClass(typeof(T));

        public static bool UnloadClass(Type service)
        {
            if (!IsService(service) || !IsLoaded(service))
                return false;

            try
            {
                Logging.Log("Unloading service " + service.Name + "...");

                Services[service].Unload();
                if (IsMonoBehaviour(service))
                    CheatLoader.Destroy((MonoBehaviour)Services[service]);
                Services.Remove(service);

                Logging.Log("Unloaded service " + service.Name + "!");
                return true;
            }
            catch (Exception ex)
            {
                Logging.LogError("Error unloading service " + service.Name, ex);
            }

            return false;
        }

        public static bool UnloadClass<T>() where T : IService =>
            UnloadClass(typeof(T));

        public static bool UnloadClass(IService service) =>
            UnloadClass(service.GetType());

        public static IService GetClass(Type service)
        {
            if (!IsService(service) || !IsLoaded(service))
                return null;

            return Services[service];
        }

        public static T GetClass<T>() where T : IService =>
            (T)GetClass(typeof(T));

        public static bool Start(IService service)
        {
            if (service.Running)
                return false;

            try
            {
                service.Running = true;

                return true;
            }
            catch (Exception ex)
            {
                Logging.LogError("Error starting service " + service.GetType().Name, ex);
            }

            return false;
        }

        public static bool Start<T>() where T : IService =>
            Start(typeof(T));

        public static bool Start(Type service)
        {
            if (!IsService(service) || !IsLoaded(service))
                return false;

            return Start(Services[service]);
        }

        public static bool Stop(IService service)
        {
            if (!service.Running)
                return false;

            try
            {
                service.Running = false;

                return true;
            }
            catch (Exception ex)
            {
                Logging.LogError("Error stopping service " + service.GetType().Name, ex);
            }

            return false;
        }

        public static bool Stop<T>() where T : IService =>
            Stop(typeof(T));

        public static bool Stop(Type service)
        {
            if (!IsService(service) || !IsLoaded(service))
                return false;

            return Stop(Services[service]);
        }

        #endregion Public Functions
    }
}