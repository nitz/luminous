namespace System
{
    using System;

    /// <summary>Extension methods for the Type class.</summary>
    public static class TypeExtensions
    {
        public static string GetFullName(this Type @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }

            if (!@this.IsGenericType) return @this.FullName;

            string name = @this.FullName;
            if (name.IndexOf('`') >= 0)
            {
                name = name.Substring(0, name.IndexOf('`'));
            }

            name += '<';
            Type[] types = @this.GetGenericArguments();
            for (int i = 0; i < types.Length; i++)
            {
                if (i > 0) name += ", ";
                name += types[i].GetFullName();
            }
            name += '>';

            return name;
        }
    }
}
