﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Executor.Models.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Executor
{
    class Program
    {
        private const string SettingsFileName = "appsettings.Secret.json";
        private static IConfiguration configuration;


        static async Task Main(string[] args)
        {
            configuration = SetupConfigs(args);

            var servicesProvider = BuildServices();

            if (!await IsDockerAvailable(servicesProvider.GetRequiredService<IDockerClient>()))
            {
                Console.WriteLine("host must see docker!");
                return;
            }

            var executor = servicesProvider.GetRequiredService<Executor>();
            await executor.Start(CancellationToken.None);
            Console.ReadLine();
        }

        private static async Task<bool> IsDockerAvailable(IDockerClient dockerClient)
        {
            try
            {
                await dockerClient.System.PingAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static IServiceProvider BuildServices()
            => new ServiceCollection()
                .Configure<StartSettings>(configuration.GetSection(nameof(StartSettings)))
                .Configure<UserInfo>(configuration.GetSection(nameof(UserInfo)))
                .AddTransient<DbManager>()
                .AddTransient<Executor>()
                .AddHttpClient(DbManager.DbManagerHttpClientName, (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<StartSettings>>();
                    client.BaseAddress = new Uri(options.Value.Address);
                })
                .Services
                .AddSingleton<IDockerClient>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<StartSettings>>();
                    return new DockerClientConfiguration(new Uri(options.Value.DockerEndPoint)).CreateClient();
                })
                .BuildServiceProvider();

        private static IConfiguration SetupConfigs(string[] args)
            => new ConfigurationBuilder()
                .AddJsonFile(SettingsFileName)
                .Build();
    }
}
