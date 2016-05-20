using PlayClubVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;

namespace PlayClubStudioVR
{
    public class StudioInterpreter : GameInterpreter
    {
        public StudioScene _Scene;

        private List<IActor> _Actors = new List<IActor>();
        public override IEnumerable<IActor> Actors
        {
            get { return _Actors; }
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitScene();
        }


        protected override void OnLevel(int level)
        {
            base.OnLevel(level);
            InitScene();
        }
        
        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_Scene)
            {
                CleanActors();
            }
            else
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

            foreach (var member in _Scene.charas.Humans)
            {
                AddActor(member.human);
            }
        }

        private void AddActor(Human member)
        {
            if (!member.GetComponent<Marker>())
            {
                _Actors.Add(new StudioActor(member));
            }
        }

        private void InitScene()
        {
            _Scene = GameObject.FindObjectOfType<StudioScene>();

        }
    }
}
