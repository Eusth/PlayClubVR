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
using UnityEngine.UI;

namespace PlayClubStudioVR
{
    static class IKCtrl
    {
        public enum IKVisibility
        {
            Deactivated,
            Hidden,
            Visible
        }
        public enum IKState
        {
            FK,
            IK
        }

        public static IKState State { get { return _State; } private set { _State = value; } }
        private static IKState _State = IKState.FK;
        public static IKVisibility Visibility { get { return _Visibility; } private set { _Visibility = value; } }
        private static IKVisibility _Visibility = IKVisibility.Deactivated;


        public static event EventHandler StateChanged = delegate { };


        internal static void Change(IKVisibility _visibility, IKState _state)
        {
            Visibility = _visibility;
            State = _state;

            foreach (var actor in VR.Interpreter.Actors.OfType<StudioActor>())
            {
                bool ikState = Visibility == IKVisibility.Visible && State == IKState.IK;
                bool fkState = Visibility == IKVisibility.Visible && State == IKState.FK;
                
                bool isActive = actor.Actor.fkCtrl.IsActive;

                if (Visibility == IKVisibility.Visible || Visibility == IKVisibility.Deactivated)
                {
                    actor.Actor.ikCtrl.SetActive(ikState);
                    actor.Actor.fkCtrl.SetActive(ikState || fkState);
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

            StateChanged(null, null);
        }

        internal static void Show()
        {
            if(Visibility == IKVisibility.Hidden)
            {
                Change(IKVisibility.Visible, State);
            }
        }

        internal static void Enable()
        {
            if (Visibility == IKVisibility.Deactivated)
            {
                Change(IKVisibility.Visible, State);
            }
        }

        internal static void Disable()
        {
            if (Visibility > IKVisibility.Deactivated)
            {
                Change(IKVisibility.Deactivated, State);
            }
        }

        internal static void Hide()
        {
            if (Visibility > IKVisibility.Hidden)
            {
                Change(IKVisibility.Hidden, State);
            }
        }

        internal static void Toggle()
        {  
            var newState = (IKState)(((int)State + 1) % Enum.GetValues(typeof(IKState)).Length);
            Change(Visibility, newState);
        }
    }
    class IKTool : Tool
    {
        
        private class IKStateReactor : ProtectedBehaviour
        {
            private Text textElement;
            protected override void OnAwake()
            {
                base.OnAwake();
                textElement = GetComponent<Text>();
            }

            void OnEnable()
            {
                IKCtrl.StateChanged += OnStateChanged;

                OnStateChanged(null, null);
            }
            
            void OnDisable()
            {
                IKCtrl.StateChanged -= OnStateChanged;
            }
            
            private void OnStateChanged(object sender, EventArgs e)
            {
                textElement.text = IKCtrl.Visibility == IKCtrl.IKVisibility.Visible
                    ? IKCtrl.State.ToString()
                    : "-";
            }
        }

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
        private Canvas _Canvas;

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
                HelpText.Create("Grab", FindAttachPosition("trigger"), new Vector3(0.06f, 0.04f, -0.05f)),
            });
        }

        protected override void OnStart()
        {
            base.OnStart();
            _Rumble = new TravelDistanceRumble(300, 0.05f, transform);

            CreateIndicatorCanvas();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Owner.StopRumble(_Rumble);

            if(!(Owner.Other.ActiveTool is IKTool))
            {
                IKCtrl.Hide();
            }

            if(_Canvas)
            {
                _Canvas.gameObject.SetActive(false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            IKCtrl.Show();

            if (_Canvas)
            {
                _Canvas.gameObject.SetActive(true);
            }
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
                    if (IKCtrl.Visibility == IKCtrl.IKVisibility.Deactivated)
                    {
                        IKCtrl.Enable();
                    }
                    else
                    {
                        IKCtrl.Toggle();
                    }
                }
                if (device.GetPressDown(EVRButtonId.k_EButton_Grip))
                {
                    if (IKCtrl.Visibility == IKCtrl.IKVisibility.Deactivated)
                    {
                        IKCtrl.Enable();
                    }
                    else
                    {
                        IKCtrl.Disable();
                    }
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

        private void Release()
        {
            _HoverObject = null;
        }

        protected override void OnDestroy()
        {
        }

        private void CreateIndicatorCanvas()
        {
            var canvas = _Canvas = UnityHelper.CreateGameObjectAsChild("IKFK", FindAttachPosition("trackpad")).gameObject.AddComponent<Canvas>();


            canvas.renderMode = RenderMode.WorldSpace;

            canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
            canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);

            canvas.transform.localPosition = new Vector3(0, 0, 0.00327f);
            canvas.transform.localRotation = Quaternion.Euler(0, 180, 90);
            canvas.transform.localScale = new Vector3(0.0002294636f, 0.0002294636f, 0.0002294636f);


            var text = UnityHelper.CreateGameObjectAsChild("Text", canvas.transform).gameObject.AddComponent<Text>();
            var outline = text.gameObject.AddComponent<Outline>();
            text.gameObject.AddComponent<IKStateReactor>();

            // Maximize
            text.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            text.GetComponent<RectTransform>().anchorMax = Vector2.one;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 50;
            text.resizeTextForBestFit = false;
        }
    }
}
