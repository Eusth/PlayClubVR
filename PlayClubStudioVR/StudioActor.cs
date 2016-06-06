using PlayClubVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Core;

namespace PlayClubStudioVR
{
    public class StudioActor : PlayClubActor
    {
        public StudioActor(Human nativeActor) : base(nativeActor)
        {
        }

        protected override void Initialize(Human actor)
        {
            base.Initialize(actor);

            if(actor.sex == Human.SEX.FEMALE)
            {
                var correcter = actor.gameObject.AddComponent<ShisenCorrecter>();
                correcter.Actor = this;
                Logger.Info("Attached Shisen Correcter to {0}", actor.CharaType);
            }
        }
    }
}
