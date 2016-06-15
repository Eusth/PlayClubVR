using RootMotion;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Controls;
using VRGIN.Core;
using VRGIN.Helpers;

namespace PlayClubVR
{
    class ImpersonationHandler : ProtectedBehaviour
    {
        PlayClubActor _Actor;
        Controller.Lock _Lock1 = Controller.Lock.Invalid;
        Controller.Lock _Lock2 = Controller.Lock.Invalid;

        private Dictionary<HumanBodyBones, string> IK_MAPPING = new Dictionary<HumanBodyBones, string>()
        {
            { HumanBodyBones.Head, "Head" },
            { HumanBodyBones.LeftUpperLeg, "LegUp00_L" },
            { HumanBodyBones.LeftLowerLeg, "LegLow01_L"},
            { HumanBodyBones.LeftFoot, "Foot01_L"},
            { HumanBodyBones.RightUpperLeg, "LegUp00_R" },
            { HumanBodyBones.RightLowerLeg, "LegLow01_R"},
            { HumanBodyBones.RightFoot, "Foot01_R"},
            { HumanBodyBones.LeftLowerArm, "ArmLow01_L"},
            { HumanBodyBones.RightLowerArm, "ArmLow01_R"},
            { HumanBodyBones.LeftHand, "Hand_L"},
            { HumanBodyBones.RightHand, "Hand_R"},
            { HumanBodyBones.LeftUpperArm, "ArmUp00_L"},
            { HumanBodyBones.RightUpperArm, "ArmUp00_R"},
            { HumanBodyBones.Hips, "Hips" }
        };

        protected override void OnStart()
        {
            base.OnStart();
            if(!(VR.Settings as PlayClubSettings).FullImpersonation)
            {
                DestroyImmediate(this);
            }
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();

            var impersonatedActor = VR.Interpreter.Actors.FirstOrDefault(a => !a.HasHead) as PlayClubActor;

            if (!_Lock1.IsValid && impersonatedActor != null) {
                _Actor = impersonatedActor;
                AcquireLocks();

                Enter();
            } else if(_Lock1.IsValid && impersonatedActor == null)
            {
                Exit();
            }
        }

        private void Enter()
        {
            VR.Mode.Left.Model.gameObject.SetActive(false);
            VR.Mode.Right.Model.gameObject.SetActive(false);

            foreach(var ik in _Actor.Actor.IKs)
            {
                if(ik.ik)
                {
                    VRLog.Info(ik.ik.name);
                }
            }

            _Actor.Actor.SetupIK();

            //var biped = SetUpSimpleBiped(_Actor.Actor);

            //biped.solvers.leftHand.target = VR.Mode.Left.transform;
            //biped.solvers.rightHand.target = VR.Mode.Right.transform;

            //left.solver.target = VR.Mode.Left.transform;
            //right.solver.target = VR.Mode.Right.transform;
        }

        private void Exit()
        {
            _Lock1.Release();
            _Lock2.Release();

            VR.Mode.Left.Model.gameObject.SetActive(true);
            VR.Mode.Right.Model.gameObject.SetActive(true);

            var scene = GameObject.FindObjectOfType<H_Scene>();
            if(scene)
            {
                scene.UpdateIK(scene.LastAnimeName);
            }
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();

            if(_Lock1.IsValid)
            {
                // Update hands!
                LeftIK.solver.IKPosition = VR.Mode.Left.transform.position - VR.Mode.Left.transform.forward * 0.1f;
                LeftIK.solver.IKRotation = VR.Mode.Left.transform.rotation * Quaternion.Euler(0, 60, 0);
                RightIK.solver.IKPosition = VR.Mode.Right.transform.position - VR.Mode.Right.transform.forward * 0.1f;
                RightIK.solver.IKRotation = VR.Mode.Right.transform.rotation * Quaternion.Euler(0, -60, 0);
                RightIK.solver.bendModifier = IKSolverLimb.BendModifier.Target;
                RightIK.solver.bendModifierWeight = 0.5f;
                LeftIK.solver.bendModifier = IKSolverLimb.BendModifier.Target;
                LeftIK.solver.bendModifierWeight = 0.5f;
                RightIK.solver.bendGoal = null;
                    
            }
        }

        private LimbIK LeftIK
        {
            get
            {
                return _Actor.Actor.IKs[(int)Human.IK_TYPE.ARM_L].ik;
            }
        }

        private LimbIK RightIK
        {
            get
            {
                return _Actor.Actor.IKs[(int)Human.IK_TYPE.ARM_R].ik;
            }
        }
        
        private bool AcquireLocks()
        {
            if(VR.Mode.Left.AcquireFocus(out _Lock1) && VR.Mode.Right.AcquireFocus(out _Lock2))
            {
                return true;
            } else
            {
                if (_Lock1.IsValid) _Lock1.Release();
                if (_Lock2.IsValid) _Lock2.Release();

                return false;
            }

        }

