using CheatModule.API;
using CheatModule.API.Menus;
using CheatModule.API.Services;
using GolfWithYourCheats.Menus;
using System.Reflection;
using UnityEngine;

namespace GolfWithYourCheats.Services
{
    public class PlayerService : IService
    {
        #region Variables

        private PlayerMenu _PlayerMenu = null;

        private FieldInfo fi_Rigidbody = null;

        private float _travelVelocity = 1f;
        private byte _updateStep = 0;
        private byte _maxUpdateSteps = 20;

        #endregion Variables

        #region Properties

        public bool Running { get; set; }

        public string Name => "Player Service";
        public string ID => "PlayerService";

        #endregion Properties

        public void Load()
        {
            fi_Rigidbody = typeof(BallMovement).GetFieldB("JKAJPEOLPAI", BindingFlags.NonPublic | BindingFlags.Instance, true);
            ServiceManager.Start(this);
        }

        public void OnGUI()
        {
        }

        public void OnThreadedUpdate()
        {
        }

        public void Update()
        {
            if (_PlayerMenu == null)
                _PlayerMenu = MenuManager.GetClass<PlayerMenu>();
            if (GolfWithYourCheats.PlayerObject == null)
                return;
            if (_PlayerMenu.HoleInOne && GolfWithYourCheats.BallMovement.hitCounter > 1)
                GolfWithYourCheats.BallMovement.hitCounter = 1;
            if (_PlayerMenu.SelfMoving)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    ((Rigidbody)fi_Rigidbody.GetValue(GolfWithYourCheats.BallMovement)).velocity = Camera.main.transform.forward * _travelVelocity;
                    if (_updateStep >= _maxUpdateSteps)
                    {
                        _travelVelocity += 0.5f;
                        _updateStep = 0;
                    }
                    else
                    {
                        _updateStep++;
                    }
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    ((Rigidbody)fi_Rigidbody.GetValue(GolfWithYourCheats.BallMovement)).velocity = (-Camera.main.transform.right) * _travelVelocity;
                    if (_updateStep >= _maxUpdateSteps)
                    {
                        _travelVelocity += 0.5f;
                        _updateStep = 0;
                    }
                    else
                    {
                        _updateStep++;
                    }
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    ((Rigidbody)fi_Rigidbody.GetValue(GolfWithYourCheats.BallMovement)).velocity = Camera.main.transform.right * _travelVelocity;
                    if (_updateStep >= _maxUpdateSteps)
                    {
                        _travelVelocity += 0.5f;
                        _updateStep = 0;
                    }
                    else
                    {
                        _updateStep++;
                    }
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    ((Rigidbody)fi_Rigidbody.GetValue(GolfWithYourCheats.BallMovement)).velocity = (-Camera.main.transform.forward) * _travelVelocity;
                    if (_updateStep >= _maxUpdateSteps)
                    {
                        _travelVelocity += 0.5f;
                        _updateStep = 0;
                    }
                    else
                    {
                        _updateStep++;
                    }
                }
                else
                {
                    _travelVelocity = 1f;
                    _updateStep = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                Rigidbody rbody = (Rigidbody)typeof(BallMovement).GetFieldB("JKAJPEOLPAI", BindingFlags.NonPublic | BindingFlags.Instance, true).GetValue(GolfWithYourCheats.BallMovement);

                if (rbody != null)
                    rbody.velocity = Vector3.zero;
            }
            if (Input.GetKeyDown(KeyCode.R))
                GolfWithYourCheats.BallMovement.ResetToLastShot();
            if (Input.GetKeyDown(KeyCode.H))
                GolfWithYourCheats.BallMovement.ResetToSpawn(Camera.main.transform.position);
        }

        public void Unload()
        {
            _PlayerMenu = null;
            ServiceManager.Stop(this);
        }
    }
}