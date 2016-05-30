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

namespace PlayClubVR
{
    public class MaestroTool : Tool
    {
        Transform _Handle = null;
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
                if (!_Available.HasValue)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        try {
                            if(assembly.GetTypes().Any(type => type.FullName == "MaestroMode.MaestroPlugin"))
                            {
                                _Available = true;
                                break;
                            }
                        } catch(Exception)
                        {}
                    }
                    if(!_Available.HasValue)
                    {
                        _Available = false;
                    }
                }
                return _Available.Value;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            _Controller = GetComponent<Controller>();
            _Dummy = new GameObject().transform;
            _Dummy.SetParent(transform, false);
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (IsMaestroHandle(other.gameObject)) {
                _FocusCount++;

                if (!_Dragging)
                {
                    _Handle = other.transform;
                } else if(!_Focused && other.transform == _Handle)
                {
                    _Focused = true;
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (IsMaestroHandle(other.gameObject))
            {
                _FocusCount--;

                if (_Dragging && _Focused && other.transform == _Handle)
                {
                    _Focused = false;
                }
            }
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
                        var maestroHandle = GetMaestroHandle(_Handle.gameObject);

                        var changedField = maestroHandle.GetType().BaseType.GetField("_changed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        changedField.SetValue(maestroHandle, true);

                        _Dummy.position = _Handle.position;
                        _Dummy.rotation = _Handle.rotation;
                        _Dragging = true;
                    }
                }
                if (_Handle && _Dragging)
                {
                    if (device.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
                    {
                        _Handle.position = _Dummy.position;
                        _Handle.rotation = _Dummy.rotation;
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
                    SendKey.Send(Keys.F9, false);
                }
            }
        }

        public override List<HelpText> GetHelpTexts()
        {
            return base.GetHelpTexts();
        }

        private Transform FindNextHandle()
        {
            // Evil method -- should be replaced with direct communication with Maestro Mode in a future release
            Transform handle = GameObject.FindObjectsOfType<Transform>().Select(t => t.gameObject).Where(IsMaestroHandle)
                               .Select(go => go.transform).OrderBy(t => Vector3.Distance(_Controller.transform.position, t.position)).FirstOrDefault();
            
            if(handle && Vector3.Distance(_Controller.transform.position, handle.position) < 0.5f)
            {
                VRLog.Info("FOUND HANDLE");
                return handle;
            } else
            {
                return null;
            }

        }

        bool IsMaestroHandle(GameObject go)
        {
            return go.layer == LayerMask.NameToLayer("Chara") && go.GetComponents<MonoBehaviour>().Any(IsMaestroHandle);
        }
        
        object GetMaestroHandle(GameObject go) {
            return go.GetComponents<MonoBehaviour>().First(IsMaestroHandle);
        }
        bool IsMaestroHandle(MonoBehaviour type)
        {
            return type.GetType().Name.Contains("Handle");
        }

        protected override void OnDestroy()
        {
        }



        /// <summary>
        /// Enumeration for virtual keys.
        /// </summary>
        public enum Keys
            : ushort
        {
            /// <summary></summary>
            LeftButton = 0x01,
            /// <summary></summary>
            RightButton = 0x02,
            /// <summary></summary>
            F9 = 0x78
        }
        class SendKey
        {

            [StructLayout(LayoutKind.Sequential)]
            private struct MOUSEINPUT
            {
                public int dx;
                public int dy;
                public int mouseData;
                public int dwFlags;
                public int time;
                public int dwExtraInfo;
            };

            [StructLayout(LayoutKind.Sequential)]
            private struct KEYBDINPUT
            {
                public short wVk;
                public short wScan;
                public int dwFlags;
                public int time;
                public int dwExtraInfo;
            };

            [StructLayout(LayoutKind.Sequential)]
            private struct HARDWAREINPUT
            {
                public int uMsg;
                public short wParamL;
                public short wParamH;
            };

            [StructLayout(LayoutKind.Explicit)]
            private struct INPUT
            {
                [FieldOffset(0)]
                public int type;
                [FieldOffset(4)]
                public MOUSEINPUT no;
                [FieldOffset(4)]
                public KEYBDINPUT ki;
                [FieldOffset(4)]
                public HARDWAREINPUT hi;
            };

            [DllImport("user32.dll")]
            private extern static void SendInput(int nInputs, ref INPUT pInputs, int cbsize);
            [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
            private extern static int MapVirtualKey(int wCode, int wMapType);

            private const int INPUT_KEYBOARD = 1;
            private const int KEYEVENTF_KEYDOWN = 0x0;
            private const int KEYEVENTF_KEYUP = 0x2;
            private const int KEYEVENTF_EXTENDEDKEY = 0x1;

            public static void Send(Keys key, bool isEXTEND)
            {
                /*
                 * Keyを送る
                 * 入力
                 *     isEXTEND : 拡張キーかどうか
                 */

                INPUT inp = new INPUT();

                // 押す
                inp.type = INPUT_KEYBOARD;
                inp.ki.wVk = (short)key;
                inp.ki.wScan = (short)MapVirtualKey(inp.ki.wVk, 0);
                inp.ki.dwFlags = ((isEXTEND) ? (KEYEVENTF_EXTENDEDKEY) : 0x0) | KEYEVENTF_KEYDOWN;
                inp.ki.time = 0;
                inp.ki.dwExtraInfo = 0;
                SendInput(1, ref inp, Marshal.SizeOf(inp));

                System.Threading.Thread.Sleep(100);

                // 離す
                inp.ki.dwFlags = ((isEXTEND) ? (KEYEVENTF_EXTENDEDKEY) : 0x0) | KEYEVENTF_KEYUP;
                SendInput(1, ref inp, Marshal.SizeOf(inp));
            }
        }
    }
}
