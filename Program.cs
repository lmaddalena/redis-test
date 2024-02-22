// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RedisTest;
internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");


        // create and configure the service container
        IServiceCollection serviceCollection = ConfigureServices();

        // build the service provider
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        
        RedisDemo demo = serviceProvider.GetService<RedisDemo>();

        string key = "key-001";

        await demo.AddToCache<string>(key, "alfa");     

        string o = await demo.ReadFromCache<string>(key);

        Console.WriteLine(o);

        Member m = new Member(){ Username = "john@tempuri.com", Name = "john" };

        await demo.AddToCache<Member>(m.GetKey(), m);

        Member mm = await demo.ReadFromCache<Member>(m.GetKey());

        Console.WriteLine(mm.Username + ":\t" + mm.Name);


    }

    private static IServiceCollection ConfigureServices()
    {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                ;     

            IConfigurationRoot configuration = builder.Build();

            IServiceCollection service = new ServiceCollection();


            service.AddLogging(configure => 
                {
                    configure.AddConsole();
                    configure.AddConfiguration(configuration.GetSection("Logging"));
                }).Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug)

                .AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetConnectionString("Redis");
                    options.InstanceName = "RedisDemo_";


                })
                
                .AddTransient<RedisDemo>();
            
            return service;
    }
}