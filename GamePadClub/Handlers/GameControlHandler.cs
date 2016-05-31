using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XInputDotNetPure;

namespace GamePadClub.Handlers
{
    public class GameControlHandler : MonoBehaviour
    {
        GameControl _GC;
        H_Scene _Scene;

        void OnLevelWasLoaded()
        {
            _GC = FindObjectOfType<GameControl>();
            _Scene = FindObjectOfType<H_Scene>();
        }

        void OnEnable()
        {
            GamePadController.Instance.Register(HandleInput);
        }

        void OnDisable()
        {
            GamePadController.Instance.Unregister(HandleInput);
        }

        bool HandleInput(GamePadState now, GamePadState before)
        {
            if (GamePadHelper.IsShoulderPressed(now)) return false;
            
            if (_GC && _Scene && now.Buttons.LeftShoulder == ButtonState.Released && now.Buttons.RightShoulder == ButtonState.Released)
            {
                return HandleGUI(now, before) || HandleSekkusu(now, before);
            }
            return false;
        }

        bool HandleGUI(GamePadState now, GamePadState before)
        {

            if (GamePadHelper.IsPressUp(now.Buttons.Back, before.Buttons.Back))
            {
                _GC.IsHideUI = !_GC.IsHideUI;
            }
            return false;
        }

        bool HandleSekkusu(GamePadState now, GamePadState before)
        {

            // Speed
            if (GamePadHelper.IsPressUp(now.DPad.Up, before.DPad.Up))
            {
                _Scene.Pad.ChangeSpeed(Mathf.Clamp(_Scene.Pad.speed + 0.6f, _Scene.Pad.SpeedMin, _Scene.Pad.SpeedMax));
            }
            else if(GamePadHelper.IsPressUp(now.DPad.Down, before.DPad.Down))
            {
                _Scene.Pad.ChangeSpeed(Mathf.Clamp(_Scene.Pad.speed - 0.6f, _Scene.Pad.SpeedMin, _Scene.Pad.SpeedMax));
            }

            // Pose
            if (GamePadHelper.IsPressUp(now.DPad.Left, before.DPad.Left))
            {
                ChangePose(-1);
            }
            else if (GamePadHelper.IsPressUp(now.DPad.Right, before.DPad.Right))
            {
                ChangePose(1);
            }

            // Mode
            if (GamePadHelper.IsPressUp(now.Buttons.X, before.Buttons.X))
            {
                ToggleGrind();
            }
            if (GamePadHelper.IsPressUp(now.Buttons.Y, before.Buttons.Y))
            {
                TogglePiston();
            }

            // Ejaculation
            if (GamePadHelper.IsPressDown(now.Buttons.RightStick, before.Buttons.RightStick))
            {
                Ejaculate(false);
            }

            if (GamePadHelper.IsPressDown(now.Buttons.LeftStick, before.Buttons.LeftStick))
            {
                Ejaculate(true);
            }
            

            return false;
        }



        private void ToggleOrgasmLock()
        {
            //Console.WriteLine(!_Scene.FemaleGage.Lock ? "Lock" : "Unlock");
            // This also updates the GUI!
            GameObject.Find("XtcLock").GetComponent<Toggle>().isOn = !_Scene.FemaleGage.Lock;
            //_Scene.FemaleGage.ChangeLock(!_Scene.FemaleGage.Lock);
        }


        private void TogglePiston()
        {
            _Scene.Pad.pistonToggle.OnPointerClick(new PointerEventData(EventSystem.current));

        }

        private void ToggleGrind()
        {
            _Scene.Pad.grindToggle.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        private void SetSpeed(float val)
        {
            _Scene.Pad.ChangeSpeed(val * 5);
        }

        private void Ejaculate(bool outside)
        {
            if (_Scene.FemaleGage.IsHigh())
            {
                _Scene.StateMgr.SetXtc(H_Pad.XTC.SYNC);
            }
            else if (_Scene.Members[0].sex == Human.SEX.MALE)
            {
                if (outside)
                {
                    _Scene.StateMgr.SetXtc(H_Pad.XTC.M_OUT);
                } else
                {
                    _Scene.StateMgr.SetXtc(H_Pad.XTC.M_IN);
                }
            }
            else
            {
                _Scene.StateMgr.SetXtc(H_Pad.XTC.F);
            }
        }

        private void ChangePose(int direction)
        {
            int currentIndex = _Scene.StyleMgr.StyleList.IndexOf(_Scene.StyleMgr.nowStyle);
            int nextIndex = ((currentIndex + _Scene.StyleMgr.StyleList.Count) + direction) % _Scene.StyleMgr.StyleList.Count;

            _Scene.ChangeStyle(_Scene.StyleMgr.StyleList[nextIndex].file);
        }

    }


}
