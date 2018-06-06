using CheatModule.API.Menus;
using System;
using System.IO;
using TMPro;
using UnityEngine;

namespace GolfWithYourCheats.Menus
{
    public class ServerMenu : IMenu
    {
        #region Variables

        private PasswordMenu _PassMenu = null;
        private RoomInfo _Info = null;

        private string steamID = "";

        #endregion Variables

        #region Properties

        public int ID => 2;
        public string Title => "Server Menu";
        public string ButtonName => "Server";

        public bool Visible { get; set; }
        public Rect? Window { get; set; }

        #endregion Properties

        #region Hacks

        private bool AllowJumping = false;
        private byte MaxShots = 13;
        private bool AllowCollision = false;
        private bool EasyAim = false;

#if DEBUG
        private string Key = "";
        private string Value = "";
#endif

        #endregion Hacks

        public void Load()
        {
            Window = new Rect(10, 10, 200, 300);
            Visible = false;
        }

        public void Unload()
        {
            Visible = false;
            Window = null;
        }

        #region Functions

        public void Draw()
        {
            GUILayout.Label("More options will appear if you are in the server or attempting to join!");
            if (GUILayout.Button("Refresh Menu"))
                OnToggleMenu();
            if (_Info != null)
            {
                if (_PassMenu != null)
                {
                    if (GUILayout.Button("Bypass Password"))
                    {
                        MonoBehaviour.print("Password Correct, Joining");
                        PhotonNetwork.JoinRoom(_Info.Name);
                        _PassMenu.noticeLine.GetComponent<Renderer>().enabled = false;
                        _PassMenu.GetComponent<TextMeshPro>().JEJDJONFGCC = "Correct, Connecting";
                    }
                }
            }
            if (PhotonNetwork.room != null)
            {
                AllowJumping = GUILayout.Toggle(AllowJumping, "Allow jumping");

                GUILayout.Label("Max Shots: " + MaxShots);
                MaxShots = (byte)GUILayout.HorizontalSlider(MaxShots, 1, 255);

                AllowCollision = GUILayout.Toggle(AllowCollision, "Allow collision");

                EasyAim = GUILayout.Toggle(EasyAim, "Easy aim");

                if (GUILayout.Button("Remove password"))
                    PhotonNetwork.room.CustomProperties["CustomPassword"] = 0;

                if (GUILayout.Button("Set Config"))
                {
                    PhotonNetwork.room.CustomProperties["AllowJump"] = (AllowJumping ? 1 : 0);
                    PhotonNetwork.room.CustomProperties["MaxShots"] = (int)MaxShots;
                    PhotonNetwork.room.CustomProperties["Collision"] = (AllowCollision ? 1 : 0);
                    PhotonNetwork.room.CustomProperties["EasyAim"] = (EasyAim ? 1 : 0);
                    PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.CustomProperties);
                }

#if DEBUG
                GUILayout.Label("Key");
                Key = GUILayout.TextField(Key);
                GUILayout.Label("Value");
                Value = GUILayout.TextField(Value);
                if (GUILayout.Button("Execute Server"))
                {
                    if (int.TryParse(Value, out int val))
                        PhotonNetwork.room.CustomProperties[Key] = val;
                    else
                        PhotonNetwork.room.CustomProperties[Key] = Value;
                    PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.CustomProperties);
                }
                if (GUILayout.Button("Execute Player"))
                {
                    if (int.TryParse(Value, out int val))
                        PhotonNetwork.player.CustomProperties[Key] = val;
                    else
                        PhotonNetwork.player.CustomProperties[Key] = Value;
                    PhotonNetwork.player.SetCustomProperties(PhotonNetwork.player.CustomProperties);
                }

                if (GUILayout.Button("Dump server"))
                    foreach (object key in PhotonNetwork.room.CustomProperties.Keys)
                        File.AppendAllText(Directory.GetCurrentDirectory() + "/Server.log", key.ToString() + " - " + PhotonNetwork.room.CustomProperties[key].ToString() + Environment.NewLine);
                if (GUILayout.Button("Dump players"))
                {
                    foreach (PhotonPlayer player in PhotonNetwork.otherPlayers)
                    {
                        File.AppendAllText(Directory.GetCurrentDirectory() + "/Players.log", player.NickName + ":" + Environment.NewLine);
                        foreach (object key in player.CustomProperties.Keys)
                            File.AppendAllText(Directory.GetCurrentDirectory() + "/Players.log", key.ToString() + " - " + player.CustomProperties[key] + Environment.NewLine);
                    }
                }
#endif
            }
        }

        public void OnToggleMenu()
        {
            _PassMenu = UnityEngine.Object.FindObjectOfType<PasswordMenu>();
            if (_PassMenu != null)
            {
                if (!_PassMenu.allowCustomRoomName)
                {
                    JoinAGame jag = _PassMenu.lobby.GetComponent<JoinAGame>();
                    _Info = jag.newList[jag.roomNumber];
                }
                else
                {
                    RoomInfo[] roomList = PhotonNetwork.GetRoomList();
                    for (int i = 0; i < roomList.Length; i++)
                    {
                        RoomInfo roomInfo = roomList[i];
                        if (roomInfo.Name == _PassMenu.customRoomName)
                            _Info = roomInfo;
                    }
                }
            }
            if (PhotonNetwork.room != null)
            {
                if (PhotonNetwork.room.CustomProperties != null && PhotonNetwork.room.CustomProperties["SteamLobbyID"] != null && PhotonNetwork.room.CustomProperties["SteamLobbyID"].ToString() != steamID)
                {
                    steamID = PhotonNetwork.room.CustomProperties["SteamLobbyID"].ToString();
                    if (PhotonNetwork.room.CustomProperties.ContainsKey("AllowJump"))
                        AllowJumping = (int)PhotonNetwork.room.CustomProperties["AllowJump"] == 1;
                    if (PhotonNetwork.room.CustomProperties.ContainsKey("MaxShots"))
                        MaxShots = (byte)PhotonNetwork.room.CustomProperties["MaxShots"];
                    if (PhotonNetwork.room.CustomProperties.ContainsKey("Collision"))
                        AllowCollision = (int)PhotonNetwork.room.CustomProperties["Collision"] == 1;
                    if (PhotonNetwork.room.CustomProperties.ContainsKey("EasyAim"))
                        EasyAim = (int)PhotonNetwork.room.CustomProperties["EasyAim"] == 1;
                }
            }
        }

        #endregion Functions
    }
}