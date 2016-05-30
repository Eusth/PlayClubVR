using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using VRGIN.Core;
using VRGIN.Controls;
using VRGIN.Helpers;
using VRGIN.Controls.Tools;

namespace PlayClubVR
{
    public class PlayTool : Tool
    {
        private const float DOT_PRODUCT_THRESHOLD = 0.7f;

        H_Scene scene;
        bool _AlteringSpeed;
        bool _IgnoreNextTrigger;

        public override Texture2D Image
        {
            get
            {
                return UnityHelper.LoadImage("icon_play.png");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            Tracking = GetComponent<SteamVR_TrackedObject>();
        }
        protected override void OnFixedUpdate()
        {
            scene = (VR.Interpreter as PlayClubInterpreter).Scene;
            if (scene && IsTracking)
            {
                var device = this.Controller;

                var tPadPos = device.GetAxis();
                var tPadClick = device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Touchpad);
                var tPadTouch = device.GetTouch(EVRButtonId.k_EButton_SteamVR_Touchpad);
                if (tPadClick)
                {
                    if (Vector2.Dot(Vector2.up, tPadPos) > DOT_PRODUCT_THRESHOLD)
                    {
                        Logger.Debug("up");
                        TogglePiston();
                    }
                    else if (Vector2.Dot(-Vector2.up, tPadPos) > DOT_PRODUCT_THRESHOLD)
                    {
                        Logger.Debug("down");

                        ToggleGrind();
                    }
                    else if (Vector2.Dot(-Vector2.right, tPadPos) > DOT_PRODUCT_THRESHOLD)
                    {
                        Logger.Debug("left");

                        ChangePose(-1);
                    }
                    else if (Vector2.Dot(Vector2.right, tPadPos) > DOT_PRODUCT_THRESHOLD)
                    {
                        Logger.Debug("right");

                        ChangePose(1);
                    }
                }
                if (device.GetTouchDown(EVRButtonId.k_EButton_Axis0))
                {
                    if (tPadPos.magnitude < 0.4f)
                    {
                        _AlteringSpeed = true;
                    }
                }

                if (_AlteringSpeed && tPadTouch)
                {
                    // Normalize
                    float val = (tPadPos.y + 1) / 2;
                    SetSpeed(val);
                }
                if (device.GetTouchUp(EVRButtonId.k_EButton_Axis0))
                {
                    _AlteringSpeed = false;
                }

                if (device.GetTouchUp(EVRButtonId.k_EButton_Grip))
                {
                    ToggleOrgasmLock();
                }

                if (device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
                {
                    if (_IgnoreNextTrigger)
                    {
                        _IgnoreNextTrigger = false;
                    }
                    else
                    {
                        Ejaculate();
                    }
                }
            }
        }

        private void OnImpersonated()
        {
            _IgnoreNextTrigger = true;
        }

        private void ToggleOrgasmLock()
        {
            //Console.WriteLine(!scene.FemaleGage.Lock ? "Lock" : "Unlock");
            // This also updates the GUI!
            GameObject.Find("XtcLock").GetComponent<Toggle>().isOn = !scene.FemaleGage.Lock;
            //scene.FemaleGage.ChangeLock(!scene.FemaleGage.Lock);
        }


        private void TogglePiston()
        {
            scene.Pad.pistonToggle.OnPointerClick(new PointerEventData(EventSystem.current));

        }

        private void ToggleGrind()
        {
            scene.Pad.grindToggle.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        private void SetSpeed(float val)
        {
            scene.Pad.ChangeSpeed(val * 5);
        }

        private void Ejaculate()
        {
            if (scene.FemaleGage.IsHigh())
            {
                scene.StateMgr.SetXtc(H_Pad.XTC.SYNC);
            }
            else if (scene.Members[0].sex == Human.SEX.MALE)
            {
                scene.StateMgr.SetXtc(H_Pad.XTC.M_OUT);
            }
            else
            {
                scene.StateMgr.SetXtc(H_Pad.XTC.F);
            }
        }

        private void ChangePose(int direction)
        {
            int currentIndex = scene.StyleMgr.StyleList.IndexOf(scene.StyleMgr.nowStyle);
            int nextIndex = ((currentIndex + scene.StyleMgr.StyleList.Count) + direction) % scene.StyleMgr.StyleList.Count;

            ChangeStyle(scene.StyleMgr.StyleList[nextIndex].file);
        }


        private void ChangeStyle(string name)
        {
            scene.ChangeStyle(name);
            scene.StyleToGUI(name);
            scene.CrossFadeStart();
        }

        public override List<HelpText> GetHelpTexts()
        {
            return new List<HelpText>(new HelpText[] {
                HelpText.Create("Slide to set speed", FindAttachPosition("trackpad"), new Vector3(0.07f, 0.02f, 0.05f)),
                HelpText.Create("Piston", FindAttachPosition("trackpad"), new Vector3(0, 0.02f, 0.05f), new Vector3(0, 0.0f, 0.015f)),
                HelpText.Create("Grind", FindAttachPosition("trackpad"), new Vector3(0, 0.02f, -0.05f), new Vector3(0, 0.0f, -0.015f)),
                HelpText.Create("Next animation", FindAttachPosition("trackpad"), new Vector3(0.05f, 0.02f, 0), new Vector3(+0.015f, 0.0f, 0)),
                HelpText.Create("Prev animation", FindAttachPosition("trackpad"), new Vector3(-0.05f, 0.02f, 0), new Vector3(-0.015f, 0.0f, 0)),
                HelpText.Create("Toggle lock", FindAttachPosition("lgrip"), new Vector3(-0.05f, 0.02f, 0)),
                HelpText.Create("Ejaculate", FindAttachPosition("trigger"), new Vector3(0.05f, 0.02f, -0.05f)),
            });
        }

        protected override void OnDestroy()
        {
        }
    }
}
