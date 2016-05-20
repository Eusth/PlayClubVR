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

namespace PlayClubStudioVR
{
    class GripHandler : ProtectedBehaviour
    {
        private readonly static int GIZMO_LAYER = LayerMask.NameToLayer("DriveUI");
        private Controller _Controller;
        private RumbleSession _Rumble;
        private Controller.Lock _Lock;
        private Transform _Target;
        private FieldInfo _GuideDriveManager = typeof(GuideDrive).GetField("manager", BindingFlags.NonPublic | BindingFlags.Instance);

        private Vector3 _PrevPos;
        private Quaternion _PrevRot;
        private Quaternion _StartRot;
        private GuideDriveManager _Manager;


        protected override void OnStart()
        {
            base.OnStart();
            _Controller = GetComponent<Controller>();
        }

        private bool HasFocus(bool takeFocus = true)
        {
            bool validLock = _Lock != null && _Lock.IsValid;
            if(takeFocus && !validLock)
            {
                return _Controller.AcquireFocus(out _Lock);
            }
            return validLock;
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if(HasFocus(false) && _Controller.Tracking.isValid)
            {
                var device = GetInput();
                if(device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger)) {
                    _PrevPos = transform.position;
                    _StartRot = _Manager.rotRoot.localRotation;
                    _PrevRot = transform.rotation;
                }
                if (device.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
                {

                    _Manager.DriveMovePosition(transform.position - _PrevPos);

                    var rotDiff = (transform.rotation) * Quaternion.Inverse(_PrevRot);
                    _Manager.DriveMoveRotation(Quaternion.Inverse(_Manager.rotRoot.rotation) * rotDiff * _Manager.rotRoot.rotation);
                    
                    _PrevPos = transform.position;
                    _PrevRot = transform.rotation;
                }
            }
        }

        private SteamVR_Controller.Device GetInput()
        {
            return SteamVR_Controller.Input((int)_Controller.Tracking.index);
        }

        public void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == GIZMO_LAYER && HasFocus())
            {
                try
                {
                    if (_Rumble != null)
                    {
                        _Rumble.Close();
                    }
                    _Rumble = new RumbleSession(500, 100);
                    _Controller.StartRumble(_Rumble);
                    _Manager = (_GuideDriveManager.GetValue(other.GetComponent<GuideDrive>()) as GuideDriveManager);
                    Logger.Info("Started rumbling");

                } catch(Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if(other.gameObject.layer == GIZMO_LAYER && HasFocus())
            {
                Logger.Info("Ended rumbling");

                _Rumble.Close();
                _Rumble = null;
                _Lock.Release();
            }
        }
    }
}
