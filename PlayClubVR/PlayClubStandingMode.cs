using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Controls;
using VRGIN.Helpers;
using VRGIN.Modes;

namespace PlayClubVR
{
    public class PlayClubStandingMode : StandingMode
    {
        private IllusionCamera _IllusionCamera;

        protected virtual void OnEnable()
        {
            base.OnEnable();
            Logger.Info("Enter standing mode");

        }

        protected virtual void OnDisable()
        {
            base.OnDisable();
            Logger.Info("Leave standing mode");
        }


        protected override void OnStart()
        {
            base.OnStart();
            _IllusionCamera = FindObjectOfType<IllusionCamera>();
            gameObject.AddComponent<ImpersonationHandler>();
        }

        protected override void OnLevel(int level)
        {
            base.OnLevel(level);
            _IllusionCamera = FindObjectOfType<IllusionCamera>();

        }

        public override IEnumerable<Type> Tools
        {
            get
            {
                var tools = base.Tools.Concat(new Type[] { typeof(PlayTool) });
                if (MaestroTool.IsAvailable) {
                    tools = tools.Concat(new Type[] { typeof(MaestroTool) });
                }
                return tools;
            }
        }

        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl + C"), new KeyStroke("Ctrl + C"), ChangeMode ),
            });
        }

        protected virtual void ChangeMode()
        {
            VR.Manager.SetMode<PlayClubSeatedMode>();
        }

        protected override void CreateControllers()
        {
            base.CreateControllers();

            foreach (var controller in new Controller[] { Left, Right })
            {
                var boneCollider = new GameObject("Dynamic Collider").AddComponent<DynamicBoneCollider>();
                boneCollider.transform.SetParent(controller.transform, false);
                boneCollider.m_Radius = -0.05f; // Does not seem to have an effect
                boneCollider.m_Center.y = -0.03f;
                boneCollider.m_Center.z = 0.01f;
                boneCollider.m_Bound = DynamicBoneCollider.Bound.Outside;
                boneCollider.m_Direction = DynamicBoneCollider.Direction.X;

                foreach(var actor in VR.Interpreter.Actors.OfType<PlayClubActor>())
                {
                    actor.RegisterDynamicBoneCollider(boneCollider);
                }
            }
        }

        protected override void SyncCameras()
        {
            base.SyncCameras();
            if (_IllusionCamera)
            {
                var my = VR.Camera.SteamCam.head;

                _IllusionCamera.Set(
                    my.position + my.forward,
                    Quaternion.LookRotation(my.forward, my.up).eulerAngles,
                1);
            }
        }
        //protected override Controller CreateLeftController()
        //{
        //    return PlayClubController.Create();
        //}

        //protected override Controller CreateRightController()
        //{
        //    var controller = PlayClubController.Create();
        //    controller.ToolIndex = 1;
        //    return controller;
        //}
    }
}
