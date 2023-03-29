using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Workshop.CSharp.Advanced
{
    public interface IEventBroker
    {
        void Publish<T>(T message);
        void Subscribe<T>(IHandle<T> instance);
        void Unsubscribe<T>(IHandle<T> instance);
    }
    public interface IHandle<T>
    {
        void Handle(T message);
    }

    public class EventBroker : IEventBroker
    {
        private readonly Dictionary<Type, List<object>> _listeners = new Dictionary<Type, List<object>>();

        public void Publish<T>(T message)
        {
            List<object> list;
            if (_listeners.TryGetValue(typeof(T), out list))
            {
                foreach (dynamic o in list)
                    o.Handle(message);
            }
        }

        public void Subscribe<T>(IHandle<T> instance)
        {
            List<object> list;
            if (!_listeners.TryGetValue(typeof(T), out list))
            {
                list = new List<object>();
                _listeners.Add(typeof(T), list);
            }

            if (!list.Contains(instance))
                list.Add(instance);
        }

        public void Unsubscribe<T>(IHandle<T> instance)
        {
            List<object> list;
            if (_listeners.TryGetValue(typeof(T), out list))
            {
                list.Remove(instance);
            }
        }
    }

    public static class EventAggregatorExtensions
    {
        public static IDisposable Subscribe<T>(this IEventBroker eventBroker, Action<T> action)
        {
            var handler = new ActionHandler<T> { Action = action };
            eventBroker.Subscribe(handler);
            return new ActionDisposable() { Action = () => eventBroker.Unsubscribe(handler) };
        }

        public class ActionHandler<T> : IHandle<T>
        {
            public Action<T> Action { get; set; }

            public void Handle(T message)
            {
                Action(message);
            }
        }

        private class ActionDisposable : IDisposable
        {
            public Action Action { get; set; }

            public void Dispose()
            {
                Action();
            }
        }
    }
}

/*


namespace Caliburn.Micro {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///   A marker interface for classes that subscribe to messages.
    /// </summary>
    public interface IHandle {}

    /// <summary>
    ///   Denotes a class which can handle a particular type of message.
    /// </summary>
    /// <typeparam name = "TMessage">The type of message to handle.</typeparam>
    public interface IHandle<TMessage> : IHandle {
        /// <summary>
        ///   Handles the message.
        /// </summary>
        /// <param name = "message">The message.</param>
        void Handle(TMessage message);
    }

    /// <summary>
    ///   Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public interface IEventAggregator {
        /// <summary>
        ///   Gets or sets the default publication thread marshaller.
        /// </summary>
        /// <value>
        ///   The default publication thread marshaller.
        /// </value>
        Action<System.Action> PublicationThreadMarshaller { get; set; }

        /// <summary>
        ///   Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "instance">The instance to subscribe for event publication.</param>
        void Subscribe(object instance);

        /// <summary>
        ///   Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "instance">The instance to unsubscribe.</param>
        void Unsubscribe(object instance);

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <remarks>
        ///   Uses the default thread marshaller during publication.
        /// </remarks>
        void Publish(object message);

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name = "marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        void Publish(object message, Action<System.Action> marshal);
    }

    /// <summary>
    ///   Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public class EventAggregator : IEventAggregator {
        readonly List<Handler> handlers = new List<Handler>();

        /// <summary>
        ///   The default thread marshaller used for publication;
        /// </summary>
        public static Action<System.Action> DefaultPublicationThreadMarshaller = action => action();

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EventAggregator" /> class.
        /// </summary>
        public EventAggregator() {
            PublicationThreadMarshaller = DefaultPublicationThreadMarshaller;
        }

        /// <summary>
        ///   Gets or sets the default publication thread marshaller.
        /// </summary>
        /// <value>
        ///   The default publication thread marshaller.
        /// </value>
        public Action<System.Action> PublicationThreadMarshaller { get; set; }

        /// <summary>
        ///   Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "instance">The instance to subscribe for event publication.</param>
        public virtual void Subscribe(object instance) {
            lock(handlers) {
                if (handlers.Any(x => x.Matches(instance))) {
                    return;
                }

                handlers.Add(new Handler(instance));
            }
        }

        /// <summary>
        ///   Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "instance">The instance to unsubscribe.</param>
        public virtual void Unsubscribe(object instance) {
            lock(handlers) {
                var found = handlers.FirstOrDefault(x => x.Matches(instance));

                if (found != null) {
                    handlers.Remove(found);
                }
            }
        }

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <remarks>
        ///   Does not marshall the the publication to any special thread by default.
        /// </remarks>
        public virtual void Publish(object message) {
            Publish(message, PublicationThreadMarshaller);
        }

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name = "marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        public virtual void Publish(object message, Action<System.Action> marshal) {
            Handler[] toNotify;
            lock (handlers) {
                toNotify = handlers.ToArray();
            }

            marshal(() => {
                var messageType = message.GetType();

                var dead = toNotify
                    .Where(handler => !handler.Handle(messageType, message))
                    .ToList();

                if(dead.Any()) {
                    lock(handlers) {
                        dead.Apply(x => handlers.Remove(x));
                    }
                }
            });
        }

#if WinRT
        protected class Handler         {
            readonly WeakReference reference;
            readonly Dictionary<TypeInfo, MethodInfo> supportedHandlers = new Dictionary<TypeInfo, MethodInfo>();

            public Handler(object handler) {
                reference = new WeakReference(handler);

                var handlerInfo = typeof(IHandle).GetTypeInfo();
                var interfaces = handler.GetType().GetTypeInfo().ImplementedInterfaces
                    .Where(x => handlerInfo.IsAssignableFrom(x.GetTypeInfo()) && x.IsGenericType);

                foreach (var @interface in interfaces) {
                    var type = @interface.GenericTypeArguments[0];
                    var method = @interface.GetTypeInfo().DeclaredMethods.First(x => x.Name == "Handle");
                    supportedHandlers[type.GetTypeInfo()] = method;
                }
            }

            public bool Matches(object instance) {
                return reference.Target == instance;
            }

            public bool Handle(Type messageType, object message) {
                var target = reference.Target;
                if (target == null)
                    return false;

                var typeInfo = messageType.GetTypeInfo();

                foreach (var pair in supportedHandlers) {
                    if (pair.Key.IsAssignableFrom(typeInfo)) {
                        pair.Value.Invoke(target, new[] { message });
                        return true;
                    }
                }

                return true;
            }
        }
#else
        protected class Handler {
            readonly WeakReference reference;
            readonly Dictionary<Type, MethodInfo> supportedHandlers = new Dictionary<Type, MethodInfo>();

            public Handler(object handler) {
                reference = new WeakReference(handler);

                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType);

                foreach(var @interface in interfaces) {
                    var type = @interface.GetGenericArguments()[0];
                    var method = @interface.GetMethod("Handle");
                    supportedHandlers[type] = method;
                }
            }

            public bool Matches(object instance) {
                return reference.Target == instance;
            }

            public bool Handle(Type messageType, object message) {
                var target = reference.Target;
                if (target == null) {
                    return false;
                }

                foreach(var pair in supportedHandlers) {
                    if(pair.Key.IsAssignableFrom(messageType)) {
                        pair.Value.Invoke(target, new[] { message });
                        return true;
                    }
                }

                return true;
            }
        }
#endif
    }
}
*/