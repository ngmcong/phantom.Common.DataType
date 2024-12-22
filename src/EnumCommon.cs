using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public static class EnumExtension
{
    public static string GetDisplay(this Enum value)
    {
        if (value == null) return null;
        try
        {
            var attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault() as DisplayAttribute;
            return attribute == null ? value.ToString() : attribute.Name;
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }
    public static string GetDescription(this Enum value)
    {
        var attribute = value.GetType()
            .GetField(value.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
    public static object[] GetValuesAndDescriptions(Type enumType)
    {
        var values = Enum.GetValues(enumType).Cast<object>();
        var valuesAndDescriptions = from value in values
                                    select new
                                    {
                                        Value = value,
                                        Description = value.GetType()
                                            .GetMember(value.ToString())[0]
                                            .GetCustomAttributes(true)
                                            .OfType<DescriptionAttribute>()
                                            .First()
                                            .Description
                                    };
        return valuesAndDescriptions.ToArray();
    }
    public static T GetEnumFromValue<T>(object aValue)
    {
        return (T)Enum.ToObject(typeof(T), aValue);
    }

    /// <summary>Trả về kiểu dữ liệu thực tế của một kiểu dữ liệu (hay dùng cho Nullable).</summary>
    /// Changes:
    /// - 210120: 1.0.0.0 [CNM]: Khởi tạo.
    /// <param name="type">Kiểu dữ liệu.</param>
    /// <returns>Kiểu dữ liệu thực tế.</returns>
    public static Type GenericType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return type.GetGenericArguments()[0];
        }
        return type;
    }
}