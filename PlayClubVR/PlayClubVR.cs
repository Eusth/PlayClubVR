using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Core;
using VRGIN.Core.Modes;

namespace PlayClubVR
{
    public class PlayClubVR : IPlugin
    {
        public string Name
        {
            get
            {
                return "PlayClubVR";
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
                var manager = VRManager.Create<PlayClubInterpreter>(new PlayClubContext());
                manager.SetMode<PlayClubSeatedMode>();
            }
            if(Environment.CommandLine.Contains("--verbose"))
            {
                Logger.Level = Logger.LogMode.Debug;
            }
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
