# Silent Hunter 3/4/5 file parsers

This repository contains:
- Silent Hunter controller templates
- Dynamic code compiler (for the controllers)
- Silent Hunter game file parsers (with extensions .dat, .sim, .zon, .dsd, .cam, .val, .sdl, .off)

## Why?

Over a decade ago, I wrote a popular modding application called S3D (http://s3d.skwas.com) for Silent Hunter game files. Back then, I used some proprietary code to do so (from my own business). Fast forward to today - for posterity sake - I took some time to clean up the old code base and release it for everyone to use.

This repository only covers the parsing logic and controller templates, but time permitted I will try to release S3D source code too in the future.

> Important: this is not fully backward compatible with S3D's controller templates that were released in 2009. This is due some refactoring done to make it more usuable in open source context. If you have updated/added/improved controllers, please submit a PR.

## Usage

There are two ways to configure the parsers:

- Using a dynamically compiled controller assembly
- Using a precompiled controller assembly

S3D specifically uses the dynamic controller assembly. This enabled modders to modify the controller templates, after which S3D would be able to use the new templates.

Which method you choose depends on how you wish to distribute your application. If you do not care much about this, simply use the precompiled package.

## Dependency injection

The file parsers are developed with dependency injection for `IServiceCollection` in mind, allowing you to inject the parsers in your own code base. As such, you need to configure the container before being able to use the parsers.

### Using a dynamically compiled controller assembly

```powershell
Install-Package SilentHunter.Controllers.Templates
Install-Package SilentHunter.Controllers.Compiler
Install-Package SilentHunter.FileFormats
```

By adding the package `SilentHunter.Controllers.Templates`, the template source files are added to your application build output directory.

```csharp
IServiceCollection services = ...

string controllerPath = "path to the controllers";

// Add services
services
    .AddSilentHunterParsers(configurer => configurer
        .Controllers.CompileFrom(
            controllerPath,
            assemblyName: "Controllers"
        )
    );
```

### Using a precompiled controller assembly

```powershell
Install-Package SilentHunter.Controllers
Install-Package SilentHunter.FileFormats
```

```csharp
IServiceCollection services = ...

// Add services
services
    .AddSilentHunterParsers(configurer => configurer
        .Controllers.FromAssembly(typeof(ParticleGenerator).Assembly)
    );
```

## Using the file parsers

### DatFile

```csharp
IServiceProvider serviceProvider = ...  // Or inject into your classes directly.

// Request (transient) instance of DatFile
DatFile datFile = serviceProvider.GetRequiredService<DatFile>();
using (var fsIn = File.OpenRead(...))
using (var fsOut = File.OpenWrite(...))
{
    await datFile.LoadAsync(fsIn);

    var newChunk = datFile.CreateChunk<LabelChunk>();
    newChunk.Text = "My new label";
    datFile.Chunks.Add(newChunk);

    await datFile.SaveAsync(fsOut);
}
```

> This example uses `IServiceProvider` directly. It is better to design classes and inject `DatFile`, `SdlFile` or `OffFile` instances in there to work with.

### SdlFile / OffFile

For SDL and OFF files, the principle is the same. Simply request a new instance from the service container or inject into a class:

```csharp
SdlFile sdlFile = serviceProvider.GetRequiredService<SdlFile>();
OffFile offFile = serviceProvider.GetRequiredService<OffFile>();
```

### Contributions
PR's are welcome. Please rebase before submitting, provide test coverage, and ensure the AppVeyor build passes. I will not consider PR's otherwise.

### Contributors
- skwas (author/maintainer)

### Useful info

- [Changelog](Changelog.md)
- [S3D website](http://s3d.skwas.com)
- [S3D release thread at SubSim.com](https://www.subsim.com/radioroom/showthread.php?t=119571)

#### With thanks to

Ubisoft, Silent Hunter development team, SubSim.com

Also individuals:  
CaptainCox, leovampire, the modders at Submarine Sim Central, tater, Anvart, Bando, WEBSTER, maikarant, Jace11, WilhelmTell, lurbz, Sansal, longam, PepsiCan, UBOAT234, Digital_Trucker, DirtyHarry3033, Kaleun_Endrass, sergbuto, SquareSteelBar, sober, jaketoox, reallydedpoet, swdw, nautilus42, nvdrifter, kapitan_zur_see, DeepIron, panthercules, l3th4l, Laffertytig, alamwuhk2a, privateer, g_BonE, Ducimus,  lurker_hlb3, haegemon, wildchild, urfisch, LukeFF, ref, kikn79, WernerSobe, Redwine, GuillermoZS, keltos01, Mikhayl, Kriller2, DrBeast, jimbob, Rubini, M. Sarsfield, miner1436, AVGWarhawk, reallydedpoet, Subject, and those who have been debugging the file formats long before me.