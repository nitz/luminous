namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>Extension methods for the Control class.</summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// Checks whether the handle is created anywhere in the control tree (from the control up to the top-level parent).
        /// </summary>
        public static bool IsHandleCreatedAnywhere(this Control @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }
            lock (@this)
            {
                Control control = @this;
                while (control != null && !control.IsHandleCreated)
                {
                    control = control.Parent;
                }
                if (control == null)
                {
                    return false;
                }
                return control.IsHandleCreated;
            }
        }

        public static void SafeInvoke(this Control @this, Action action)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Contract assertion not met: action != null");
            }

            if (@this == null)
            {
                action();
                return;
            }

            if (!@this.IsHandleCreatedAnywhere())
            {
                throw new InvalidOperationException("The control and its parent(s) have no handle created yet.");
            }
            while (@this.Disposing || @this.IsDisposed)
            {
                if (@this.Parent == null && (!(@this is Form) || (@this as Form).Owner == null))
                {
                    throw new ObjectDisposedException("The control is currently disposing or already disposed.");
                }
                @this = @this.Parent != null
                            ? @this.Parent
                            : @this is Form
                                ? (@this as Form).Owner
                                : null;
                if (@this == null)
                {
                    throw new InvalidOperationException("The control has no parent/owner with a created handle.");
                }
            }

            if (@this.InvokeRequired)
            {
                @this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        public static TResult SafeInvoke<TResult>(this Control @this, Func<TResult> func)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this), "Contract assertion not met: @this != null");
            }
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func), "Contract assertion not met: func != null");
            }

            TResult result = default(TResult);
            SafeInvoke(@this, () =>
            {
                result = func();
            });
            return result;
        }

        public static bool IsInDesignMode(this Control target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "Contract assertion not met: target != null");
            }

            for (Control control = target; control != null; control = control.Parent)
            {
                ISite site = control.Site;
                if (site != null && site.DesignMode)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsInRuntimeMode(this Control target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "Contract assertion not met: target != null");
            }

            for (Control control = target; control != null; control = control.Parent)
            {
                ISite site = control.Site;
                if (site != null && site.DesignMode)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
