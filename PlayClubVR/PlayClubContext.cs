using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Core.Visuals;

namespace PlayClubVR
{
    public class PlayClubContext : IVRManagerContext
    {

        private DefaultMaterialPalette _Materials;
        private VRSettings _Settings;
        private string[] _IgnoredCanvas = new string[0];

        public PlayClubContext()
        {
            _Materials = new DefaultMaterialPalette();
            _Settings = VRSettings.Load<VRSettings>("vr_settings.xml");

            _Materials.StandardShader = Shader.Find("Marmoset/Specular IBL");
        }

        public string GuiLayer
        {
            get
            {
                return "Ignore Raycast";
            }
        }

        public string HMDLayer
        {
            get
            {
                return "ScreenShot";
            }
        }

        public string[] IgnoredCanvas
        {
            get
            {
                return _IgnoredCanvas;
            }
        }

        public IMaterialPalette Materials
        {
            get
            {
                return _Materials;
            }
        }

        public Color PrimaryColor
        {
            get
            {
                return Color.cyan;
            }
        }
        
        public VRSettings Settings
        {
            get
            {
                return _Settings;
            }
        }

        public int UILayerMask
        {
            get
            {
                return LayerMask.GetMask("UI");
            }
        }
    }
}
