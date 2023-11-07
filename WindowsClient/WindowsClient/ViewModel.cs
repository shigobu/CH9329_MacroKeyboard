using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WindowsClient.Model;

namespace WindowsClient
{
    internal class ViewModel: INotifyPropertyChanged
    {
        public ViewModel() 
        {
            ime.ImeEnabledChanged += Ime_ImeEnabledChanged;
            ime.StartListening();

            this.Keyboard = new _202MacroKeyboard();
            OpenKeybord();
        }

        /// <summary>
        /// IMEモードが変更されたときのイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ime_ImeEnabledChanged(object sender, ImeEnabledChangedEventArgs e)
        {
            if (e.ImeEnabled)
            {
                Keyboard.SetLED((byte)LedBrightness, 0);
            }
            else
            {
                Keyboard.SetLED(0, 0);
            }
        }

        private readonly Ime ime = new Ime();

        #region INotifyPropertyChangedの実装
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        /// <summary>
        /// LED点灯ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LedTestButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoLedTeat)
            {
                Keyboard.SetLED((byte)LedBrightness, (byte)LedBrightness);
            }
            else
            {
                Keyboard.SetLED(0, 0);
            }
        }

        /// <summary>
        /// LEDの明るさ変更時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LedBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DoLedTeat)
            {
                Keyboard.SetLED((byte)LedBrightness, (byte)LedBrightness);
            }
        }

        /// <summary>
        ///  再接続ボタンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReConnectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenKeybord();
        }

        /// <summary>
        /// マクロキーボードのオブジェクト
        /// </summary>
        public _202MacroKeyboard Keyboard { get; set; }

        private bool isKetboardReady = false;

        private static readonly PropertyChangedEventArgs IsKetboardReadyPropertyChangedEventArgs = new(nameof(IsKetboardReady));

        /// <summary>
        /// キーボードが接続されていて、使える状態かどうか。
        /// </summary>
        public bool IsKetboardReady
        {
            get { return isKetboardReady; }
            set
            {
                if (isKetboardReady == value) { return; }
                isKetboardReady = value;
                this.PropertyChanged?.Invoke(this, IsKetboardReadyPropertyChangedEventArgs);
                this.PropertyChanged?.Invoke(this, ShowReconnectButtonPropertyChangedEventArgs);
           }
        }

        private static readonly PropertyChangedEventArgs ShowReconnectButtonPropertyChangedEventArgs = new(nameof(ShowReconnectButton));

        /// <summary>
        /// キーボードの再接続ボタンを表示するかどうか。
        /// </summary>
        public bool ShowReconnectButton
        {
            get { return !IsKetboardReady; }
        }


        private GeneralSetting _setting = new GeneralSetting();

        private static readonly PropertyChangedEventArgs LedBrightnessPropertyChangedEventArgs = new(nameof(LedBrightness));

        /// <summary>
        /// LEDの明るさ
        /// </summary>
        public int LedBrightness
        {
            get { return _setting.LedBrightness; }
            set
            {
                if (_setting.LedBrightness == value) { return; }
                _setting.LedBrightness = value;
                this.PropertyChanged?.Invoke(this, LedBrightnessPropertyChangedEventArgs);
            }
        }

        private static readonly PropertyChangedEventArgs LedTeatPropertyChangedEventArgs = new(nameof(DoLedTeat));

        private bool doLedTest;

        public bool DoLedTeat
        {
            get { return this.doLedTest; }
            set
            {
                if (this.doLedTest == value) { return; }
                this.doLedTest = value;
                this.PropertyChanged?.Invoke(this, LedTeatPropertyChangedEventArgs);
            }
        }

        /// <summary>
        /// キーボードを開き、接続状態を更新します。
        /// </summary>
        public void OpenKeybord()
        {
            this.Keyboard.Open();
            IsKetboardReady = Keyboard.DeviceReady;
        }

        /// <summary>
        /// 終了処理を実行します。
        /// </summary>
        public void Close()
        {
            Keyboard.SetLED(0, 0);
            Keyboard.Close();
            ime.StopListening();
        }
    }
}
