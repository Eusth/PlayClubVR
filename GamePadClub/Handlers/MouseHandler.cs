using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WindowsInput;
using XInputDotNetPure;

namespace GamePadClub.Handlers
{
    public class MouseHandler : MonoBehaviour
    {
        private const float TRIGGER_THRESHOLD = 0.5f;
        InputSimulator _Input;

        void Awake()
        {
            _Input = new InputSimulator();
        }

        void OnEnable()
        {
            GamePadController.Instance.Register(MoveMouse);
            GamePadController.Instance.Register(HandleClicks);
        }

        void OnDisable()
        {
            GamePadController.Instance.Unregister(HandleClicks);
            GamePadController.Instance.Unregister(MoveMouse);
        }

        bool HandleClicks(GamePadState now, GamePadState before)
        {
            if (GamePadHelper.IsShoulderPressed(now)) return false;

            // Left click
            if (GamePadHelper.IsPressDown(now.Buttons.A, before.Buttons.A))
            {
                _Input.Mouse.LeftButtonDown();
            }
            else if (GamePadHelper.IsPressUp(now.Buttons.A, before.Buttons.A))
            {
                _Input.Mouse.LeftButtonUp();
            }

            // Right click
            if (GamePadHelper.IsPressDown(now.Buttons.B, before.Buttons.B))
            {
                _Input.Mouse.RightButtonDown();
            }
            else if (GamePadHelper.IsPressUp(now.Buttons.B, before.Buttons.B))
            {
                _Input.Mouse.RightButtonUp();
            }

            return false;
        }

        bool MoveMouse(GamePadState now, GamePadState before)
        {
            if (now.Triggers.Left < TRIGGER_THRESHOLD && now.Triggers.Right < TRIGGER_THRESHOLD && !GamePadHelper.IsShoulderPressed(now))
            {
                // Move cursor
                _Input.Mouse.MoveMouseBy((int)(now.ThumbSticks.Left.X * Time.deltaTime * 700),
                                         (int)(-now.ThumbSticks.Left.Y * Time.deltaTime * 700));
            }

            return false;
        }

    }
}
