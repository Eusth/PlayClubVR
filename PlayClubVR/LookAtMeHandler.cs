using CameraModifications;
using IllusionInjector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Core;

namespace PlayClubVR
{
    class LookAtMeHandler : ProtectedBehaviour
    {

        //CameraModifications.KocchiMitePlugin _KocchitMite;

        protected override void OnStart()
        {
            base.OnStart();
            if(!PluginManager.Plugins.Any(plugin => plugin.GetType().FullName == "CameraModifications.KocchiMitePlugin"))
            {
                VRLog.Warn("Didn't find Kocchi Mite plugin");

                enabled = false;
                return;
            }

            VRLog.Info("Found Kocchi Mite plugin");

        }

        protected override void OnLevel(int level)
        {
            base.OnLevel(level);

            if ((VR.Settings as PlayClubSettings).AutoLookAtMe) {
                StartCoroutine(WaitALittle(delegate
                {
                    VRLog.Info("Setting gaze to camera");
                    KocchiMite.HeadLookType = LookAtRotator.TYPE.TARGET;
                    KocchiMite.LookType = LookAtRotator.TYPE.TARGET;
                }));
            }
        }

        private KocchiMitePlugin KocchiMite
        {
            get
            {
                return PluginManager.Plugins.OfType<KocchiMitePlugin>().First();
            }
        }

        IEnumerator WaitALittle(Action action)
        {
            yield return null;
            yield return null;

            action();
        }

    }
}
