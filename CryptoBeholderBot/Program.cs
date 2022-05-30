using CryptoBeholder.DAL;
using CryptoBeholder.Lib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace CryptoBeholderBot {
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddDbContext<UserContext>(ServiceLifetime.Transient);
                services.AddTransient<IDatabaseReader, DatabaseReader>();
                services.AddSingleton<Bot>();
                services.AddSingleton<ITracer, Tracer>();
            }).Build();

            
            var bot = host.Services.GetService<Bot>();
            bot.MainAsync().GetAwaiter().GetResult();
            
            //var bot = ActivatorUtilities.CreateInstance<Bot>(host.Services);
            //bot.MainAsync().GetAwaiter();

        }
    }
}
