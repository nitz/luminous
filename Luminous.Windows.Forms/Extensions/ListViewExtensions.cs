namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using Luminous.Windows.Forms;

    /// <summary>Extension methods for the ListView class.</summary>
    public static class ListViewExtensions
    {
        public static void EnableSimpleSelect(this ListView @this, bool enable)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }

            Native.ListView.SetExtendedListViewStyle(@this, Native.ListView.ExtendedStyle.SimpleSelect, enable);
        }

        public static void EnableDoubleBuffering(this ListView @this, bool enable = true)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }

            Native.ListView.SetExtendedListViewStyle(@this, Native.ListView.ExtendedStyle.DoubleBuffer, enable);
        }
    }
}
