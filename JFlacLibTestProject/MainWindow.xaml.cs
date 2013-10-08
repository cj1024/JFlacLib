using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JFlacLib.JFlac;

namespace JFlacLibTestProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly OpenFileDialog _dialog;
        public MainWindow()
        {
            InitializeComponent();
            _dialog = new OpenFileDialog() { Multiselect = false, Filter = Properties.Resources.FlacOpenFileDialogFilter };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(_dialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                var path= _dialog.FileName;
                var decoder = new JFlacLib.JFlac.Apps.Decoder();
                decoder.Decode(path,path.Replace("flac","wav"));
            }
        }
    }
}
