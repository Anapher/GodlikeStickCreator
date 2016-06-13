using System.Windows.Media;

namespace GodlikeStickCreator.Utilities
{
    public static class ColorExtensions
    {
        public static int ToInt(this Color color)
        {
            return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
        }

        public static Color ToColor(this int color)
        {
            return Color.FromArgb((byte) (color >> 24),
                (byte) (color >> 16),
                (byte) (color >> 8),
                (byte) (color));
        }
    }
}