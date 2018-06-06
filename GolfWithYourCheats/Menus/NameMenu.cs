using CheatModule.API.Menus;
using UnityEngine;

namespace GolfWithYourCheats.Menus
{
    public class NameMenu : IMenu
    {
        #region Variables

        private string _Name = "";

        #endregion Variables

        #region Properties

        public int ID => 3;
        public string Title => "Name Menu";
        public string ButtonName => "Name";

        public bool Visible { get; set; }
        public Rect? Window { get; set; }

        #endregion Properties

        public void Load()
        {
            Visible = false;
            Window = new Rect(10, 10, 200, 300);
        }

        public void Unload()
        {
            Visible = false;
            Window = null;
        }

        #region Functions

        public void Draw()
        {
            GUILayout.Label("Player name:");
            _Name = GUILayout.TextField(_Name);
            if (GUILayout.Button("Set name"))
            {
                PlayerPrefs.SetString("playerName", _Name);
                PhotonNetwork.playerName = _Name;
            }
        }

        public void OnToggleMenu()
        {
            _Name = PlayerPrefs.GetString("playerName");
        }

        #endregion Functions
    }
}