        private BipedIK SetUpSimpleBiped(Human member)
        {
            if (member.gameObject.GetComponent<BipedIK>())
            {
                return member.gameObject.GetComponent<BipedIK>();
            }
            var ik = member.gameObject.AddComponent<BipedIK>();

            var root = member.transform.FindChild("cm_body_01");
            if (root == null) root = member.transform.FindChild("cf_body_01");
            if (root == null)
            {
                Console.WriteLine("No entry point found: {0}", member.name);
            }

            ik.references = new BipedReferences();
            ik.references.root = root;

            var spines = new List<Transform>();
            foreach (var transform in root.GetComponentsInChildren<Transform>())
            {
                string name = transform.name;
                if (name.Length > 5)
                {
                    name = name.Substring(5);
                    if (name == IK_MAPPING[HumanBodyBones.Head])
                        ik.references.head = transform;

                    // LEGS
                    else if (name == IK_MAPPING[HumanBodyBones.LeftUpperLeg])
                        ik.references.leftThigh = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.LeftLowerLeg])
                        ik.references.leftCalf = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.LeftFoot])
                        ik.references.leftFoot = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.RightUpperLeg])
                        ik.references.rightThigh = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.RightLowerLeg])
                        ik.references.rightCalf = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.RightFoot])
                        ik.references.rightFoot = transform;

                    // ARMS
                    else if (name == IK_MAPPING[HumanBodyBones.LeftUpperArm])
                        ik.references.leftUpperArm = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.LeftLowerArm])
                        ik.references.leftForearm = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.LeftHand])
                        ik.references.leftHand = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.RightUpperArm])
                        ik.references.rightUpperArm = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.RightLowerArm])
                        ik.references.rightForearm = transform;
                    else if (name == IK_MAPPING[HumanBodyBones.RightHand])
                        ik.references.rightHand = transform;

                    // HIPS
                    else if (name == IK_MAPPING[HumanBodyBones.Hips])
                        ik.references.pelvis = transform;

                    else if (name == "Spine01" || name == "Spine02" || name == "Spine03")
                        spines.Add(transform);

                }
            }

            ik.references.spine = spines.ToArray();

            //Console.WriteLine("1: " + ik.references.head);
            //Console.WriteLine("2: " + ik.references.leftCalf);
            //Console.WriteLine("3: " + ik.references.leftFoot);
            //Console.WriteLine("4: " + ik.references.leftForearm);
            //Console.WriteLine("5: " + ik.references.leftHand);
            //Console.WriteLine("6: " + ik.references.leftThigh);
            //Console.WriteLine("7: " + ik.references.leftUpperArm);
            //Console.WriteLine("8: " + ik.references.pelvis);
            //Console.WriteLine("9: " + ik.references.rightCalf);
            //Console.WriteLine("10: " + ik.references.rightFoot);
            //Console.WriteLine("11: " + ik.references.rightForearm);
            //Console.WriteLine("12: " + ik.references.rightHand);
            //Console.WriteLine("13: " + ik.references.rightThigh);
            //Console.WriteLine("14: " + ik.references.rightUpperArm);

            ik.SetToDefaults();

            foreach (var limb in ik.solvers.limbs)
            {
                limb.IKPositionWeight = 0;
                limb.IKRotationWeight = 0;
                limb.bendModifier = IKSolverLimb.BendModifier.Goal;
            }

            return ik;
        }


        //private LimbIK setUpArmIfNeeded(Human member, bool isLeft)
        //{
        //    var upperArm = isLeft
        //        ? member.gameObject.Descendants().First(t => t.name.Substring(5) == IK_MAPPING[HumanBodyBones.LeftUpperArm]).transform
        //        : member.gameObject.Descendants().First(t => t.name.Substring(5) == IK_MAPPING[HumanBodyBones.RightUpperArm]).transform;

        //    var limb = upperArm.GetComponent<LimbIK>();
        //    if (limb) return limb;

        //    limb = upperArm.gameObject.AddComponent<LimbIK>();
        //    if (isLeft)
        //    {
        //        limb.solver.bone2.transform = member.gameObject.Descendants().First(t => t.name.Substring(5) == IK_MAPPING[HumanBodyBones.LeftLowerArm]).transform;
        //        limb.solver.bone3.transform = member.gameObject.Descendants().First(t => t.name.Substring(5) == IK_MAPPING[HumanBodyBones.LeftHand]).transform;
        //    }
        //    else
        //    {
        //        limb.solver.bone2.transform = member.gameObject.Descendants().First(t => t.name.Substring(5) == IK_MAPPING[HumanBodyBones.RightLowerArm]).transform;
        //        limb.solver.bone3.transform = member.gameObject.Descendants().First(t => t.name.Substring(5) == IK_MAPPING[HumanBodyBones.RightHand]).transform;
        //    }

        //    limb.solver.SetChain(limb.solver.bone1.transform, limb.solver.bone2.transform, limb.solver.bone3.transform, member.transform);

        //    limb.solver.IKPositionWeight = 1;
        //    limb.solver.IKRotationWeight = 1;
        //    // Changing the automatic bend modifier
        //    //limb.solver.bendModifier = IKSolverLimb.BendModifier.Animation; // Will maintain the bending direction as it is animated.
        //                                                                    //limb.solver.bendModifier = IKSolverLimb.BendModifier.Target; // Will bend the limb with the target rotation
        //                                                                    //limb.solver.bendModifier = IKSolverLimb.BendModifier.Parent; // Will bend the limb with the parent bone (pelvis or shoulder)
        //                                                                    //                                                             // Will try to maintain the bend direction in the most biometrically relaxed way for the arms. 
        //                                                                    // Will not work for the legs.
        //    limb.solver.bendModifier = IKSolverLimb.BendModifier.Target;
        //    return limb;
        //}
    }
}
