using CheatModule.API;
using CheatModule.API.Services;
using CheatModule.Services;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace GolfWithYourCheats
{
    public class GolfWithYourCheats : MonoBehaviour, IService
    {
        #region Game Data

        public static GameObject Scripts = null;
        public static Menu Menu = null;
        public static MainMenu MainMenu = null;
        public static GameObject PlayerObject = null;
        public static BallMovement BallMovement = null;

        #endregion Game Data

        #region Public Properties

        public bool Running { get; set; }
        public string Name => "GolfWithYourCheats";
        public string ID => "golfwithyourcheats";

        #endregion Public Properties

        #region Mono Functions

        public void Start()
        {
            // Set the game data
            Scripts = GameObject.Find("_Scripts");
        }

        private void FixedUpdate()
        {
            if (Scripts == null)
            {
                Scripts = GameObject.Find("_Scripts");
            }
            else
            {
                if (Menu == null)
                    Menu = Scripts.GetComponent<Menu>();
                if (PlayerObject == null && Menu != null)
                    PlayerObject = Menu.playerBall;
                if (BallMovement == null && PlayerObject != null)
                    BallMovement = PlayerObject.GetComponent<BallMovement>();
            }
        }

        public void OnGUI()
        {
        }

        public void Update()
        {
            if (cInput.GetButton("Leaderboard"))
            {
                CheatMenu.Visible = false;
                return;
            }

            if (Input.GetKeyDown(KeyCode.F1))
                CheatMenu.Visible = !CheatMenu.Visible;
        }

        #endregion Mono Functions

        #region Service Functions

        public void OnThreadedUpdate()
        {
        }

        public void Load()
        {
            InjectionDetector.Dispose(); // Disable injection detector
            ObscuredCheatingDetector.Dispose(); // Disable obscured cheating detector
            SpeedHackDetector.Dispose(); // Disable speedhack detector
            WallHackDetector.Dispose(); // Disable wallhack detector

            Logging.LogImportant("Codestage disabled ;D");
        }

        public void Unload()
        {
        }

        #endregion Service Functions
    }
}