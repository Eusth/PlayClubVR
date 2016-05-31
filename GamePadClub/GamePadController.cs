using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XInputDotNetPure;

namespace GamePadClub
{
    public class GamePadController : MonoBehaviour
    {
        public delegate bool GamePadConsumer(GamePadState nowState, GamePadState prevState);
        
        private static GamePadController _Instance;
        private List<GamePadConsumer> _Consumers = new List<GamePadConsumer>();

        private GamePadState? _PrevState;

        public static GamePadController Instance
        {
            get
            {
                if (!_Instance)
                {
                    _Instance = new GameObject("GamePadClub_Controller").AddComponent<GamePadController>();
                    DontDestroyOnLoad(_Instance.gameObject);
                }
                return _Instance;
            }
        }

        public void Register(GamePadConsumer consumer)
        {
            _Consumers.Add(consumer);
        }

        public void RegisterAtStart(GamePadConsumer consumer)
        {
            _Consumers.Insert(0, consumer);
        }
        public bool Unregister(GamePadConsumer consumer)
        {
            return _Consumers.Remove(consumer);
        }


        public void FixedUpdate()
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            if (_PrevState != null)
            {
                foreach (var consumer in _Consumers)
                {
                    if (consumer(state, _PrevState.Value))
                    {
                        break;
                    }
                }
            }

            _PrevState = state;
        }
    }
}
