namespace System.ComponentModel
{
    using System;
    using System.ComponentModel;

    public static class IComponentExtensions
    {
        public static bool IsInDesignMode(this IComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component), "Contract assertion not met: component != null");
            }

            bool designMode = false;
            ISite site = component.Site;
            if (site != null)
            {
                designMode = site.DesignMode;
            }
            return designMode;
        }

        public static bool IsInRuntimeMode(this IComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component), "Contract assertion not met: component != null");
            }

            bool flag = true;
            ISite site = component.Site;
            if (site != null)
            {
                flag = !site.DesignMode;
            }
            return flag;
        }

        public static void DisposeAlso(this IComponent component, IDisposable disposable)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component), "Contract assertion not met: component != null");
            }
            if (disposable == null)
            {
                throw new ArgumentNullException(nameof(disposable), "Contract assertion not met: disposable != null");
            }

            component.Disposed += (sender, e) =>
            {
                disposable.Dispose();
            };
        }
    }
}
