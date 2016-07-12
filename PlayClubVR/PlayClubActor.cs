using PlayClubVR.OSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Helpers;

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

            // Register bones
            foreach (var bone in SoftCustomBones)
            {
                DynamicColliderRegistry.RegisterDynamicBone(bone);
            }
            foreach (var bone in SoftBones)
            {
                DynamicColliderRegistry.RegisterDynamicBone(bone);
            }
        }

        public IEnumerable<DynamicBone> SoftBones
        {
            get
            {
                return Actor.GetComponentsInChildren<DynamicBone>();
            }
        }

        public IEnumerable<DynamicBone_Custom> SoftCustomBones
        {
            get
            {
                return Actor.GetComponentsInChildren<DynamicBone_Custom>();
            }
        }
    }
}
