using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;

namespace PlayClubVR
{

    public static class DynamicColliderRegistry
    {
        private static IDictionary<DynamicBoneCollider, Predicate<IDynamicBoneWrapper>> _Colliders = new Dictionary<DynamicBoneCollider, Predicate<IDynamicBoneWrapper>>();
        private static IList<IDynamicBoneWrapper> _Bones = new List<IDynamicBoneWrapper>();

        public static IEnumerable<IDynamicBoneWrapper> Bones { get { return _Bones; } }

        public static void RegisterCollider(DynamicBoneCollider collider, Predicate<IDynamicBoneWrapper> targetSelector = null)
        {
            if (targetSelector == null) targetSelector = (bone) => true;

            _Colliders[collider] = targetSelector;

            foreach (var bone in _Bones)
            {
                Correlate(bone, collider, targetSelector);
            }
        }

        private static void Correlate(IDynamicBoneWrapper bone, DynamicBoneCollider collider, Predicate<IDynamicBoneWrapper> targetSelector)
        {
            if (targetSelector(bone))
            {
                if (!bone.Colliders.Contains(collider))
                {
                    bone.Colliders.Add(collider);
                }
            }
            else
            {
                bone.Colliders.Remove(collider);
            }
        }

        private static void Register(IDynamicBoneWrapper wrapper)
        {
            _Bones.Add(wrapper);
            
            foreach(var colliderPair in _Colliders)
            {
                Correlate(wrapper, colliderPair.Key, colliderPair.Value);
            }
        }

        public static void RegisterDynamicBone(DynamicBone bone)
        {
            Register(new DynamicBoneWrapper(bone));
        }

        public static void RegisterDynamicBone(DynamicBone_Custom bone)
        {
            Register(new DynamicBone_CustomWrapper(bone));
        }

        public static void Clear()
        {
            _Colliders.Clear();
            _Bones.Clear();
        }

        public static Predicate<IDynamicBoneWrapper> GetCondition(DynamicBoneCollider key)
        {
            return _Colliders[key];
        }
    }


    public interface IDynamicBoneWrapper
    {
        MonoBehaviour Bone
        {
            get;
        }

        Transform Root
        {
            get;
        }

        List<DynamicBoneCollider> Colliders { get; }
    }

    public class DynamicBoneWrapper : IDynamicBoneWrapper
    {
        public MonoBehaviour Bone
        {
            get;
            private set;
        }

        public List<DynamicBoneCollider> Colliders
        {
            get
            {
                return (Bone as DynamicBone).m_Colliders;
            }
        }

        public Transform Root
        {
            get
            {
                return (Bone as DynamicBone).m_Root;
            }
        }

        public DynamicBoneWrapper(DynamicBone bone)
        {
            Bone = bone;
        }
    }

    public class DynamicBone_CustomWrapper : IDynamicBoneWrapper
    {
        public MonoBehaviour Bone
        {
            get;
            private set;
        }

        public List<DynamicBoneCollider> Colliders
        {
            get
            {
                return (Bone as DynamicBone_Custom).m_Colliders;
            }
        }

        public Transform Root
        {
            get
            {
                return (Bone as DynamicBone_Custom).m_Nodes[0];
            }
        }

        public DynamicBone_CustomWrapper(DynamicBone_Custom bone)
        {
            Bone = bone;
        }
    }
}
