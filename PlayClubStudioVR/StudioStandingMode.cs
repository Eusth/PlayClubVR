using PlayClubVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Core;

namespace PlayClubStudioVR
{
    class StudioStandingMode : PlayClubStandingMode
    {
        protected override void ChangeMode()
        {
            VR.Manager.SetMode<StudioSeatedMode>();
        }

        protected override void CreateControllers()
        {
            base.CreateControllers();
        }

        public override IEnumerable<Type> Tools
        {
            get
            {
                return base.Tools
                    // Remove unneeded
                    .Except((new Type[] { typeof(PlayTool), typeof(MaestroTool) }))
                    // Add new
                    .Concat(new Type[] { typeof(IKTool) });
            }
        }
    }
}
