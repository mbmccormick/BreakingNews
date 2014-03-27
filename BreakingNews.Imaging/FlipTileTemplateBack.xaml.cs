using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BreakingNews.Imaging
{
    public partial class FlipTileTemplateBack : UserControl
    {
        public FlipTileTemplateBack()
        {
            InitializeComponent();
        }

        public void RenderLiveTileImage(string filename, string content, BitmapImage background, bool isApplicationTile)
        {
            this.txtContent.Text = content;
            this.imgBackground.Source = background;

            if (isApplicationTile)
                this.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 221, 54, 24));

            this.UpdateLayout();
            this.Measure(new Size(336, 336));
            this.UpdateLayout();
            this.Arrange(new Rect(0, 0, 336, 336));
            
            WriteableBitmap image = new WriteableBitmap(336, 336);
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
