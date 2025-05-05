using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorR_Code.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            // NEW MediatR registration (v12+)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetUserByIdQuery>());

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<IMediator>();

            // Create and send command
            var query = new GetUserByIdQuery(1);

            var result = await mediator.Send(query);

            Console.WriteLine(result.Name);
        }
    }


    public class GetUserByIdQuery : IRequest<User>
    {
        public int Id { get; set; }


        public GetUserByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, User>
    {
        public Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            // Simulate user creation logic
            var newUserId = Guid.NewGuid();

            return Task.FromResult(new User
            {
                Id = 1,
                Name = "User is Usman!"
            });
        }
    }
}


