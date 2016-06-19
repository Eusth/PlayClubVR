using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Valve.VR;
using VRGIN.Core;
using VRGIN.Controls;
using VRGIN.Helpers;
using VRGIN.Modes;
using GamePadClub;
using XInputDotNetPure;

namespace PlayClubVR
{
    public class PlayClubSeatedMode : SeatedMode
    {
        private FieldInfo _IllusionCameraRotation = typeof(IllusionCamera).GetField("rotate", BindingFlags.NonPublic | BindingFlags.Instance);
        private IllusionCamera _IllusionCamera;

        protected override void OnEnable()
        {
            base.OnEnable();
            Logger.Info("Enter seated mode");
            GamePadController.Instance.Register(HandleInput);

        }
        protected override void OnDisable()
        {
            base.OnDisable();

            Logger.Info("Leave seated mode");
            GamePadController.Instance.Unregister(HandleInput);

        }

        bool HandleInput(GamePadState nowState, GamePadState prevState)
        {
            if (GamePadHelper.IsPressUp(nowState.Buttons.Start, prevState.Buttons.Start))
            {
                Recenter();
            }
            
            if (nowState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                Vector2 rightStick = new Vector2(nowState.ThumbSticks.Right.X, nowState.ThumbSticks.Right.Y);
                Vector2 leftStick = new Vector2(nowState.ThumbSticks.Left.X, nowState.ThumbSticks.Left.Y);
                if(rightStick.magnitude > 0.1f)
                {
                    VR.Settings.Rotation += rightStick.x * Time.deltaTime * 50f;
                    VR.Settings.OffsetY += rightStick.y * Time.deltaTime * 0.1f;

                }
                if (leftStick.magnitude > 0.1f)
                {
                    VR.Settings.Distance += leftStick.x * Time.deltaTime * 0.1f;
                    VR.Settings.Angle += leftStick.y * Time.deltaTime * 50f;
                }

                if(nowState.DPad.Up == ButtonState.Pressed)
                {
                    VR.Settings.IPDScale += Time.deltaTime * 0.1f;
                } else if(nowState.DPad.Down == ButtonState.Pressed)
                {
                    VR.Settings.IPDScale -= Time.deltaTime * 0.1f;
                }

                // Impersonate
                if (GamePadHelper.IsPressUp(nowState.Buttons.Y, prevState.Buttons.Y))
                {
                    if (LockTarget == null || !LockTarget.IsValid) {
                        Impersonate(VR.Interpreter.Actors.FirstOrDefault());
                    } else {
                        Impersonate(null);
                    }
                }

            }
            
            return false;
        }


        public override ETrackingUniverseOrigin TrackingOrigin
        {
            get
            {
                return ETrackingUniverseOrigin.TrackingUniverseSeated;
            }
        }


        public override IEnumerable<Type> Tools
        {
            get
            {
                var tools = base.Tools;
                if (MaestroTool.IsAvailable)
                {
                    tools = tools.Concat(new Type[] { typeof(MaestroTool) });
                }
                return tools;
            }
        }

        protected override void ChangeModeOnControllersDetected()
        {
            ChangeMode();
        }

        protected virtual void ChangeMode()
        {
            VR.Manager.SetMode<PlayClubStandingMode>();
        }

        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl + C"), new KeyStroke("Ctrl + C"), ChangeMode ),
            });
        }

        protected override void OnStart()
        {
            base.OnStart();
            _IllusionCamera = FindObjectOfType<IllusionCamera>();

        }

        protected override void OnLevel(int level)
        {
            base.OnLevel(level);
            _IllusionCamera = FindObjectOfType<IllusionCamera>();

        }

        protected override void CorrectRotationLock()
        {
            if (_IllusionCamera)
            {
                var my = VR.Camera.SteamCam.origin;

                _IllusionCameraRotation.SetValue(_IllusionCamera, my.eulerAngles);

                Vector3 b = my.rotation * (Vector3.back * _IllusionCamera.Distance);
                my.position = _IllusionCamera.Focus + b;
            }
        }

        protected override void SyncCameras()
        {
            if (_IllusionCamera)
            {
                var my = VR.Camera.SteamCam.origin;

                _IllusionCamera.Set(
                    my.position + my.forward,
                    Quaternion.LookRotation(my.forward, my.up).eulerAngles,
                1);
            }
        }
    }
}
