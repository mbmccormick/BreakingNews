using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BreakingNews.Imaging
{
    public partial class FlipTileTemplate : UserControl
    {
        public FlipTileTemplate()
        {
            InitializeComponent();
        }

        public void RenderLiveTileImage(string filename, string content)
        {
            this.txtContent.Text = content;

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
