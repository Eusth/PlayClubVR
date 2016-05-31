using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XInputDotNetPure;

namespace GamePadClub
{
    public static class GamePadHelper
    {
        public static bool IsPressDown(ButtonState now, ButtonState before)
        {
            return now == ButtonState.Pressed && before == ButtonState.Released;
        }

        public static bool IsPressUp(ButtonState now, ButtonState before)
        {
            return now == ButtonState.Released && before == ButtonState.Pressed;
        }

        public static bool IsShoulderPressed(GamePadState state)
        {
            return state.Buttons.LeftShoulder == ButtonState.Pressed || state.Buttons.RightShoulder == ButtonState.Pressed;
        }

    }
}
