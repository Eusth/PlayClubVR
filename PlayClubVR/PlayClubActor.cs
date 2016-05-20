using PlayClubVR.OSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using VRGIN.Core;

namespace PlayClubVR
{
    public class PlayClubActor : DefaultActor<Human>
    {

        private static FieldInfo _HeartField = typeof(HumanVoice).GetField("heart", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        private static FieldInfo _VoiceField = typeof(HumanVoice).GetField("voice", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        private Transform _EyeTransform;
        private TransientHead _Head;

        public PlayClubActor(Human nativeActor) : base(nativeActor)
        {
        }

        public override Transform Eyes
        {
            get
            {
                return _EyeTransform;
            }
        }

        public override bool HasHead
        {
            get
            {
                return _Head.Visible;
            }
            set
            {
                _Head.Visible = value;
            }
        }

        protected override void Initialize(Human actor)
        {
            base.Initialize(actor);
            
            // Get position of actor's eyes
            _Head = Actor.gameObject.AddComponent<TransientHead>();
            _EyeTransform = TransientHead.GetEyes(Actor);

            // 3D-ize actor's voice
            if (actor.voice)
            {
                _HeartField.SetValue(actor.voice, OSPAudioVolumeManager.Create(_HeartField.GetValue(actor.voice) as AudioVolumeManager));
                _VoiceField.SetValue(actor.voice, OSPAudioVolumeManager.Create(_VoiceField.GetValue(actor.voice) as AudioVolumeManager));
            }

            // Register bone collider
            RegisterDynamicBoneCollider(VR.Mode.Left.GetComponent<DynamicBoneCollider_Custom>());
            RegisterDynamicBoneCollider(VR.Mode.Right.GetComponent<DynamicBoneCollider_Custom>());
        }


        public void RegisterDynamicBoneCollider(DynamicBoneCollider_Custom collider)
        {
            if (collider)
            {
                foreach(var bone in Actor.GetComponentsInChildren<DynamicBone>())
                {
                    Logger.Info("Registering to {0}", bone.name);
                    bone.m_Colliders.Add(collider);
                }
                foreach (var bone in Actor.GetComponentsInChildren<DynamicBone_Custom>())
                {
                    Logger.Info("Registering to {0}", bone.name);

                    bone.m_Colliders.Add(collider);
                }
                
            }
        }
    }
}
