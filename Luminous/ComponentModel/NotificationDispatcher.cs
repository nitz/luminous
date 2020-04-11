namespace Luminous.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public class NotificationDispatcher
    {
        #region Constructor & properties

        private readonly Dictionary<string, PropertyChangingEventHandler> _changingHandlers;
        private readonly Dictionary<string, PropertyChangedEventHandler> _changedHandlers;
        private void ObjectInvariant()
        {
        }

        public NotificationDispatcher()
        {
            _changingHandlers = new Dictionary<string, PropertyChangingEventHandler>();
            _changedHandlers = new Dictionary<string, PropertyChangedEventHandler>();
        }

        #endregion

        #region Methods

        public void RegisterNotifyingObject(NotifyingObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Contract assertion not met: obj != null");
            }

            RegisterNotifyingObject((INotifyPropertyChanging)obj);
            RegisterNotifyingObject((INotifyPropertyChanged)obj);
        }

        public void RegisterNotifyingObject(INotifyPropertyChanging obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Contract assertion not met: obj != null");
            }

            obj.PropertyChanging += (sender, e) => ProcessNotification(sender, e);
        }

        public void RegisterNotifyingObject(INotifyPropertyChanged obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Contract assertion not met: obj != null");
            }

            obj.PropertyChanged += (sender, e) => ProcessNotification(sender, e);
        }

        public void RegisterNotificationHandlingMethods(object handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "Contract assertion not met: handler != null");
            }

            var methods = handler.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            foreach (var method in methods)
            {
                if (method.ContainsGenericParameters || method.IsGenericMethod) continue;
                var parameters = method.GetParameters();
                if (parameters.Length != 2) continue;
                if (parameters[0].ParameterType != typeof(object)) continue;
                if (parameters[1].ParameterType.IsAssignableFrom(typeof(PropertyChangingEventArgs)))
                {
                    var changingAttributes = method.GetCustomAttributes(typeof(PropertyChangingHandlerAttribute), false).OfType<PropertyChangingHandlerAttribute>().ToList();
                    if (changingAttributes.Count > 0)
                    {
                        var handlerDelegate = (PropertyChangingEventHandler)Delegate.CreateDelegate(typeof(PropertyChangingEventHandler), handler, method);
                        if (handlerDelegate == null)
                        {
                            throw new ArgumentException("Contract assertion not met: handlerDelegate != null", "value");
                        }
                        foreach (var attribute in changingAttributes)
                        {
                            RegisterNotificationHandler(attribute.PropertyName, handlerDelegate);
                        }
                    }
                }
                if (parameters[1].ParameterType.IsAssignableFrom(typeof(PropertyChangedEventArgs)))
                {
                    var changedAttributes = method.GetCustomAttributes(typeof(PropertyChangedHandlerAttribute), false).OfType<PropertyChangedHandlerAttribute>().ToList();
                    if (changedAttributes.Count > 0)
                    {
                        var handlerDelegate = (PropertyChangedEventHandler)Delegate.CreateDelegate(typeof(PropertyChangedEventHandler), handler, method);
                        if (handlerDelegate == null)
                        {
                            throw new ArgumentException("Contract assertion not met: handlerDelegate != null", "value");
                        }
                        foreach (var attribute in changedAttributes)
                        {
                            RegisterNotificationHandler(attribute.PropertyName, handlerDelegate);
                        }
                    }
                }
            }
        }

        public void RegisterNotificationHandler(string propertyName, PropertyChangingEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "Contract assertion not met: handler != null");
            }

            if (!_changingHandlers.ContainsKey(propertyName))
            {
                _changingHandlers[propertyName] = handler;
            }
            else
            {
                _changingHandlers[propertyName] = _changingHandlers[propertyName] += handler;
            }
        }

        public void UnregisterNotificationHandler(string propertyName, PropertyChangingEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "Contract assertion not met: handler != null");
            }

            if (_changingHandlers.ContainsKey(propertyName))
            {
                _changingHandlers[propertyName] = _changingHandlers[propertyName] -= handler;
            }
        }

        public void RegisterNotificationHandler(string propertyName, PropertyChangedEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "Contract assertion not met: handler != null");
            }

            if (!_changedHandlers.ContainsKey(propertyName))
            {
                _changedHandlers[propertyName] = handler;
            }
            else
            {
                _changedHandlers[propertyName] = _changedHandlers[propertyName] += handler;
            }
        }

        public void UnregisterNotificationHandler(string propertyName, PropertyChangedEventHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "Contract assertion not met: handler != null");
            }

            if (_changedHandlers.ContainsKey(propertyName))
            {
                _changedHandlers[propertyName] = _changedHandlers[propertyName] -= handler;
            }
        }

        public void UnregisterAllNotificationHandlers(string propertyName)
        {
            if (_changingHandlers.ContainsKey(propertyName))
            {
                _changingHandlers.Remove(propertyName);
            }
            if (_changedHandlers.ContainsKey(propertyName))
            {
                _changedHandlers.Remove(propertyName);
            }
        }

        public void UnregisterAllNotificationHandlers()
        {
            _changingHandlers.Clear();
            _changedHandlers.Clear();
        }

        public void ProcessNotification(object sender, PropertyChangingEventArgs e)
        {
            if (_changingHandlers.ContainsKey(e.PropertyName))
            {
                var handler = _changingHandlers[e.PropertyName];
                if (handler != null) handler(sender, e);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("No {0} property changing handler registered.", e.PropertyName));
            }
        }

        public void ProcessNotification(object sender, PropertyChangedEventArgs e)
        {
            if (_changedHandlers.ContainsKey(e.PropertyName))
            {
                var handler = _changedHandlers[e.PropertyName];
                if (handler != null) handler(sender, e);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("No {0} property changed handler registered.", e.PropertyName));
            }
        }

        #endregion
    }
}
