using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsClient.Model
{
    internal class _202MacroKeyboard : HIDSimple
    {
        private static uint vid = 0x1A86;
        private static uint pid = 0xE129;

        private static byte LEDstate = 0x80;

        public _202MacroKeyboard() : base()
        {

        }

        /// <summary>
        /// キーボードをオープンします。
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            return Open(vid, pid);
        }

        /// <summary>
        /// LEDの明るさを設定します。
        /// </summary>
        /// <param name="LED1">LED1</param>
        /// <param name="LED2">LED2</param>
        public void SetLED(byte LED1, byte LED2)
        {
            if (DeviceReady)
            {
                Send(8, LEDstate, LED1, LED2);
            }
        }

        /// <summary>
        /// キー設定をキーボードへ書き込みます。
        /// </summary>
        /// <param name="keySetting">キー設定</param>
        public void WriteKeySetting(KeySetting keySetting)
        {
            byte[] sendData = new byte[9];
            sendData[0] = 8;
            sendData[1] = keySetting.state;
            sendData[2] = keySetting.keys[0];
            sendData[3] = keySetting.keys[1];
            sendData[4] = keySetting.keys[2];
            sendData[5] = keySetting.keys[3];
            sendData[6] = keySetting.keys[4];
            sendData[7] = keySetting.keys[5];
            sendData[8] = keySetting.modifiers;

            Send(sendData);
        }
    }
}
