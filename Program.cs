using System;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.Definition;
using MassTransitTest;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

const string localstackHost = "localhost";
const int localstackPort = 4566;
string serviceUrl = $"http://{localstackHost}:{localstackPort}";

AmazonSQSConfig amazonSqsConfig = new() { ServiceURL = serviceUrl };
AmazonSimpleNotificationServiceConfig amazonSnsConfig = new() { ServiceURL = serviceUrl };

await Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(x =>
        {
            x.UsingAmazonSqs((_, cfg) =>
            {
                cfg.Host(
                    new Uri($"amazonsqs://{localstackHost}:{localstackPort}"),
                    h =>
                    {
                        h.Config(amazonSqsConfig);
                        h.Config(amazonSnsConfig);
                    });

                cfg.ReceiveEndpoint(
                    new TemporaryEndpointDefinition(),
                    new DefaultEndpointNameFormatter(false),
                    e => { e.Consumer<TestConsumer>(); });
            });
        });
        services.AddMassTransitHostedService();
    })
    .UseSerilog((_, log) =>
    {
        log.MinimumLevel.Override("MassTransit", LogEventLevel.Debug);
        log.WriteTo.Console();
    })
    .Build()
    .RunAsync();