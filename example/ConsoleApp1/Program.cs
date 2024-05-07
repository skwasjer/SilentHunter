using Microsoft.Extensions.DependencyInjection;
using Particles;
using SilentHunter.FileFormats.Dat;
using SilentHunter.FileFormats.DependencyInjection;

IServiceCollection svcCollection = new ServiceCollection();
svcCollection.AddSilentHunterParsers(configurer => configurer.Controllers.FromAssembly(typeof(ParticleGenerator).Assembly));
ServiceProvider svcProvider = svcCollection.BuildServiceProvider();
DatFile datFile = svcProvider.GetRequiredService<DatFile>();

const string fPath = "../../../AI_Sensors.dat";

await datFile.LoadAsync(fPath);

Console.WriteLine(datFile.Chunks[100].Id);
