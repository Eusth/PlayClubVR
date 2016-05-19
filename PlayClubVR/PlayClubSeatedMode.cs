using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Valve.VR;
using VRGIN.Core;
using VRGIN.Core.Controls;
using VRGIN.Core.Helpers;
using VRGIN.Core.Modes;

namespace PlayClubVR
{
    class PlayClubSeatedMode : SeatedMode
    {
        private static bool _ControllerFound = false;
        private FieldInfo _IllusionCameraRotation = typeof(IllusionCamera).GetField("rotate", BindingFlags.NonPublic | BindingFlags.Instance);
        private IllusionCamera _IllusionCamera;
        protected void OnEnable()
        {
            Logger.Info("Enter seated mode");
            SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
        }
        protected void OnDisable()
        {
            Logger.Info("Leave seated mode");
            SteamVR_Utils.Event.Remove("device_connected", OnDeviceConnected);

        }

        public override ETrackingUniverseOrigin TrackingOrigin
        {
            get
            {
                return ETrackingUniverseOrigin.TrackingUniverseSeated;
            }
        }

        private void OnDeviceConnected(object[] args)
        {
            if (!_ControllerFound)
            {
                var index = (uint)(int)args[0];
                var connected = (bool)args[1];
                Logger.Info("Device connected: {0}", index);

                if (connected && index > OpenVR.k_unTrackedDeviceIndex_Hmd)
                {
                    var system = OpenVR.System;
                    if (system != null && system.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller)
                    {
                        _ControllerFound = true;

                        // Switch to standing mode
                        if(VR.Manager.Mode is PlayClubSeatedMode)
                        {
                            VR.Manager.SetMode<PlayClubStandingMode>();
                        }
                    }
                }
            }
        }

        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl + C"), new KeyStroke("Ctrl + C"), delegate { VR.Manager.SetMode<PlayClubStandingMode>(); } ),
            });
        }

        protected override void OnStart()
        {
            base.OnStart();

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
