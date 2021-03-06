﻿using System;
using BreakingNews.API.Models;
using BreakingNews.Imaging;
using Microsoft.Phone.Shell;

namespace BreakingNews.Background.Common
{
    public class LiveTileManager
    {
        public static FlipTileData RenderApplicationLiveTile(Post data)
        {
            FlipTileData tile = new FlipTileData();

            tile.Count = 0;
            tile.BackBackgroundImage = new Uri("appdata:background.png");
            tile.WideBackBackgroundImage = new Uri("appdata:background.png");

            if (data != null)
            {
                FlipTileTemplateBack image = new FlipTileTemplateBack();
                string imagePath = "/Shared/ShellContent/defaultBack" + data.id + ".png";

                FlipTileTemplateWideBack imageWide = new FlipTileTemplateWideBack();
                string imageWidePath = "/Shared/ShellContent/defaultWideBack" + data.id + ".png";

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
            tile.BackBackgroundImage = new Uri("appdata:background.png");
            tile.WideBackBackgroundImage = new Uri("appdata:background.png");

            if (data != null)
            {
                FlipTileTemplateBack image = new FlipTileTemplateBack();
                string imagePath = "/Shared/ShellContent/defaultBack" + data.id + ".png";

                FlipTileTemplateWideBack imageWide = new FlipTileTemplateWideBack();
                string imageWidePath = "/Shared/ShellContent/defaultWideBack" + data.id + ".png";

                image.RenderLiveTileImage(imagePath, data.content, data.FriendlyImage, false);
                tile.BackBackgroundImage = new Uri("isostore:" + imagePath, UriKind.Absolute);

                imageWide.RenderLiveTileImage(imageWidePath, data.content, data.FriendlyImage, false);
                tile.WideBackBackgroundImage = new Uri("isostore:" + imageWidePath, UriKind.Absolute);
            }

            return tile;
        }
    }
}