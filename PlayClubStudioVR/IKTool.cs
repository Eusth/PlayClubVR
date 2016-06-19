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
    static class IKCtrl
    {
        private static bool _Ik = true;


        internal static void Change(bool state)
        {
            foreach (var actor in VR.Interpreter.Actors.OfType<StudioActor>())
            {
                bool ikState = state && _Ik;
                bool fkState = state && !_Ik;

                //if (ikState)
                //{
                //    actor.Actor.ikCtrl.SetActive(ikState);
                //}
                //if(fkState) {
                //    actor.Actor.fkCtrl.SetActive(fkState);
                //}

                //actor.Actor.ikCtrl.SetActiveTargets(ikState);
                //actor.Actor.fkCtrl.SetActiveTargets(fkState);
                
                //if(ikState)
                //{
                //    actor.Actor.ikCtrl.SetupIK();
                //}
                //if(fkState)
                //{
                //    actor.Actor.fkCtrl.SetupFK();
                //}

                bool isActive = actor.Actor.fkCtrl.IsActive;

                if (state)
                {
                    actor.Actor.ikCtrl.SetActive(ikState);
                    actor.Actor.fkCtrl.SetActive(state);
                }
                
                actor.Actor.ikCtrl.SetActiveTargets(ikState);
                actor.Actor.fkCtrl.SetActiveTargets(fkState);

                if (ikState)
                {
                    actor.Actor.ikCtrl.SetupIK();
                    if (!isActive)
                    {
                        actor.Actor.fkCtrl.SetupFK();
                    }
                }
                else if (fkState)
                {
                    actor.Actor.fkCtrl.SetupFK();
                }

            }
        }

        internal static void Enable()
        {
            Change(true);
        }

        internal static void Disable()
        {
            Change(false);
        }

        internal static void Toggle()
        {
            _Ik = !_Ik;
            Change(true);
        }
    }
    class IKTool : Tool
    {
        private const float MAX_DISTANCE = 0.5f;

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

        public override List<HelpText> GetHelpTexts()
        {
            return new List<HelpText>(new HelpText[] {
                HelpText.Create("Switch FK/IK", FindAttachPosition("trackpad"), new Vector3(0.07f, 0.02f, 0.05f)),
                HelpText.Create("Grab", FindAttachPosition("trigger"), new Vector3(0.06f, 0.04f, -0.05f)),
            });
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

            if(!(Owner.Other.ActiveTool is IKTool))
            {
                IKCtrl.Disable();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            IKCtrl.Enable();
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
                if(device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad))
                {
                    IKCtrl.Toggle();
                }
            }
        }

        private GameObject FindNearestHandle()
        {
            var nearest = GameObject.FindObjectsOfType<GuideDrive>().OrderBy(g => Vector3.Distance(g.transform.position, transform.position)).FirstOrDefault();
            if(nearest)
            {
                if (Vector3.Distance(nearest.transform.position, transform.position) <= MAX_DISTANCE)
                {
                    return nearest.gameObject;
                }
            }
            return null;
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
