using CheatModule.API.Interfaces;
using UnityEngine;

namespace CheatModule.API.Menus
{
    public interface IMenu : ILoadable
    {
        #region Properties

        int ID { get; }
        string Title { get; }
        string ButtonName { get; }

        bool Visible { get; set; }
        Rect? Window { get; set; }

        #endregion Properties

        #region Functions

        void Draw();

        void OnToggleMenu();

        #endregion Functions
    }
}