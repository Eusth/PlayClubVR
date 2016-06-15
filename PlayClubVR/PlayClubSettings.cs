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
    }
}
