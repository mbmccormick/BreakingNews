using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BreakingNews.Imaging
{
    public partial class FlipTileTemplateWideBack : UserControl
    {
        public FlipTileTemplateWideBack()
        {
            InitializeComponent();
        }

        public void RenderLiveTileImage(string filename, string content, BitmapImage background, bool isApplicationTile)
        {
            this.txtContent.Text = content;
            this.imgBackground.Source = background;

            if (isApplicationTile)
                this.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 221, 54, 24));

            this.Measure(new Size(691, 336));
            this.Arrange(new Rect(0, 0, 691, 336));
            this.UpdateLayout();
            
            WriteableBitmap image = new WriteableBitmap(691, 336);
            image.Render(this, null);
            image.Invalidate();

            using (IsolatedStorageFile output = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (output.FileExists(filename))
                    output.DeleteFile(filename);

                using (var stream = output.OpenFile(filename, System.IO.FileMode.OpenOrCreate))
                {
                    image.WritePNG(stream);
                }
            }
        }
    }
}
