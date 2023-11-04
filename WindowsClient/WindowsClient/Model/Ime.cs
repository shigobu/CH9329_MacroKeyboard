using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WindowsClient.Model
{
    internal class Ime
    {

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();

        const int WM_IME_CONTROL = 0x283;
        const int IMC_GETCONVERSIONMODE = 1;
        const int IMC_SETCONVERSIONMODE = 2;
        const int IMC_GETOPENSTATUS = 5;
        const int IMC_SETOPENSTATUS = 6;

        const int IME_CMODE_NATIVE = 1;
        const int IME_CMODE_KATAKANA = 2;
        const int IME_CMODE_FULLSHAPE = 8;
        const int IME_CMODE_ROMAN = 16;

        public bool GetImeEnabled()
        {
            //フォアグラウンドウィンドウ取得
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
            {
                return false;
            }
            //現在のウィンドウのIMEウィンドウハンドル取得
            IntPtr imwd = ImmGetDefaultIMEWnd(hwnd);
            if (imwd == IntPtr.Zero)
            {
                return false;
            }

            //メッセージを送信してIMEモード取得
            bool imeEnabled = SendMessage(imwd, WM_IME_CONTROL, (IntPtr)IMC_GETOPENSTATUS, IntPtr.Zero) != 0;

            return imeEnabled;
        }

        #region イベント

        public delegate void ImeEnabledChangedHandler(object sender, ImeEnabledChangedEventArgs e);

        public event ImeEnabledChangedHandler? ImeEnabledChanged;

        private bool lastImeEnabled = false;

        //キャンセルトークンとか
        private CancellationTokenSource? tokenSource = null;
        private CancellationToken token;

        //ループのタスクオブジェクト
        Task? loopTask = null;

        /// <summary>
        /// IMEモードの監視を開始します
        /// </summary>
        public void StartListening()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            loopTask = Task.Run(new Action(Loop), token);
        }

        /// <summary>
        /// IMEモードの監視を終了します
        /// </summary>
        public async void StopListening()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                try
                {
                    await loopTask;
                }
                finally
                {
                    tokenSource.Dispose();
                    tokenSource = null;
                }
            }
        }

        /// <summary>
        /// MIDIメッセージを取得するタスク
        /// </summary>
        private void Loop()
        {
            while (!token.IsCancellationRequested)
            {
                bool imeEnabled = GetImeEnabled();
                if (imeEnabled == lastImeEnabled)
                {
                    //何もしない
                }
                else
                {
                    //イベント発動
                    ImeEnabledChanged?.Invoke(this, new ImeEnabledChangedEventArgs(imeEnabled));
                }
                lastImeEnabled = imeEnabled;
                Thread.Sleep(1);
            }
        }

        #endregion
    }

    public class ImeEnabledChangedEventArgs : EventArgs
    {
        public ImeEnabledChangedEventArgs(bool enabled)
        {
            ImeEnabled = enabled;
        }

        public bool ImeEnabled { get; private set; }
    }
}
