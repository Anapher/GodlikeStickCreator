using System;
using System.ComponentModel;
using System.Linq;

namespace GodlikeStickCreator.Utilities
{
    public static class EnumUtilities
    {
        public static string GetDescription(Enum enumValue)
        {
            var descriptionAttribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof (DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            return descriptionAttribute != null
                ? descriptionAttribute.Description
                : enumValue.ToString();
        }
    }
}