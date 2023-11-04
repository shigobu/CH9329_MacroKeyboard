using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
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

        public void LedBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DoLedTeat)
            {
                Keyboard.SetLED((byte)LedBrightness, (byte)LedBrightness);
            }
        }


        /// <summary>
        /// マクロキーボードのオブジェクト
        /// </summary>
        public _202MacroKeyboard Keyboard { get; set; } = _202MacroKeyboard.Open();


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
        /// 終了処理を実行します。
        /// </summary>
        public void Close()
        {
            Keyboard.SetLED(0, 0);
            ime.StopListening();
        }
    }
}
