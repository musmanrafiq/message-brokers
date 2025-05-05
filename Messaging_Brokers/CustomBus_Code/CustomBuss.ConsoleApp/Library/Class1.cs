using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CustomBuss.ConsoleApp.Library
{
    public interface ICommand { }
    public interface ICommand<TResult> { }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }

    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand command);
    }

    // Marker interface for queries
    public interface IQuery<TResult> { }

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }

    // Event handler (optional pub-sub)
    public interface IEventHandler<TEvent>
    {
        Task HandleAsync(TEvent @event);
    }
    public interface IMessageBus
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
        Task<TResult> SendAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>;

        //Task PublishAsync<TEvent>(TEvent @event); // Optional for events
        Task PublishAsync<TNotification>(TNotification notification) where TNotification : INotification;
    }

    public interface INotification { }

    public interface INotificationHandler<TNotification> where TNotification : INotification
    {
        Task HandleAsync(TNotification notification);
    }

    public class InMemoryMessageBus : IMessageBus
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryMessageBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            await handler.HandleAsync(command);
        }
        public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
        {
            // Get the handler for the command
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResult>>();

            if (handler == null)
            {
                throw new InvalidOperationException($"Handler for {typeof(TCommand).Name} not found.");
            }

            // Execute the handler and return the result
            return await handler.HandleAsync(command);
        }
        public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return await handler.HandleAsync(query);
        }

        /* public async Task PublishAsync<TEvent>(TEvent @event)
         {
             var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
             foreach (var handler in handlers)
             {
                 await handler.HandleAsync(@event);
             }
         }*/
        public async Task PublishAsync<TNotification>(TNotification notification) where TNotification : INotification
        {
            var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(notification);
            }
        }
    }

    public static class MessageBusServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageBus(this IServiceCollection services, params Assembly[] assemblies)
        {
            // Register the IMessageBus and any required infrastructure
            services.AddSingleton<IMessageBus, InMemoryMessageBus>();

            if (assemblies == null || assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                var interfaces = type.GetInterfaces().Where(i =>
                    i.IsGenericType && (
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                    )
                );

                foreach (var handlerInterface in interfaces)
                {
                    services.AddScoped(handlerInterface, type);
                }
            }

            return services;
        }
    }
}
