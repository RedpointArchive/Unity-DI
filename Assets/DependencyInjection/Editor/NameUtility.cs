using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Code;

public static class NameUtility
{
    /// <summary>
    /// Gets the fancy (user friendly) name of the type.
    /// </summary>
    /// <returns>The fancy name.</returns>
    /// <param name="name">The original full name of the type.</param>
    public static string GetFancyName(Assembly assembly, string name)
    {
        var type = assembly.GetType(name);

        if (type != null)
        {
            var friendAttrs = type.GetCustomAttributes(typeof(ImplementationFriendlyNameAttribute), false);
            var friendAttr = friendAttrs.OfType<ImplementationFriendlyNameAttribute>().FirstOrDefault();

            if (friendAttr != null)
            {
                return friendAttr.Name;
            }
        }

        if (name.StartsWith("Code."))
        {
            name = name.Substring("Code.".Length);
        }

        if (name.EndsWith("Implementation"))
        {
            name = name.Substring(0, name.Length - "Implementation".Length);
        }

        if (name.EndsWith("Implementations"))
        {
            name = name.Substring(0, name.Length - "Implementations".Length) + "s";
        }

        var regex = new Regex("^(([a-z][A-Z])|([a-z][0-9])|([0-9][A-Z]))$", RegexOptions.Compiled);
        for (var i = 1; i < name.Length; i++)
        {
            var str = name[i - 1].ToString() + name[i];
            if (regex.IsMatch(str))
            {
                name = name.Substring(0, i) + " " + name.Substring(i);
                i++;
            }
        }

        return name;
    }
}
