using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Valve.VR;
using VRGIN.Core;
using VRGIN.Controls;
using VRGIN.Helpers;
using VRGIN.Native;
using VRGIN.Controls.Tools;
using IllusionInjector;
using MaestroMode;

namespace PlayClubVR
{
    public class MaestroTool : Tool
    {
        // This is to avoid a dependency to MaestroMode (no instance variables allowed or the application will crash at the absence of the DLL)
        object __Handle = null;
        private IKHandle _Handle
        {
            get
            {
                return __Handle as IKHandle;
            }
            set
            {
                __Handle = value;
            }
        }
        Controller _Controller;
        Transform _Dummy = null;
        private bool _Dragging;
        private bool _Focused;
        private int _FocusCount;

        private static bool? _Available;

        public override Texture2D Image
        {
            get
            {
                return UnityHelper.LoadImage("icon_maestro.png");
            }
        }

        /// <summary>
        /// Gets whether or not the MaestroMode DLL is loaded.
        /// </summary>
        public static bool IsAvailable
        {
            get
            {
                return PluginManager.Plugins.Any(plugin => plugin.GetType().FullName == "MaestroMode.MaestroPlugin");
            }
        }

        public MaestroPlugin Maestro
        {
            get
            {
                return PluginManager.Plugins.OfType<MaestroPlugin>().FirstOrDefault();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            _Controller = GetComponent<Controller>();
            _Dummy = new GameObject().transform;
            _Dummy.SetParent(transform, false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _FocusCount = 0;
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (_Controller.Tracking.isValid)
            {
                var device = SteamVR_Controller.Input((int)_Controller.Tracking.index);
          
                if (device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger))
                {
                    if(_FocusCount == 0)
                    {
                        VRLog.Info("Search for handle");
                        _Handle = FindNextHandle();
                    }
                    if (_Handle)
                    {
                        _Handle.gameObject.SendMessage("Select");
                        _Handle.gameObject.SendMessage("Activate", 1);

                        _Handle.MarkDirty();

                        _Dummy.position = _Handle.transform.position;
                        _Dummy.rotation = _Handle.transform.rotation;
                        _Dragging = true;
                    }
                }
                if (_Handle && _Dragging)
                {
                    if (device.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
                    {
                        _Handle.transform.position = _Dummy.position;
                        _Handle.transform.rotation = _Dummy.rotation;
                    }
                    if (device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
                    {
                        _Handle.gameObject.SendMessage("Deselect");
                        _Dragging = false;
                    }
                }

                if(_Handle && device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad))
                {
                    _Handle.gameObject.SendMessage("Reset");
                }

                if(device.GetPressDown(EVRButtonId.k_EButton_Grip))
                {
                    // Toggle
                    var prevMode = Maestro.Mode;
                    Maestro.Mode = MaestroPlugin.MaestroMode.FBBIK;
                    Maestro.Visible = prevMode == Maestro.Mode ? !Maestro.Visible : true;
                }
            }
        }

        public override List<HelpText> GetHelpTexts()
        {
            return base.GetHelpTexts();
        }

        private IKHandle FindNextHandle()
        {
            var handle = Maestro.Handles.OrderBy(h => Vector3.Distance(_Controller.transform.position, h.transform.position)).FirstOrDefault();
            
            if(handle && Vector3.Distance(_Controller.transform.position, handle.transform.position) < 0.5f)
            {
                return handle;
            } else
            {
                return null;
            }

        }

        protected override void OnDestroy()
        {
        }
    }
}
