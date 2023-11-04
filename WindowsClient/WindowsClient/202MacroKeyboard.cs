using OaktreeLab.USBDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsClient
{
    internal class _202MacroKeyboard : HIDSimple
    {
        private static uint vid = 0x1A86;
        private static uint pid = 0xE129;

        private static byte LEDstate = 0x80;

        private _202MacroKeyboard() : base()
        {
        
        }

        public static _202MacroKeyboard Open()
        {
            _202MacroKeyboard keyboard = new _202MacroKeyboard();
            keyboard.Open(vid, pid);
            return keyboard;
        }

        /// <summary>
        /// LEDの明るさを設定します。
        /// </summary>
        /// <param name="LED1">LED1</param>
        /// <param name="LED2">LED2</param>
        public void SetLED(byte LED1,  byte LED2)
        {
            this.Send(8, LEDstate, LED1, LED2, 0, 0, 0, 0, 0);
        }
    }
}
