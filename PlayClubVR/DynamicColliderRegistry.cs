using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Core;

namespace PlayClubVR
{
    public static class DynamicColliderRegistry
    {
        private static List<DynamicBoneCollider> _Colliders = new List<DynamicBoneCollider>();
        public static void Register(DynamicBoneCollider collider)
        {
            _Colliders.Add(collider);

            foreach (var actor in VR.Interpreter.Actors.OfType<PlayClubActor>())
            {
                actor.RegisterDynamicBoneCollider(collider);
            }
        }

        public static IEnumerable<DynamicBoneCollider> Colliders
        {
            get
            {
                return _Colliders;
            }
        }

        public static void Clear()
        {
            _Colliders.Clear();
        }
    }
}
