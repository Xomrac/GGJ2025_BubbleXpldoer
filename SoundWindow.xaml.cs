using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Reflection;
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

namespace BubbleExe
{

    public class SoundEffect
    {
        public int probability;
        public string path;
    }


    /// <summary>
    /// Logica di interazione per SoundWindow.xaml
    /// </summary>
    public partial class SoundWindow : Window
    {
        private const float SFX_VOLUME = 1;
        private const float MUSIC_VOLUME = .2f;

        private List<SoundEffect> _sounds = new();
        bool soundFinished = true;
        bool windowWasClosed;
        private MediaPlayer _sfxPlayer;
        private MediaPlayer _soundtrackPlayer;

        public void PlaySound()
        {
            int totalPercentage = _sounds.Sum(sound => sound.probability);
            var random = new Random(DateTime.Now.Millisecond);
            var randomValue = random.Next(0, totalPercentage);
            var counter = 0;
            var pickedSound = "";
            foreach (SoundEffect sound in _sounds)
            {
                counter += sound.probability;
                if (randomValue - counter <= 0)
                {
                    pickedSound = sound.path;
                    break;
                }
            }
            Debug.WriteLine(pickedSound);
            _sfxPlayer.Open(new Uri(pickedSound));
            _sfxPlayer.Play();
        }

        private void PlaySoundtrack()
        {
            while (true)
            {
                if (windowWasClosed)
                {
                    Dispatcher.Invoke(() =>
                    {
                        _soundtrackPlayer.Stop();
                    });
                    break;
                }
                if (soundFinished)
                {
                    Dispatcher.Invoke(() =>
                    {
                        soundFinished = false;
                        var root = Assembly.GetExecutingAssembly().Location;
                        var file = new FileInfo(root);
                        var folder = file.Directory;
                        var sfxDirectory = System.IO.Path.Combine(folder.FullName, "Sounds", "SFX");
                        _soundtrackPlayer.Open(new Uri(System.IO.Path.Combine(folder.FullName, "Sounds", "Soundtrack.wav")));
                        _soundtrackPlayer.Play();
                    });

                }
            }
        }
        public SoundWindow()
        {
            InitializeComponent();
            var root = Assembly.GetExecutingAssembly().Location;
            var file = new FileInfo(root);
            var folder = file.Directory;
            var sfxDirectory = System.IO.Path.Combine(folder.FullName, "Sounds", "SFX");
            string[] files = Directory.GetFiles(sfxDirectory);
            foreach (string sound in files)
            {
                var soundName = System.IO.Path.GetFileName(sound).Replace(sfxDirectory, "");
                var probability = Convert.ToInt32(soundName.Split('_')[0]);
                _sounds.Add(new SoundEffect { probability = probability, path = sound });
            }
            _sfxPlayer = new MediaPlayer();
            _sfxPlayer.Volume = SFX_VOLUME;
            _soundtrackPlayer = new MediaPlayer();
            _soundtrackPlayer.Volume = MUSIC_VOLUME;
            _soundtrackPlayer.MediaEnded += (_, _) => { soundFinished = true; };

            _soundtrackPlayer.Open(new Uri(System.IO.Path.Combine(folder.FullName, "Sounds", "Soundtrack.wav")));
            Left = Width / 2; Top = Height / 2;
            ResizeMode = ResizeMode.NoResize;
            var thread = new Thread(PlaySoundtrack);
            thread.IsBackground = true;
            thread.Start();
            Closed += (_, _) => windowWasClosed = true;
        }
    }
}
