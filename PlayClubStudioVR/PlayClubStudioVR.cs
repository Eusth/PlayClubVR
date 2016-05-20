using IllusionPlugin;
using PlayClubVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Core;

namespace PlayClubStudioVR
{
    public class PlayClubStudioVR : IEnhancedPlugin
    {
        public string[] Filter
        {
            get
            {
                return new string[] { "PlayClubStudio" };
            }
        }

        public string Name
        {
            get
            {
                return "Play Club Studio VR";
            }
        }

        public string Version
        {
            get
            {
                return "0.5";
            }
        }

        public void OnApplicationQuit()
        {

        }

        public void OnApplicationStart()
        {
            if (Environment.CommandLine.Contains("--vr"))
            {
                var manager = VRManager.Create<StudioInterpreter>(new PlayClubContext());
                manager.SetMode<StudioSeatedMode>();
            }
            if (Environment.CommandLine.Contains("--verbose"))
            {
                Logger.Level = Logger.LogMode.Debug;
            }
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLateUpdate()
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
