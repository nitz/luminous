namespace System
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    /// <summary>Extension methods for the Enum class.</summary>
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }
            if (!Enum.IsDefined(@this.GetType(), @this)) return null; //Contract.Requires<ArgumentOutOfRangeException>(Enum.IsDefined(@this.GetType(), @this));

            Type type = @this.GetType();
            var mis = type.GetMember(Enum.GetName(type, @this));
            if (mis.Length > 0)
            {
                var mi = mis[0];
                if (Attribute.IsDefined(mi, typeof(DescriptionAttribute)))
                {
                    DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(mi, typeof(DescriptionAttribute));
                    if (da != null)
                    {
                        return da.Description;
                    }
                }
            }
            return null;
        }
    }
}
