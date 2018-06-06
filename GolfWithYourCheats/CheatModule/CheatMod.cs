using CheatModule.API.Detours;
using CheatModule.API.Menus;
using CheatModule.API.Services;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace CheatModule
{
    public class CheatMod : MonoBehaviour
    {
        #region Private Variables

        private Thread _ticker;

        #endregion Private Variables

        #region Mono Functions

        private void Start()
        {
            foreach (Type _class in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!_class.IsClass)
                    continue;

                // Service and menu checks
                if (MenuManager.IsMenu(_class))
                    MenuManager.LoadClass(_class);
                else if (ServiceManager.IsService(_class))
                {
                    IService service = ServiceManager.LoadClass(_class);

                    ServiceManager.Start(service);
                }

                // Detour checks
                foreach (MethodInfo method in _class.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    DetourAttribute detour = (DetourAttribute)Attribute.GetCustomAttribute(method, typeof(DetourAttribute));
                    if (detour != null)
                    {
                        if (detour.Modified == null)
                            detour.Modified = method;

                        DetourManager.Detour(detour.Original, detour.Modified);
                    }
                }
            }

            _ticker = new Thread(new ThreadStart(Tick));
            _ticker.Start();
        }

        private void Update()
        {
            lock (ServiceManager.Services)
            {
                foreach (IService service in ServiceManager.Services.Values)
                {
                    if (!service.Running || ServiceManager.IsMonoBehaviour(service.GetType()))
                        continue;

                    service.Update();
                }
            }
        }

        private void OnGUI()
        {
            lock (ServiceManager.Services)
            {
                foreach (IService service in ServiceManager.Services.Values)
                {
                    if (!service.Running || ServiceManager.IsMonoBehaviour(service.GetType()))
                        continue;

                    service.OnGUI();
                }
            }
        }

        #endregion Mono Functions

        #region Thread Functions

        private void Tick()
        {
            lock (ServiceManager.Services)
            {
                foreach (IService service in ServiceManager.Services.Values)
                {
                    if (!service.Running)
                        continue;

                    service.OnThreadedUpdate();
                }
            }
        }

        #endregion Thread Functions
    }
}