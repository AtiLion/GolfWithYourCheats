using System;
using System.Collections.Generic;

namespace CheatModule.API.Menus
{
    public static class MenuManager
    {
        #region Public Properties

        public static Dictionary<Type, IMenu> Menus { get; private set; } = new Dictionary<Type, IMenu>();

        #endregion Public Properties

        #region Public Functions

        public static bool IsMenu(Type menu) =>
            typeof(IMenu).IsAssignableFrom(menu) && menu != typeof(IMenu);

        public static bool IsMenu<T>() where T : IMenu =>
            IsMenu(typeof(T));

        public static bool IsLoaded(Type menu) =>
            Menus.ContainsKey(menu);

        public static bool IsLoaded<T>() where T : IMenu =>
            IsLoaded(typeof(T));

        public static IMenu LoadClass(Type menu)
        {
            if (!IsMenu(menu) || IsLoaded(menu))
                return null;

            try
            {
                Logging.Log("Loading menu " + menu.Name + "...");

                IMenu instance = (IMenu)Activator.CreateInstance(menu);

                instance.Load();
                Menus.Add(menu, instance);

                Logging.Log("Loaded menu " + menu.Name + "!");
                return instance;
            }
            catch (Exception ex)
            {
                Logging.LogError("Error loading menu " + menu, ex);
            }

            return null;
        }

        public static T LoadClass<T>() where T : IMenu =>
            (T)LoadClass(typeof(T));

        public static bool UnloadClass(Type menu)
        {
            if (!IsMenu(menu) || !IsLoaded(menu))
                return false;

            try
            {
                Logging.Log("Unloading menu " + menu.Name + "...");

                Menus[menu].Unload();
                Menus.Remove(menu);

                Logging.Log("Unloaded menu " + menu.Name + "!");
                return true;
            }
            catch (Exception ex)
            {
                Logging.LogError("Error unloading menu " + menu.Name, ex);
            }

            return false;
        }

        public static bool UnloadClass<T>() where T : IMenu =>
            UnloadClass(typeof(T));

        public static bool UnloadClass(IMenu menu) =>
            UnloadClass(menu.GetType());

        public static IMenu GetClass(Type menu)
        {
            if (!IsMenu(menu) || !IsLoaded(menu))
                return null;

            return Menus[menu];
        }

        public static T GetClass<T>() where T : IMenu =>
            (T)GetClass(typeof(T));

        public static bool ShowMenu(IMenu menu)
        {
            if (menu.Window == null || menu.Visible)
                return false;

            menu.Visible = true;
            return true;
        }

        public static bool ShowMenu<T>() where T : IMenu =>
            ShowMenu(typeof(T));

        public static bool ShowMenu(Type menu)
        {
            if (!IsMenu(menu) || !IsLoaded(menu))
                return false;

            return ShowMenu(Menus[menu]);
        }

        public static bool HideMenu(IMenu menu)
        {
            if (menu.Window == null || !menu.Visible)
                return false;

            menu.Visible = false;
            return true;
        }

        public static bool HideMenu<T>() where T : IMenu =>
            HideMenu(typeof(T));

        public static bool HideMenu(Type menu)
        {
            if (!IsMenu(menu) || !IsLoaded(menu))
                return false;

            return HideMenu(Menus[menu]);
        }

        #endregion Public Functions
    }
}