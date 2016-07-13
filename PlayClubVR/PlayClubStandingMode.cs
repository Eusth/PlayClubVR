using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Controls;
using VRGIN.Helpers;
using VRGIN.Modes;
using VRGIN.Controls.Speech;
using Leap.Unity;

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

        public override void OnDestroy()
        {
            base.OnDestroy();

            DynamicColliderRegistry.Clear();
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
            var interpreter = VR.Interpreter as PlayClubInterpreter;

            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl + C"), new KeyStroke("Ctrl + C"), ChangeMode ),

                new VoiceShortcut(PlayClubVoiceCommand.StartAnimation, delegate { interpreter.TogglePiston(true); }),
                new VoiceShortcut(PlayClubVoiceCommand.StopAnimation, delegate { interpreter.TogglePiston(false); }),
                new VoiceShortcut(PlayClubVoiceCommand.Faster, delegate { interpreter.IncreaseSpeed(0.6f); }),
                new VoiceShortcut(PlayClubVoiceCommand.Slower, delegate { interpreter.IncreaseSpeed(-0.6f); }),
                new VoiceShortcut(PlayClubVoiceCommand.Climax, delegate { interpreter.Ejaculate(); }),
                new VoiceShortcut(PlayClubVoiceCommand.NextAnimation, delegate { interpreter.ChangePose(1); }),
                new VoiceShortcut(PlayClubVoiceCommand.PreviousAnimation, delegate { interpreter.ChangePose(-1); }),
                new VoiceShortcut(PlayClubVoiceCommand.DisableClimax, delegate { interpreter.ToggleOrgasmLock(true); }),
                new VoiceShortcut(PlayClubVoiceCommand.EnableClimax, delegate { interpreter.ToggleOrgasmLock(false); }),
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
                var boneCollider = CreateCollider(controller.transform, -0.05f);
                boneCollider.m_Center.y = -0.03f;
                boneCollider.m_Center.z = 0.01f;
                DynamicColliderRegistry.RegisterCollider(boneCollider, (b) => !IsNotBust(b));

                boneCollider = CreateCollider(controller.transform, 0.01f);
                boneCollider.m_Center.y = -0.03f;
                boneCollider.m_Center.z = 0.01f;
                DynamicColliderRegistry.RegisterCollider(boneCollider, IsNotBust);
            }
        }

        private DynamicBoneCollider CreateCollider(Transform parent, float radius)
        {
            var collider = UnityHelper.CreateGameObjectAsChild("Dynamic Collider", parent).gameObject.AddComponent<DynamicBoneCollider>();
            collider.m_Radius = radius;
            collider.m_Bound = DynamicBoneCollider.Bound.Outside;
            collider.m_Direction = DynamicBoneCollider.Direction.X;
            collider.m_Center.y = 0;
            collider.m_Center.z = 0;
            return collider;
        }

        protected override HandAttachments BuildAttachmentHand(Chirality handedness)
        {
            var hand = base.BuildAttachmentHand(handedness);

            foreach (var sphere in new Transform[] { hand.Thumb, hand.Index, hand.Middle, hand.Ring, hand.Pinky, hand.Palm })
            {
                var boneCollider = CreateCollider(sphere, -0.05f).gameObject.AddComponent<DynamicBoneCollider>();
                boneCollider.enabled = false;
                DynamicColliderRegistry.RegisterCollider(boneCollider, (b) => !IsNotBust(b));

                boneCollider = CreateCollider(sphere, 0.01f).gameObject.AddComponent<DynamicBoneCollider>();
                boneCollider.enabled = false;
                DynamicColliderRegistry.RegisterCollider(boneCollider, IsNotBust);


            }
            hand.OnBegin += delegate
            {
                foreach(var collider in hand.GetComponentsInChildren<DynamicBoneCollider>())
                {
                    collider.enabled = true;
                }
            };

            hand.OnFinish += delegate
            {
                foreach (var collider in hand.GetComponentsInChildren<DynamicBoneCollider>())
                {
                    collider.enabled = false;
                }
            };

            return hand;
        }

        private bool IsNotBust(IDynamicBoneWrapper wrapper)
        {
            return !wrapper.Bone.name.Contains("Bust");
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
