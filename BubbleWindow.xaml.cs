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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace BubbleExe
{
    /// <summary>
    /// Logica di interazione per BubbleWindow.xaml
    /// </summary>
    public partial class BubbleWindow : Window
    {

        private BubbleImages _images;
        private SoundWindow _sound;

        public event Action<BubbleWindow> BubblePopped;

        public BubbleWindow(BubbleImages images, SoundWindow _soundWindow)
        {
            InitializeComponent();
            _sound = _soundWindow;
            _images = images;
            Image.Source = _images.normalState;
          
            UpdateLayout();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var random = new Random(DateTime.Now.Millisecond);
            double randomValue = 1.2f - (random.NextDouble() * 0.4f);

            Width *= randomValue;
            Height *= randomValue;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            BubblePopped?.Invoke(this);
            _sound?.PlaySound();
            Image.Source = _images.explodedState;
            UpdateLayout();
            await Task.Run(() =>
            {
                Thread.Sleep(250);
            });
            Close();
        }
    }
}
