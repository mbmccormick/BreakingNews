using System;
using BreakingNews.API.Models;
using BreakingNews.Imaging;
using Microsoft.Phone.Shell;

namespace BreakingNews.Common
{
    public class LiveTileManager
    {
        public static FlipTileData RenderApplicationLiveTile()
        {
            FlipTileData tile = new FlipTileData();

            tile.SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative);
            tile.BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative);
            tile.WideBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileWide.png", UriKind.Relative);

            tile.Title = "Breaking News";
            tile.Count = 0;

            return tile;
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

            tile.SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmallTransparent.png", UriKind.Relative);
            tile.BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMediumTransparent.png", UriKind.Relative);
            tile.WideBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileWideTransparent.png", UriKind.Relative);

            tile.Title = "Breaking News";
            tile.Count = 0;

            FlipTileTemplate image = new FlipTileTemplate();
            string imagePath = "/Shared/ShellContent/default" + data.id + ".png";

            FlipTileTemplateWide imageWide = new FlipTileTemplateWide();
            string imageWidePath = "/Shared/ShellContent/defaultWide" + data.id + ".png";

            image.RenderLiveTileImage(imagePath, data.name);
            tile.BackgroundImage = new Uri("isostore:" + imagePath, UriKind.Absolute);

            imageWide.RenderLiveTileImage(imageWidePath, data.name);
            tile.WideBackgroundImage = new Uri("isostore:" + imageWidePath, UriKind.Absolute);

            return tile;
        }
    }
}