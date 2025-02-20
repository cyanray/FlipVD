using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace FlipVD
{
    class KeyboardSimulation
    {
        public enum KeyDef
        {
            Ctrl = VIRTUAL_KEY.VK_CONTROL,
            LeftWin = VIRTUAL_KEY.VK_LWIN,
            LeftArrow = VIRTUAL_KEY.VK_LEFT,
            RightArrow = VIRTUAL_KEY.VK_RIGHT,
            Tab = VIRTUAL_KEY.VK_TAB
        }

        public static void KeyPress(List<KeyDef> keys)
        {
            Span<INPUT> inputs = stackalloc INPUT[keys.Count * 2];
            int index = 0;

            // Press keys
            foreach (var key in keys)
            {
                inputs[index] = new INPUT
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous = new()
                    {
                        ki = new KEYBDINPUT { wVk = (VIRTUAL_KEY)key }
                    }
                };
                index++;
            }

            // Release keys in reverse order
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                inputs[index] = new INPUT
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous = new()
                    {
                        ki = new KEYBDINPUT { wVk = (VIRTUAL_KEY)keys[i], dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP }
                    }
                };
                index++;
            }

            PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        }
    }
}