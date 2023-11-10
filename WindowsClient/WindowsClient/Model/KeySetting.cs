namespace WindowsClient.Model
{
    public struct KeySetting
    {
        public KeySetting()
        {

        }

        /// <summary>
        /// 0x00~0x7fキー番号、0x80LED情報。LED情報は、keysに格納。
        /// </summary>
        public byte state = 0;
        /// <summary>
        /// キー 先頭が0だったらカスタムボタン(PC側で動作を設定)。それ以外は、キーボードボタン。
        /// </summary>
        public readonly byte[] keys = new byte[6];
        /// <summary>
        /// 修飾キー
        /// </summary>
        public byte modifiers = 0;
    }
}