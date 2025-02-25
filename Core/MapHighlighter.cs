using ExileCore.Shared.Helpers;
using System.Collections.Generic;
using System.Drawing;
using WheresMyShitMapsAt.Types;
using Graphics = ExileCore.Graphics;

namespace WheresMyShitMapsAt.Core;
public sealed class MapHighlighter
{
    private readonly Color _badModColor;
    private readonly Color _goodModColor;
    private Graphics _graphics;

    public MapHighlighter()
    {
        _badModColor = Color.FromArgb(
            MapConstants.Alpha,
            MapConstants.Colors.Bad.Red,
            MapConstants.Colors.Bad.Green,
            MapConstants.Colors.Bad.Blue
        );

        _goodModColor = Color.FromArgb(
            MapConstants.Alpha,
            MapConstants.Colors.Good.Red,
            MapConstants.Colors.Good.Green,
            MapConstants.Colors.Good.Blue
        );
    }

    public void Initialise(Graphics graphics)
    {
        _graphics = graphics;
    }

    public void RenderHighlights(IReadOnlyDictionary<long, MapHighlightInfo> highlights)
    {
        foreach (var highlight in highlights.Values)
        {
            try
            {
                if (highlight.HasBadMod && highlight.HasGoodMod)
                {
                    _graphics.DrawRectFilledMultiColor(highlight.Item.GetClientRectCache.TopLeft.ToVector2Num(),
                        highlight.Item.GetClientRectCache.BottomRight.ToVector2Num(),
                        _goodModColor.ToSharpDx(),
                        _badModColor.ToSharpDx(),
                        _goodModColor.ToSharpDx(),
                        _badModColor.ToSharpDx());
                }
                else if (highlight.HasBadMod)
                {
                    _graphics.DrawRectFilledMultiColor(highlight.Item.GetClientRectCache.TopLeft.ToVector2Num(), highlight.Item.GetClientRectCache.BottomRight.ToVector2Num(), _badModColor.ToSharpDx(), _badModColor.ToSharpDx(), _badModColor.ToSharpDx(), _badModColor.ToSharpDx());
                }
                else if (highlight.HasGoodMod)
                {
                    _graphics.DrawRectFilledMultiColor(highlight.Item.GetClientRectCache.TopLeft.ToVector2Num(), highlight.Item.GetClientRectCache.BottomRight.ToVector2Num(), _goodModColor.ToSharpDx(), _goodModColor.ToSharpDx(), _goodModColor.ToSharpDx(), _goodModColor.ToSharpDx());
                }
            }
            catch { }
        }
    }
}