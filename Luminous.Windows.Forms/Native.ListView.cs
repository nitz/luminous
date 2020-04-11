namespace Luminous.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static partial class Native
    {
        public static class ListView
        {
            public enum ExtendedStyle
            {
                SimpleSelect = 0x00100000,
                DoubleBuffer = 0x00010000,
            }

            public static void SetExtendedListViewStyle(System.Windows.Forms.ListView @this, ExtendedStyle style, bool enable = true)
            {
                if (@this == null)
                {
                    throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
                }

                Messages.Send(new HandleRef(@this, @this.Handle), (uint)Messages.ListView.SetExtendedListViewStyle, new IntPtr((int)style), enable ? new IntPtr((int)style) : IntPtr.Zero);
            }
        }
    }
}
