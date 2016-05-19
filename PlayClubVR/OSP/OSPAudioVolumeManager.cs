using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayClubVR.OSP
{
    class OSPAudioVolumeManager : AudioVolumeManager
    {
        private OSPAudioSource ospSource;

        public OSPAudioVolumeManager(AudioSource source, Human human) : base(source, human)
        {
        }

        public static OSPAudioVolumeManager Create(AudioVolumeManager blueprint)
        {
            var sourceField = typeof(AudioVolumeManager).GetField("source", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var humanField = typeof(AudioVolumeManager).GetField("human", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            var manager = new OSPAudioVolumeManager(sourceField.GetValue(blueprint) as AudioSource, humanField.GetValue(blueprint) as Human);
            manager.ospSource = manager.source.gameObject.AddComponent<OSPAudioSource>();
            return manager;
        }

        public override void Play(float fade)
        {
            this.ospSource.Play();
            if (fade == 0f)
            {
                this.rate = 1f;
                this.move = 0f;
            }
            else
            {
                this.rate = 0f;
                this.move = 1f / fade;
            }
            this.CalcVolume();
        }

        public override void PlayDelay(float fade, float delay)
        {
            ospSource.PlayDelayed(delay);
            if (fade == 0f)
            {
                this.rate = 1f;
                this.move = 0f;
            }
            else
            {
                this.rate = 0f;
                this.move = 1f / fade;
            }
            this.CalcVolume();
        }

        public override void Update()
        {
            if (this.move != 0f)
            {
                this.rate += this.move * Time.deltaTime;
                if (this.move > 0f && this.rate >= 1f)
                {
                    this.rate = 1f;
                    this.move = 0f;
                }
                else if (this.move < 0f && this.rate <= 0f)
                {
                    this.rate = 0f;
                    this.move = 0f;
                    this.ospSource.Stop();
                }
            }
            this.CalcVolume();
        }

        public override void PlayDelay(AudioClip clip, bool loop, float fade, float delay)
        {
            this.ospSource.Stop();
            this.source.clip = clip;
            this.source.loop = loop;
            this.ospSource.Stop();
            this.PlayDelay(fade, delay);
        }

    }
}
