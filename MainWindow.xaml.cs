using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace BubbleExe
{
    public class ScoreManager
    {
        public const int GAME_DURATION = 60000;
        public int _score;

        public event Action TimeFinished;

        private Label _scoreLabel;
        private Label _timeLabel;

        public ScoreManager(Label scoreLabel, Label timeLabel)
        {
            _scoreLabel = scoreLabel;
            _timeLabel = timeLabel;
            _score = 0;
           
        }

        public void StartCountDown()
        {
            var thread = new Thread(CountDown);
            thread.IsBackground = true;
            thread.Start();
        }

        private void CountDown()
        {
            var elapsedTime = 0;
            var startTime = DateTime.Now;
            while (elapsedTime < GAME_DURATION)
            {
                _timeLabel.Dispatcher.Invoke(() =>
                {
                    _timeLabel.Content = "Time: " + (GAME_DURATION - elapsedTime) / 1000;
                    _timeLabel.UpdateLayout();
                });
                var currentTime = DateTime.Now;
                elapsedTime = (int)currentTime.Subtract(startTime).TotalMilliseconds;
            }
            TimeFinished?.Invoke();
        }


        
        public void IncrementScore()
        {
           _score++;
            _scoreLabel.Content = "Score: " + _score;
           _scoreLabel.UpdateLayout();
        }
    }

    public partial class MainWindow : Window
    {

        private const int WIDTH_OFFSET = 200;
        private const int HEIGHT_OFFSET = 100;
        private List<BubbleWindow> _bubbles = new List<BubbleWindow>();
        private int score;
        private bool _startCounting;
        private const int STARTING_BUBBLES = 100;
        private ScoreManager _scoreManager;
        private bool _gameFinished;
        private SoundWindow _soundWindow;

            private List<BubbleImages> _imagesPath = new List<BubbleImages> ();


        private void OnGameEnd()
        {
            _gameFinished = true;
            for (int i = _bubbles.Count - 1; i >= 0; i--)
            {
                BubbleWindow? bubble = _bubbles[i];
                bubble?.Dispatcher.Invoke(() =>
                    {
                        bubble.Close();
                    });
            }

            this.Dispatcher.Invoke(() =>
            {
                Close();
            });

            MessageBox.Show(
            $"Your score is: {_scoreManager._score}",
            "GAME OVER!",
            MessageBoxButton.OK,
            MessageBoxImage.Information,
            MessageBoxResult.None,
            MessageBoxOptions.DefaultDesktopOnly);
        }
        public MainWindow()
        {
            InitializeComponent();
            InitializeImages();
            _soundWindow = new SoundWindow();
            _soundWindow.Show();
            _scoreManager = new ScoreManager(Score, Timer);
            _scoreManager.TimeFinished += OnGameEnd;
            MainMenu.Opacity = 1;
            GameplayGraphic.Opacity = 0;
            Closed += (a, b) =>
            {
                _gameFinished = true;
                _soundWindow?.Close();
                foreach (var bubble in _bubbles)
                {
                    bubble.Close();
                }
            };

        }

        private void InitializeImages()
        {
            var root = Assembly.GetExecutingAssembly().Location;
            var file = new FileInfo(root);
            var folder = file.Directory;
            string[] subfolders = Directory.GetDirectories(Path.Combine(folder.FullName, "Bubbles"));
            foreach (string subfolder in subfolders)
            {
                string[] files = Directory.GetFiles(subfolder);
                string normalStatePath = files[0];
                string explodedStatePath = files[1];
                BubbleImages newBubbleImages = new BubbleImages(normalStatePath, explodedStatePath);
                _imagesPath.Add(newBubbleImages);
            }
        }


        
        private void CreateBubble()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var randomImagesIndex = random.Next(0, _imagesPath.Count);
            var randomImages = _imagesPath[randomImagesIndex];
            var newBubbleWindow = new BubbleWindow(randomImages,_soundWindow);
            _bubbles.Add(newBubbleWindow);
            newBubbleWindow.Show();
            newBubbleWindow.Title = "Bubble";
            var randomPosition = GetRandomPointOnScreen();
            newBubbleWindow.Top = randomPosition.Y;
            newBubbleWindow.Left = randomPosition.X;
            newBubbleWindow.BubblePopped += (bubble) => { _bubbles.Remove(bubble); };
            newBubbleWindow.Closed += (a, b) =>
            {
                if(!_gameFinished)
                {
                    CreateBubble();
                    _scoreManager.IncrementScore();
                }
            };
        }

        private Point GetRandomPointOnScreen()
        {
            var point = new Point();

            var random = new Random(DateTime.Now.Millisecond);
            point.X = random.Next(0, (int)SystemParameters.PrimaryScreenWidth - WIDTH_OFFSET);
            point.Y = random.Next(0, (int)SystemParameters.PrimaryScreenHeight - HEIGHT_OFFSET);
            return point;
        }



        private void SpamWindows()
        {
            for (int i = 0;i < STARTING_BUBBLES;i++)
            {
               CreateBubble();
            }
            _startCounting = true;
            _scoreManager.StartCountDown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Hide();
            MainMenu.Opacity = 0;
            GameplayGraphic.Opacity = 1;
            SpamWindows();
        }
    }
}