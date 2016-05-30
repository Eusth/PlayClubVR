using PlayClubVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Core;

namespace PlayClubStudioVR
{
    class StudioSeatedMode : PlayClubSeatedMode
    {
        protected override void ChangeMode()
        {
            VR.Manager.SetMode<StudioStandingMode>();
        }

        public override IEnumerable<Type> Tools
        {
            get
            {
                // Remove tools that ain't needed
                return base.Tools.Except((new Type[] { typeof(PlayTool), typeof(MaestroTool) }));
            }
        }
    }
}
