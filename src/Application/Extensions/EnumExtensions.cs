using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum enumValue)
    {
        FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo == null)
        {
            return enumValue.ToString(); // Fallback if no field info
        }

        DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes != null && attributes.Length > 0)
        {
            return attributes[0].Description;
        }
        else
        {
            return enumValue.ToString(); // Fallback if no description attribute
        }
    }
}
