using Microsoft.Phone.Shell;
using BreakingNews.Imaging;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using BreakingNews.API.Models;

namespace BreakingNews.Common
{
    public class LiveTileManager
    {
        public static FlipTileData RenderApplicationLiveTile()
        {
            throw new NotImplementedException();
        }

        public static FlipTileData RenderLiveTile(Topic data)
        {
            TopicItem item = new TopicItem()
            {
                id = data.id,
                name = data.name,
                is_following = data.is_following
            };

            return RenderLiveTile(item);
        }

        public static FlipTileData RenderLiveTile(TopicItem data)
        {
            FlipTileData tile = new FlipTileData();

            tile.BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative);
            tile.SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative);

            tile.Title = "Breaking News";
            tile.Count = 0;

            FlipTileTemplate image = new FlipTileTemplate();
            string imagePath = "/Shared/ShellContent/default.png";

            FlipTileTemplateWide imageWide = new FlipTileTemplateWide();
            string imageWidePath = "/Shared/ShellContent/defaultwide.png";

            image.RenderLiveTileImage(imagePath, data.name);
            tile.BackgroundImage = new Uri("isostore:" + imagePath, UriKind.Absolute);

            imageWide.RenderLiveTileImage(imageWidePath, data.name);
            tile.WideBackgroundImage = new Uri("isostore:" + imageWidePath, UriKind.Absolute);

            return tile;
        }
    }
}