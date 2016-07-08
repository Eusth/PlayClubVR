using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRGIN.Core;

namespace PlayClubVR
{
    public class PlayClubInterpreter : GameInterpreter
    {
        /// <summary>
        /// Gets the current H scene.
        /// </summary>
        public H_Scene Scene { get; private set; }

        private readonly int pngLayer = LayerMask.NameToLayer("PNG");
        private List<IActor> _Actors = new List<IActor>();
        public override IEnumerable<IActor> Actors
        {
            get { return _Actors; }
        }

        protected override void OnStart()
        {
            base.OnStart();

            gameObject.AddComponent<OSPManager>();

            LookForScene();

            if (IsUIOnlyScene)
            {
                GameControl ctrl = GameObject.FindObjectOfType<GameControl>();
                ctrl.MapDataCtrl.ChangeMap(ctrl.MapDataCtrl.Datas.ElementAt(1).Value, ctrl, VRCamera.Instance.camera, false, false);
            }
            
        }

        public override bool IsIgnoredCanvas(Canvas canvas)
        {
            return canvas.gameObject.layer == pngLayer;
        }


        protected override void OnLevel(int level)
        {
            base.OnLevel(level);
           
            LookForScene();

            if(IsUIOnlyScene)
            {
                GameControl ctrl = GameObject.FindObjectOfType<GameControl>();
                ctrl.MapDataCtrl.ChangeMap(ctrl.MapDataCtrl.Datas.ElementAt(1).Value, ctrl, VRCamera.Instance.camera, false, false);
            }

            //var oldShadowCaster = VR.Camera.SteamCam.head.FindChild("ShadowCaster");
            //if (oldShadowCaster)
            //{
            //    DestroyImmediate(oldShadowCaster);
            //}
            //var shadowCaster = GameObject.Find("ShadowCaster");
            //if(shadowCaster)
            //{
            //    VRLog.Info("FOund shadow caster");
            //    //shadowCaster.transform.SetParent(VR.Camera.SteamCam.head);
            //    shadowCaster.transform.GetChild(0).gameObject.AddComponent<SteamVR_CameraFlip>();
            //}
        }

        private bool IsUIOnlyScene
        {
            get
            {
                return !GameObject.FindObjectOfType<IllusionCamera>();
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Scene)
            {
                CleanActors();
            } else
            {
                _Actors.Clear();
            }
        }

        /// <summary>
        /// Removes dead entries from actors list and adds new ones.
        /// </summary>
        private void CleanActors()
        {
            _Actors = Actors.Where(a => a.IsValid).ToList();

            foreach (var member in Scene.Members)
            {
                AddActor(member);
            }
        }

        private void AddActor(Human member)
        {
            if (!member.GetComponent<Marker>())
            {
                _Actors.Add(new PlayClubActor(member));
            }
        }
        
        /// <summary>
        /// Looks for the scene object.
        /// </summary>
        private void LookForScene()
        {
            Scene = GameObject.FindObjectOfType<H_Scene>();

            if(!Scene)
            {
                //// Search for actors
                //foreach (var member in GameObject.FindObjectsOfType<Human>())
                //{
                //    AddActor(member);
                //}
            }
        }

        public override bool IsBody(Collider collider)
        {
            return collider.gameObject.layer == LayerMask.NameToLayer("ToLiquidCollision") 
                && collider.transform.parent.name.StartsWith("cf_") || collider.transform.parent.name.StartsWith("cm_");
        }


        public void ChangePose(int direction)
        {
            if (!Scene) return;
            
            int currentIndex = Scene.StyleMgr.StyleList.IndexOf(Scene.StyleMgr.nowStyle);
            int nextIndex = ((currentIndex + Scene.StyleMgr.StyleList.Count) + direction) % Scene.StyleMgr.StyleList.Count;

            ChangeStyle(Scene.StyleMgr.StyleList[nextIndex].file);
        }

        internal void IncreaseSpeed(float v)
        {
            Scene.Pad.ChangeSpeed(Mathf.Clamp(Scene.Pad.speed + v, Scene.Pad.SpeedMin, Scene.Pad.SpeedMax));
        }

        public void ChangeStyle(string name)
        {
            if (!Scene) return;

            Scene.ChangeStyle(name);
            Scene.StyleToGUI(name);
            Scene.CrossFadeStart();
        }

        public void Ejaculate()
        {
            if (!Scene) return;

            if (Scene.FemaleGage.IsHigh())
            {
                Scene.StateMgr.SetXtc(H_Pad.XTC.SYNC);
            }
            else if (Scene.Members[0].sex == Human.SEX.MALE)
            {
                Scene.StateMgr.SetXtc(H_Pad.XTC.M_OUT);
            }
            else
            {
                Scene.StateMgr.SetXtc(H_Pad.XTC.F);
            }
        }

        public void SetSpeed(float val)
        {
            if (!Scene) return;
            
            Scene.Pad.ChangeSpeed(val * 5);
        }

        public void ToggleOrgasmLock(bool? enabled = null)
        {
            if (!Scene) return;
            //Console.WriteLine(!scene.FemaleGage.Lock ? "Lock" : "Unlock");
            // This also updates the GUI!
            var toggle = GameObject.Find("XtcLock").GetComponent<Toggle>();

            if (enabled == null || (toggle.isOn != enabled.Value))
                toggle.isOn = !Scene.FemaleGage.Lock;
            //scene.FemaleGage.ChangeLock(!scene.FemaleGage.Lock);
        }

        public void TogglePiston(bool? start = null)
        {
            if (!Scene) return;

            if (start == null || (Scene.Pad.pistonToggle.isOn != start.Value))
                Scene.Pad.pistonToggle.OnPointerClick(new PointerEventData(EventSystem.current));

        }

        public void ToggleGrind(bool? start = null)
        {
            if (!Scene) return;

            if (start == null || (Scene.Pad.grindToggle.isOn != start.Value))
                Scene.Pad.grindToggle.OnPointerClick(new PointerEventData(EventSystem.current));
        }


    }
}
