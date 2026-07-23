using System;
using System.ComponentModel;
using System.Reflection;

namespace Wallet.API.Domain.Enums;

public static class EnumExtensions
{
    public static bool TryParseFromDescription<TEnum>(string value, out TEnum result) where TEnum : struct, Enum
    {
        if (string.IsNullOrEmpty(value))
        {
            result = default;
            return false;
        }

        if (Enum.TryParse<TEnum>(value, true, out result))
        {
            return true;
        }

        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null && string.Equals(attribute.Description, value, StringComparison.OrdinalIgnoreCase))
            {
                result = (TEnum)field.GetValue(null);
                return true;
            }
        }

        result = default;
        return false;
    }
}
