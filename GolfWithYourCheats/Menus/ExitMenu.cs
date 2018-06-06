using CheatModule.API.Menus;
using CheatModule.Services;
using UnityEngine;

namespace GolfWithYourCheats.Menus
{
    public class ExitMenu : IMenu
    {
        #region Properties

        public int ID => 4;
        public string Title => "";
        public string ButtonName => "Close";

        public bool Visible { get; set; }
        public Rect? Window { get; set; }

        #endregion Properties

        public void Load()
        {
            Visible = false;
            Window = null;
        }

        public void Unload()
        {
            Visible = false;
            Window = null;
        }

        #region Functions

        public void Draw()
        {
        }

        public void OnToggleMenu()
        {
            Visible = false;
            CheatMenu.Visible = false;
        }

        #endregion Functions
    }
}