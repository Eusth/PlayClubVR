using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;

namespace PlayClubStudioVR
{
    /**
    * Manages the eye target for a single female.
    */
    class EyeTargetController : MonoBehaviour
    {
        public Transform Target { get; private set; }
        public Transform rootNode;
        private EyeTarget eyeTarget;

        void Start()
        {
            Target = new GameObject().transform;

            eyeTarget = Target.gameObject.AddComponent<EyeTarget>();
            eyeTarget.rootNode = rootNode;
        }


        void OnDestroy()
        {
            // Character was destroyed, so destroy the created target!
            Destroy(Target.gameObject);
        }
    }

    /// <summary>
    /// Represents a target that can be looked at.
    /// </summary>
    class EyeTarget : MonoBehaviour
    {
        /// <summary>
        /// Offset in meters from the camera.
        /// </summary>
        public float offset = 0.5f;

        /// <summary>
        /// Origin of the gaze.
        /// </summary>
        public Transform rootNode;

        void Update()
        {
            if (rootNode != null)
            {
                var camera = VR.Camera.SteamCam.head.transform;
                var dir = (camera.position - rootNode.position).normalized;

                transform.position = camera.position + dir * offset;
            }
            
        }
    }
}
