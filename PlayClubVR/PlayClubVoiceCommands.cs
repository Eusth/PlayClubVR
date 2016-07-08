using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Controls.Speech;

namespace PlayClubVR
{
    public class PlayClubVoiceCommand : VoiceCommand
    {
        public static readonly VoiceCommand NextAnimation = new PlayClubVoiceCommand("next");
        public static readonly VoiceCommand PreviousAnimation = new PlayClubVoiceCommand("previous");
        public static readonly VoiceCommand StartAnimation = new PlayClubVoiceCommand("start");
        public static readonly VoiceCommand StopAnimation = new PlayClubVoiceCommand("stop");
        public static readonly VoiceCommand Faster = new PlayClubVoiceCommand("faster", "increase speed");
        public static readonly VoiceCommand Slower = new PlayClubVoiceCommand("slower", "decrease speed");
        public static readonly VoiceCommand Climax = new PlayClubVoiceCommand("climax", "come");
        public static readonly VoiceCommand DisableClimax = new PlayClubVoiceCommand("disable climax", "disable orgasm");
        public static readonly VoiceCommand EnableClimax = new PlayClubVoiceCommand("enable climax", "enable orgasm");

        protected PlayClubVoiceCommand(params string[] texts) : base(texts)
        {
        }
    }
}
