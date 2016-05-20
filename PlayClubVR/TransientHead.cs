using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;

namespace PlayClubVR
{
    public class TransientHead : ProtectedBehaviour
    {
        private List<Renderer> rendererList = new List<Renderer>();
        private bool hidden = false;
        private Transform root;

        private Renderer[] m_tongues;
        private Human avatar;
        private Transform headTransform;
        private Transform eyesTransform;

        public bool Visible
        {
            get
            {
                return !hidden;
            }
            set
            {
                if (value)
                {
                    Console.WriteLine("SHOW");
                }
                else
                {
                    Console.WriteLine("HIDE");
                }
                SetVisibility(value);
            }
        }

        protected override void OnStart()
        {

            avatar = GetComponent<Human>();
            headTransform = GetHead(avatar);
            eyesTransform = GetEyes(avatar);

            root = headTransform.GetComponentsInParent<Transform>().Last(t => t.name.Contains("body")).parent;
            m_tongues = root.GetComponentsInChildren<SkinnedMeshRenderer>().Where(renderer => renderer.name.StartsWith("cm_O_tang") || renderer.name == "cf_O_tang").Where(tongue => tongue.enabled).ToArray();

        }
        public static Transform GetHead(Human human)
        {
            return human.headPos.GetComponentsInParent<Transform>().First(t => t.name.StartsWith("c") && t.name.Contains("J_Head"));
        }


        public static Transform GetEyes(Human human)
        {
            var eyes = human.headPos.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name.StartsWith("c") && t.name.EndsWith("Eye_ty"));
            if (!eyes)
            {
                eyes = new GameObject("cm_Eye_ty").transform;
                eyes.SetParent(GetHead(human), false);
                eyes.transform.localPosition = new Vector3(0, 0.17f, 0.05f);
            }
            return eyes;
        }

        void SetVisibility(bool visible)
        {
            if (visible)
            {
                if (hidden)
                {
                    // enable
                    //Console.WriteLine("Enabling {0} renderers", rendererList.Count);
                    foreach (var renderer in rendererList)
                    {
                        renderer.enabled = true;
                    }
                    foreach (var renderer in m_tongues)
                    {
                        renderer.enabled = true;
                    }

                }
            }
            else
            {
                if (!hidden)
                {
                    m_tongues = root.GetComponentsInChildren<SkinnedMeshRenderer>().Where(renderer => renderer.name.StartsWith("cm_O_tang") || renderer.name == "cf_O_tang").Where(tongue => tongue.enabled).ToArray();

                    // disable
                    rendererList.Clear();
                    foreach (var renderer in headTransform.GetComponentsInChildren<Renderer>().Where(renderer => renderer.enabled))
                    {
                        rendererList.Add(renderer);
                        renderer.enabled = false;
                    }

                    foreach (var renderer in m_tongues)
                    {
                        renderer.enabled = false;
                    }
                }
            }

            hidden = !visible;
        }
    }
}
