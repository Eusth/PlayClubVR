using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using WindowsInput;
using XInputDotNetPure;

namespace GamePadClub.Handlers
{
    public class CameraHandler : MonoBehaviour
    {
        private const float TRIGGER_THRESHOLD = 0.8f;

        private IllusionCamera _Camera;
        private FieldInfo _DistanceField = typeof(IllusionCamera).GetField("distance", BindingFlags.NonPublic | BindingFlags.Instance);
        private Queue<Action> _Messages = new Queue<Action>();
        void OnEnable()
        {
            GamePadController.Instance.Register(MoveCamera);
        }

        void OnDisable()
        {
            GamePadController.Instance.Unregister(MoveCamera);
        }

        void Start()
        {
            StartCoroutine(HandleMessages());
        }
        
        public void OnLevelWasLoaded()
        {
            _Camera = FindObjectOfType<IllusionCamera>();
        }

        bool MoveCamera(GamePadState state, GamePadState prevState)
        {
            if (GamePadHelper.IsShoulderPressed(state)) return false;
            
            if (_Camera)
            {
                var leftThumb = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
                var rightThumb = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
                bool leftTrigger = state.Triggers.Left > TRIGGER_THRESHOLD;
                bool rightTrigger = state.Triggers.Right > TRIGGER_THRESHOLD;

                if (leftTrigger || rightTrigger)
                {
                    // Zoom
                    _DistanceField.SetValue(_Camera, Mathf.Max(0.1f, _Camera.Distance - rightThumb.y * Time.deltaTime * 2));

                    _Messages.Enqueue(() =>
                    {
                        _Camera.Rotate(new Vector3(0, -rightThumb.x, 0) * Time.deltaTime * 100);
                    });
                }
                if (leftTrigger)
                {
                    // Pan 1
                    _Camera.Translate(leftThumb * Time.deltaTime * 100, false);

                }
                else if (rightTrigger)
                {
                    // Pan 2
                    _Camera.Translate(new Vector3(leftThumb.x, 0, leftThumb.y) * Time.deltaTime * 100, false);

                }
                else
                {
                    // Move
                    _Messages.Enqueue(() =>
                    {
                        _Camera.Rotate(new Vector3(rightThumb.y, -rightThumb.x, 0) * Time.deltaTime * 100);
                    });
                }
            }
            return false;
        }

        IEnumerator HandleMessages()
        {
            //Console.WriteLine("Starting message queue");
            while(true)
            {
                //Console.WriteLine("Iterate");
                // This code is executed between Update() and LateUpdate()
                // We do this to not interfere with the VR mod
                while (_Messages.Count > 0)
                {
                    //Console.WriteLine("Dequeue");

                    _Messages.Dequeue()();
                }
                yield return null;
            }
        }
    }
    
}
