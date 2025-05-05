using CustomBuss.ConsoleApp.Library;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CustomBuss.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            // Register message bus and handlers
            services.AddMessageBus(Assembly.GetExecutingAssembly());

            var provider = services.BuildServiceProvider();
            // Manually resolve IMessageBus
            var messageBus = provider.GetRequiredService<IMessageBus>();

            // Call the command
            await messageBus.PublishAsync(new ProductCreatedNotification
            {
                ProductId = Guid.NewGuid(),
            });
        }
    }

    public class ProductCreatedNotification : INotification
    {
        public Guid ProductId { get; set; }
    }

    public class SendAdminEmailHandler : INotificationHandler<ProductCreatedNotification>
    {
        public Task HandleAsync(ProductCreatedNotification notification)
        {
            Console.WriteLine(notification.ProductId);
            // Send email
            return Task.CompletedTask;
        }
    }

    public class LogProductCreationHandler : INotificationHandler<ProductCreatedNotification>
    {
        public Task HandleAsync(ProductCreatedNotification notification)
        {
            Console.WriteLine(notification.ProductId);
            // Log to file
            return Task.CompletedTask;
        }
    }
}

/*If each module has its own assembly, this is perfect for:

csharp
Copy
Edit
services.AddMessageBus(typeof(ProductModule).Assembly, typeof(OrderModule).Assembly);
*/