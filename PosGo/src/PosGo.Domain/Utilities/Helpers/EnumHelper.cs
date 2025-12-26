using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace PosGo.Domain.Utilities.Helpers;

public static class EnumHelper<T> where T : struct, IConvertible
{
    public static bool IsDefined(int value)
    {
        Type enumType = typeof(T);
        var defined = Enum.IsDefined(enumType, value);
        if (!defined && IsEnumTypeFlags(enumType))
            defined = IsDefinedCombined(enumType, value);
        return defined;
    }

    public static bool ContainFlags(T combined, T checkagainst)
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enumerated type");

        var intValue = (int)(object)combined;
        var intLookingForFlag = (int)(object)checkagainst;
        return ((intValue & intLookingForFlag) == intLookingForFlag);
    }

    public static T Parse(string value)
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enumerated type");
        return (T)Enum.Parse(typeof(T), value);
    }

    public static IList<T> GetValues()
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enumerated type");
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    public static string GetDescription(T en)
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be of type System.Enum");

        Type type = en.GetType();
        MemberInfo[] memInfo = type.GetMember(en.ToString());
        if (memInfo.Length > 0)
        {
            object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs.Length > 0)
            {
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        return en.ToString();
    }

    public static IDictionary<string, string> GetNameAndDescription()
    {
        var enumValArray = Enum.GetValues(typeof(T));
        return enumValArray.Cast<object>().ToDictionary(val => ((T)val).ToString(CultureInfo.InvariantCulture), val => GetDescription((T)val));
    }

    private static bool IsEnumTypeFlags(Type enumType)
    {
        var attributes = enumType.GetCustomAttributes(typeof(FlagsAttribute), true);
        return attributes != null && attributes.Length > 0;
    }

    private static bool IsDefinedCombined(Type enumType, int value)
    {
        var mask = 0;
        foreach (object enumValue in Enum.GetValues(enumType))
            mask |= (int)enumValue;
        return (mask & value) == value;
    }
}
