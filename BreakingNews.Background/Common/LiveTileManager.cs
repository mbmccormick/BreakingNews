using Microsoft.Phone.Shell;
using BreakingNews.Imaging;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using BreakingNews.API.Models;

namespace BreakingNews.Background.Common
{
    public class LiveTileManager
    {
        public static FlipTileData RenderApplicationLiveTile(Post data)
        {
            FlipTileData tile = new FlipTileData();

            tile.Count = 0;
            tile.BackBackgroundImage = null;
            tile.WideBackBackgroundImage = null;

            if (data != null)
            {
                FlipTileTemplateBack image = new FlipTileTemplateBack();
                string imagePath = "/Shared/ShellContent/defaultBack.png";

                FlipTileTemplateWideBack imageWide = new FlipTileTemplateWideBack();
                string imageWidePath = "/Shared/ShellContent/defaultWideBack.png";

                image.RenderLiveTileImage(imagePath, data.content, data.FriendlyImage, true);
                tile.BackBackgroundImage = new Uri("isostore:" + imagePath, UriKind.Absolute);

                imageWide.RenderLiveTileImage(imageWidePath, data.content, data.FriendlyImage, true);
                tile.WideBackBackgroundImage = new Uri("isostore:" + imageWidePath, UriKind.Absolute);
            }

            return tile;
        }

        public static FlipTileData RenderLiveTile(Post data)
        {
            FlipTileData tile = new FlipTileData();

            tile.Count = 0;
            tile.BackBackgroundImage = null;
            tile.WideBackBackgroundImage = null;

            if (data != null)
            {
                FlipTileTemplateBack image = new FlipTileTemplateBack();
                string imagePath = "/Shared/ShellContent/defaultBack.png";

                FlipTileTemplateWideBack imageWide = new FlipTileTemplateWideBack();
                string imageWidePath = "/Shared/ShellContent/defaultWideBack.png";

                image.RenderLiveTileImage(imagePath, data.content, data.FriendlyImage, false);
                tile.BackBackgroundImage = new Uri("isostore:" + imagePath, UriKind.Absolute);

                imageWide.RenderLiveTileImage(imageWidePath, data.content, data.FriendlyImage, false);
                tile.WideBackBackgroundImage = new Uri("isostore:" + imageWidePath, UriKind.Absolute);
            }

            return tile;
        }
    }
}