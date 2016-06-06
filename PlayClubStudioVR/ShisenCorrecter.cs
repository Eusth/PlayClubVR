using PlayClubVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using VRGIN.Core;

namespace PlayClubStudioVR
{
    class ShisenCorrecter : MonoBehaviour {

        private Human _Human;
        public PlayClubActor Actor;

        void Start()
        {
            _Human = GetComponent<Human>();
        }


        void Update()
        {
            if (!_Human) return;

            var eyeLookCtrl = _Human.eyeLook;
            var targetCtrl = _Human.bodyRoot.GetComponent<EyeTargetController>();

            // Make a new EyeTargetController if required
            if (targetCtrl == null)
            {
                targetCtrl = _Human.bodyRoot.gameObject.AddComponent<EyeTargetController>();
                targetCtrl.rootNode = Actor.Eyes;
            }

            // Replace camera targets if required
            if (eyeLookCtrl.Target.camera == Camera.main)
            {
                eyeLookCtrl.Change(LookAtRotator.TYPE.TARGET, targetCtrl.Target, true);
            }

            var neckLookCtrl = _Human.neckLook;
            if (neckLookCtrl.Target.camera == Camera.main)
            {
                neckLookCtrl.Change(LookAtRotator.TYPE.TARGET, targetCtrl.Target, true);
            }
        }
    }
}
