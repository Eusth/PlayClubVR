using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using VRGIN.Core;

namespace PlayClubVR
{
    [XmlRoot("Settings")]
    public class PlayClubSettings : VRSettings
    {

        public bool FullImpersonation { get; set; }

        /// <summary>
        /// Sets or gets whether or not the girls should look at the player by default.
        /// </summary>
        public bool AutoLookAtMe { get { return _AutoLookAtMe; }  set { _AutoLookAtMe = value; TriggerPropertyChanged("AutoLookAtMe"); } }
        private bool _AutoLookAtMe = true;
    }
}
