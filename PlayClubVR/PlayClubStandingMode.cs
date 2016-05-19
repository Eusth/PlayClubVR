using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Core.Controls;
using VRGIN.Core.Helpers;
using VRGIN.Core.Modes;

namespace PlayClubVR
{
    class PlayClubStandingMode : StandingMode
    {

        protected virtual void OnEnable()
        {
            Logger.Info("Enter standing mode");

        }

        protected virtual void OnDisable()
        {
            Logger.Info("Leave standing mode");
        }

        public override IEnumerable<Type> Tools
        {
            get
            {
                return base.Tools.Concat(new Type[] { typeof(PlayTool) });
            }
        }

        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl + C"), new KeyStroke("Ctrl + C"), delegate { VR.Manager.SetMode<PlayClubSeatedMode>(); } ),
            });
        }

        protected override void CreateControllers()
        {
            base.CreateControllers();


            foreach(var controller in new Controller[] { Left, Right })
            {
                var boneCollider = controller.gameObject.AddComponent<DynamicBoneCollider_Custom>();
                boneCollider.m_Radius = 0.000005f; // Does not seem to have an effect
                boneCollider.m_Center.y = -0.03f;
                boneCollider.m_Center.z = 0.01f;

                foreach(var actor in VR.Interpreter.Actors.OfType<PlayClubActor>())
                {
                    actor.RegisterDynamicBoneCollider(boneCollider);
                }
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
