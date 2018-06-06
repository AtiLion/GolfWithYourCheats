using CheatModule.API;
using CheatModule.API.Menus;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GolfWithYourCheats.Menus
{
    public class PlayerMenu : IMenu
    {
        #region Properties

        public int ID => 1;
        public string Title => "Player Menu";
        public string ButtonName => "Player";

        public bool Visible { get; set; }
        public Rect? Window { get; set; }

        #endregion Properties

        #region Hacks

        public bool HoleInOne = false;
        public bool SelfMoving = false;
        public bool UnlimitedFreecam = false;

        #endregion Hacks

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
            GUILayout.Label("More options will appear if you are playing the game!");
            if (GolfWithYourCheats.PlayerObject == null)
                return;
            HoleInOne = GUILayout.Toggle(HoleInOne, "Hole in one");
            if (GUILayout.Button("Hit hole"))
            {
                if (GolfWithYourCheats.BallMovement.hitCounter < 1)
                    GolfWithYourCheats.BallMovement.hitCounter = 1;
                GolfWithYourCheats.BallMovement.inHole = true;
            }
            if (GUILayout.Button("Stop moving(G)"))
            {
                Rigidbody rbody = (Rigidbody)typeof(BallMovement).GetFieldB("JKAJPEOLPAI", BindingFlags.NonPublic | BindingFlags.Instance, true).GetValue(GolfWithYourCheats.BallMovement);

                if (rbody != null)
                    rbody.velocity = Vector3.zero;
            }
            if (GUILayout.Button("Reset hits"))
                GolfWithYourCheats.BallMovement.hitCounter = 0;
            SelfMoving = GUILayout.Toggle(SelfMoving, "Self moving");
            if (GUILayout.Button("Reset to last shot(R)"))
                GolfWithYourCheats.BallMovement.ResetToLastShot();
            if (GUILayout.Button((UnlimitedFreecam ? "Disable" : "Enable") + " unlimited freecam"))
            {
                UnlimitedFreecam = !UnlimitedFreecam;
                if (UnlimitedFreecam)
                    typeof(Menu).GetFieldB("GDOEFDDLMJA", BindingFlags.NonPublic | BindingFlags.Instance, true).SetValue(GolfWithYourCheats.Menu, 9999);
                else
                    typeof(Menu).GetFieldB("GDOEFDDLMJA", BindingFlags.NonPublic | BindingFlags.Instance, true).SetValue(GolfWithYourCheats.Menu, 15);
            }
            if (GUILayout.Button("Reset freecam timer"))
                typeof(Menu).GetFieldB("GDOEFDDLMJA", BindingFlags.NonPublic | BindingFlags.Instance, true).SetValue(GolfWithYourCheats.Menu, 15);
            if (GUILayout.Button("Teleport ball to camera(H)"))
                GolfWithYourCheats.BallMovement.ResetToSpawn(Camera.main.transform.position);

#if DEBUG
            if (GUILayout.Button("Dump player"))
                foreach (object key in PhotonNetwork.player.CustomProperties.Keys)
                    File.AppendAllText(Directory.GetCurrentDirectory() + "/Player.log", key.ToString() + " - " + PhotonNetwork.room.CustomProperties[key].ToString() + Environment.NewLine);
#endif
        }

        public void OnToggleMenu()
        {
        }

        #endregion Functions
    }
}