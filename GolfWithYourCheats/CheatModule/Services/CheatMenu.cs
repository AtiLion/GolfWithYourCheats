using CheatModule.API.Menus;
using CheatModule.API.Services;
using System.Linq;
using UnityEngine;

namespace CheatModule.Services
{
    public class CheatMenu : IService
    {
        #region Private Variables

        private Rect _window = new Rect(10, 10, 200, 200);

        private static bool _Visible = false;

        #endregion Private Variables

        #region Public Properties

        public bool Running { get; set; }
        public string Name => "CheatModule Menu";
        public string ID => "cheatmodule_menu";

        public static bool Visible
        {
            get => _Visible;
            set
            {
                foreach (IMenu menu in MenuManager.Menus.Values)
                {
                    if (!menu.Visible || menu.Window == null)
                        continue;

                    menu.OnToggleMenu();
                }

                _Visible = value;
            }
        }

        #endregion Public Properties

        #region Service Functions

        public void Load()
        {
            // Start service
            ServiceManager.Start(this);
        }

        public void OnGUI()
        {
            if (!Visible)
                return;

            _window = GUILayout.Window(0, _window, Main_Window, CheatConfig.Title);

            foreach (IMenu menu in MenuManager.Menus.Values)
            {
                if (!menu.Visible || menu.Window == null)
                    continue;

                menu.Window = GUILayout.Window(menu.ID, (Rect)menu.Window, Side_Window, menu.Title);
            }
        }

        public void OnThreadedUpdate()
        {
        }

        public void Update()
        {
        }

        public void Unload()
        {
            // Stop service
            ServiceManager.Stop(this);
        }

        #endregion Service Functions

        #region Mono Functions

        private void Main_Window(int id)
        {
            foreach (IMenu menu in (from val in MenuManager.Menus.Values orderby val.ID ascending select val))
            {
                if (GUILayout.Button(menu.ButtonName))
                {
                    menu.Visible = !menu.Visible;
                    menu.OnToggleMenu();
                }
            }

            GUI.DragWindow();
        }

        private void Side_Window(int id)
        {
            IMenu menu = MenuManager.Menus.Values.FirstOrDefault(a => a.ID == id);

            if (menu == null)
                return;

            menu.Draw();
            GUI.DragWindow();
        }

        #endregion Mono Functions
    }
}