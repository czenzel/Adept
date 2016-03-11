using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.System;

namespace Microsoft.Adept.Input
{
    static public class GamepadExtensions
    {
        /// <summary>
        /// Attempts to convert a <see cref="VirtualKey"/> to its corresponding <see cref="GamepadButtons"/> enum value.
        /// </summary>
        /// <param name="key">
        /// The <see cref="VirtualKey"/> to convert.
        /// </param>
        /// <returns>
        /// The converted <see cref="GamepadButtons"/> enum value, which may be <see cref="GamepadButtons.None"/>.
        /// </returns>
        static public GamepadButtons ToButton(this VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadB:
                    return GamepadButtons.B;
                case VirtualKey.GamepadDPadDown:
                    return GamepadButtons.A;
                case VirtualKey.GamepadDPadLeft:
                    return GamepadButtons.A;
                case VirtualKey.GamepadDPadRight:
                    return GamepadButtons.A;
                case VirtualKey.GamepadDPadUp:
                    return GamepadButtons.A;
                case VirtualKey.GamepadLeftShoulder:
                    return GamepadButtons.A;
                case VirtualKey.GamepadLeftThumbstickButton:
                    return GamepadButtons.A;
                case VirtualKey.GamepadLeftThumbstickDown:
                    return GamepadButtons.A;
                case VirtualKey.GamepadLeftThumbstickRight:
                    return GamepadButtons.A;
                case VirtualKey.GamepadLeftThumbstickUp:
                    return GamepadButtons.A;
                case VirtualKey.GamepadLeftTrigger:
                    return GamepadButtons.A;
                case VirtualKey.GamepadMenu:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;
                case VirtualKey.GamepadA:
                    return GamepadButtons.A;

                default:
                    return GamepadButtons.None;
            }
        }
    }
}
