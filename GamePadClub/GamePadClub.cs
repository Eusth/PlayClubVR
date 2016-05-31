using GamePadClub.Handlers;
using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamePadClub
{
    public class GamePadClub : IPlugin
    {
        public string Name
        {
            get
            {
                return "GamePadClub";
            }
        }

        public string Version
        {
            get
            {
                return "0.1";
            }
        }

        public void OnApplicationQuit()
        {
        }

        public void OnApplicationStart()
        {
            var instance = GamePadController.Instance;
            instance.gameObject.AddComponent<CameraHandler>();
            instance.gameObject.AddComponent<MouseHandler>();
            instance.gameObject.AddComponent<GameControlHandler>();
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnUpdate()
        {
        }
    }
}
