using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Core.Controls;

namespace PlayClubVR
{
    public class PlayClubController : Controller
    {
        public static PlayClubController Create()
        {
            var leftHand = new GameObject("A Controller").AddComponent<PlayClubController>();

            return leftHand;
        }


        //void OnTriggerEnter(Collider collider)
        //{
        //    var boneCollider = collider.GetComponentInChildren<DynamicBoneCollider_Custom>();
        //    if(boneCollider)
        //    {
        //        boneCollider.Collide()
        //    }
        //}
    }
}
