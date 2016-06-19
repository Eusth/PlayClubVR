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
using VRGIN.Controls.Tools;

namespace PlayClubStudioVR
{
    class IKTool : Tool
    {
        private static string[] BLACKLIST = new string[] { "XY", "YZ", "XZ", "RingGuidZ", "RingGuidX", "RingGuidY" };
        private readonly static int GIZMO_LAYER = LayerMask.NameToLayer("DriveUI");
        private bool _Dragging = false;
        private TravelDistanceRumble _Rumble;
        private Controller.Lock _Lock;
        private Transform _Target;
        private FieldInfo _GuideDriveManager = typeof(GuideDrive).GetField("manager", BindingFlags.NonPublic | BindingFlags.Instance);

        private Vector3 _PrevPos;
        private Quaternion _PrevRot;
        private GuideDriveManager _Manager;
        private bool _Hover;
        private GameObject _HoverObject;

        public override Texture2D Image
        {
            get
            {
                return UnityHelper.LoadImage("icon_maestro.png");
            }
        }


        protected override void OnStart()
        {
            base.OnStart();
            _Rumble = new TravelDistanceRumble(300, 0.05f, transform);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Owner.StopRumble(_Rumble);
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if(Tracking.isValid)
            {
                var device = Controller;
                if(device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger)) {
                    _HoverObject = FindNearestHandle();
                    if (!_HoverObject) return;

                    _Manager = (_GuideDriveManager.GetValue(_HoverObject.GetComponent<GuideDrive>()) as GuideDriveManager);
                    _PrevPos = transform.position;
                    _PrevRot = transform.rotation;
                    _Dragging = true;
                    Owner.StartRumble(_Rumble);
                }
                if (_HoverObject)
                {
                    if (device.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
                    {
                        _Manager.DriveMovePosition(transform.position - _PrevPos);

                        var rotDiff = (transform.rotation) * Quaternion.Inverse(_PrevRot);
                        _Manager.DriveMoveRotation(Quaternion.Inverse(_Manager.rotRoot.rotation) * rotDiff * _Manager.rotRoot.rotation);

                        _PrevPos = transform.position;
                        _PrevRot = transform.rotation;
                    }
                    if (device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
                    {
                        _Dragging = false;
                        Release();
                        Owner.StopRumble(_Rumble);
                    }
                }
            }
        }

        private GameObject FindNearestHandle()
        {
            var nearest = GameObject.FindObjectsOfType<GuideDrive>().OrderBy(g => Vector3.Distance(g.transform.position, transform.position)).FirstOrDefault();
            return nearest ? nearest.gameObject : null;
        }

        //public void OnTriggerEnter(Collider other)
        //{
        //    try
        //    {

        //        if (other.gameObject.layer == GIZMO_LAYER && !BLACKLIST.Contains(other.name))
        //        {
        //            VRLog.Debug("Enter {0}", other.name);
        //            if (!_Dragging)
        //            {
        //                if (_Rumble != null)
        //                {
        //                    _Rumble.Close();
        //                }
        //                _Rumble = new RumbleSession(500, 100);
        //                Owner.StartRumble(_Rumble);
        //                _Manager = (_GuideDriveManager.GetValue(other.GetComponent<GuideDrive>()) as GuideDriveManager);
        //                _HoverObject = other.gameObject;
        //                _Hover = true;
        //            }
        //        }
        //        if (other.gameObject == _HoverObject)
        //        {
        //            _Hover = true;
        //        }
        //    } catch(Exception e)
        //    {
        //        Logger.Error(e);
        //    }
        //}

        //public void OnTriggerExit(Collider other)
        //{
        //    try
        //    {
        //        if(other.gameObject.layer == GIZMO_LAYER && !_Dragging)
        //        {
        //            _HoverObject = other.gameObject;
        //        }
        //    } catch(Exception e)
        //    {
        //        Logger.Error(e);

        //    }
        //}
        private void Release()
        {
            _HoverObject = null;
        }

        protected override void OnDestroy()
        {
        }
    }
}
