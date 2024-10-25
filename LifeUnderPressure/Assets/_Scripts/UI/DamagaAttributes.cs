using System;
using System.ComponentModel;
using System.Reflection;

public enum DamageType
{
    [Description("trying to reach the deapth beyond your grasp.")]
    Depth,
    [Description("from crashing into the jagged rocks.")]
    Crashed,
    [Description("from the relentless bites of the cookiecutter sharks.")]
    CookieShark,
    [Description("from the crushing force of a collapsing cave.")]
    Cave
}

public static class EnumExtensions
{
    public static string ToCustomString(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute == null ? value.ToString() : attribute.Description;
    }
}


