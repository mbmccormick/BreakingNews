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
        public static FlipTileData RenderApplicationLiveTile()
        {
            throw new NotImplementedException();
        }

        public static FlipTileData RenderLiveTile(Post data)
        {
            FlipTileData tile = new FlipTileData();

            tile.Count = 0;

            FlipTileTemplateBack image = new FlipTileTemplateBack();
            string imagePath = "/Shared/ShellContent/defaultBack.png";

            FlipTileTemplateWideBack imageWide = new FlipTileTemplateWideBack();
            string imageWidePath = "/Shared/ShellContent/defaultWideBack.png";

            image.RenderLiveTileImage(imagePath, data.content, data.FriendlyImage);
            tile.BackBackgroundImage = new Uri("isostore:" + imagePath, UriKind.Absolute);

            imageWide.RenderLiveTileImage(imageWidePath, data.content, data.FriendlyImage);
            tile.WideBackBackgroundImage = new Uri("isostore:" + imageWidePath, UriKind.Absolute);

            return tile;
        }
    }
}