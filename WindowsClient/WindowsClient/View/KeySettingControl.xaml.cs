using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsClient.View
{
    /// <summary>
    /// KeySettingControl.xaml の相互作用ロジック
    /// </summary>
    public partial class KeySettingControl : UserControl
    {
        public KeySettingControl()
        {
            InitializeComponent();
        }

        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            if (mainTab == null)
            {
                return;
            }

            if (sender == none)
            {
                mainTab.SelectedIndex = 0;
            }
            else if (sender == same)
            {
                mainTab.SelectedIndex = 1;
            }
            else if (sender == order)
            {
                mainTab.SelectedIndex = 2;
            }
            else if (sender == command)
            {
                mainTab.SelectedIndex = 3;
            }
            else if (sender == sound)
            {
                mainTab.SelectedIndex = 4;
            }
            else
            {
                mainTab.SelectedIndex = 0;
            }
        }
    }
}
