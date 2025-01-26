using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BubbleExe
{
    public class BubbleImages
    {
        public BitmapImage normalState;
        public BitmapImage explodedState;

        public BubbleImages(string normalStatePath,string explodedStatePath)
        {
            normalState = new BitmapImage(new Uri(normalStatePath, UriKind.RelativeOrAbsolute));
            explodedState = new BitmapImage(new Uri(explodedStatePath, UriKind.RelativeOrAbsolute));
        }
    }
}
