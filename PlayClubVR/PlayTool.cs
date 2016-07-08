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
                        (VR.Interpreter as PlayClubInterpreter).TogglePiston();
                    }
                    else if (Vector2.Dot(-Vector2.up, tPadPos) > DOT_PRODUCT_THRESHOLD)
                    {
                        Logger.Debug("down");

                        (VR.Interpreter as PlayClubInterpreter).ToggleGrind();
                    }
                    else if (Vector2.Dot(-Vector2.right, tPadPos) > DOT_PRODUCT_THRESHOLD)
                    {
                        Logger.Debug("left");

                        (VR.Interpreter as PlayClubInterpreter).ChangePose(-1);
                    }
                    else if (Vector2.Dot(Vector2.right, tPadPos) > DOT_PRODUCT_THRESHOLD)
                    {
                        Logger.Debug("right");

                        (VR.Interpreter as PlayClubInterpreter).ChangePose(1);
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
                    (VR.Interpreter as PlayClubInterpreter).SetSpeed(val);
                }
                if (device.GetTouchUp(EVRButtonId.k_EButton_Axis0))
                {
                    _AlteringSpeed = false;
                }

                //if (device.GetTouchUp(EVRButtonId.k_EButton_Grip))
                //{
                //    ToggleOrgasmLock();
                //}

                if (device.GetPressUp(EVRButtonId.k_EButton_Grip))
                {
                    if (_IgnoreNextTrigger)
                    {
                        _IgnoreNextTrigger = false;
                    }
                    else
                    {
                        (VR.Interpreter as PlayClubInterpreter).Ejaculate();
                    }
                }
            }
        }

        private void OnImpersonated()
        {
            _IgnoreNextTrigger = true;
        }
        
        public override List<HelpText> GetHelpTexts()
        {
            return new List<HelpText>(new HelpText[] {
                HelpText.Create("Slide to set speed", FindAttachPosition("trackpad"), new Vector3(0.07f, 0.02f, 0.05f)),
                HelpText.Create("Piston", FindAttachPosition("trackpad"), new Vector3(0, 0.02f, 0.05f), new Vector3(0, 0.0f, 0.015f)),
                HelpText.Create("Grind", FindAttachPosition("trackpad"), new Vector3(0, 0.02f, -0.05f), new Vector3(0, 0.0f, -0.015f)),
                HelpText.Create("Next animation", FindAttachPosition("trackpad"), new Vector3(0.05f, 0.02f, 0), new Vector3(+0.015f, 0.0f, 0)),
                HelpText.Create("Prev animation", FindAttachPosition("trackpad"), new Vector3(-0.05f, 0.02f, 0), new Vector3(-0.015f, 0.0f, 0)),
                //HelpText.Create("Toggle lock", FindAttachPosition("lgrip"), new Vector3(-0.05f, 0.02f, 0)),
                HelpText.Create("Ejaculate", FindAttachPosition("lgrip"), new Vector3(0.05f, 0.02f, -0.05f)),
            });
        }

        protected override void OnDestroy()
        {
        }
    }
}
