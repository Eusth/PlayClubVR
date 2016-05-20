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

            Left.gameObject.AddComponent<GripHandler>();
            Right.gameObject.AddComponent<GripHandler>();
        }
    }
}
