using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;

namespace PlayClubVR
{
    public class PlayClubInterpreter : GameInterpreter
    {
        /// <summary>
        /// Gets the current H scene.
        /// </summary>
        public H_Scene Scene { get; private set; }

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

        protected override void OnLevel(int level)
        {
            base.OnLevel(level);

            LookForScene();

            if(IsUIOnlyScene)
            {
                GameControl ctrl = GameObject.FindObjectOfType<GameControl>();
                ctrl.MapDataCtrl.ChangeMap(ctrl.MapDataCtrl.Datas.ElementAt(1).Value, ctrl, VRCamera.Instance.camera, false, false);
            }
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
    }
}
