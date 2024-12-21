# Sardine
<div align="center">
  
<img src="logo.png" width="400">

| **Core** | **WPF Addons** |
| -------- | -------------- |
| [![NuGet Core](https://img.shields.io/nuget/v/Sardine.Core.svg)](https://www.nuget.org/packages/Sardine.Core/) | [![NuGet WPF](https://img.shields.io/nuget/v/Sardine.Core.Views.WPF.svg)](https://www.nuget.org/packages/Sardine.Core.Views.WPF/) |

[![Version](https://img.shields.io/badge/Change%20Log-green)](CHANGELOG.md)
</div>

Sardine is an open-source framework for developing and executing complex scientific workflows written in C# and built with .NET 8.
It is designed to rapidly create and execute data acquisition and processing desktop applications following a modular approach. 

Sardine allows the reliable execution of dynamic networks of independent modules - where each module can interface with hardware, process data, or both.
It includes integrated logging and UI management, has a low overhead, provides fault-tolerant hardware control, and minimizes downstream delays by
providing dedicated data processing queues for each module. Any .NET class can be easily adapted to a Sardine module, facilitating integration with existing codebases.

**Get Sardine through [NuGet](https://www.nuget.org/packages/Sardine.Core/)!** The WPF addons to build desktop applications are available in the [WPF addons](https://www.nuget.org/packages/Sardine.Core.Views.WPF/) package.

[Our ready-to-use components are available here.](https://github.com/orger-lab/sardine-components)

---

A change log is available [here](CHANGELOG.md).

Submit crash reports/bugs/feature requests through [GitHub Issues](https://github.com/orger-lab/sardine/issues).

## Quick Start


### Building a Fleet and Freighting Vessels
```
using Sardine.Core;
namespace ExampleApplication;

// Create a new Fleet
public class MySystem : Fleet
{

    // Declare Vessel properties that will serve as containers for the components
    public Vessel<CameraService> CameraProvider { get; }
    public Vessel<Camera> ImagingCamera { get; }
    public Vessel<DataSaver> FrameSaver { get; }

    public MySystem()
    {

        // Freight the Vessels by providing both dependencies
        // and build, initializer, and invalidator methods
        CameraProvider = Freighter.Freight<CameraService>(
				builder: () => new CameraService()
			     );

        ImagingCamera = Freighter.Freight<Camera>(
				CameraProvider,
				builder: (provider) => ...,
				initializer: (provider, camera) => ...,
				invalidator: (provider, camera) => ... );

        FrameSaver = Freighter.Freight<DataSaver>(() => ...);
    }
}

```
### Adding a data operation
```
// Write methods matching one of the data operation signatures
public delegate TOut? Source<out TOut>(THandle handle, out bool hasMore);
public delegate TOut? Transformer<in TIn, out TOut>(THandle handle, TIn data, MessageMetadata metadata);
public delegate void Sink<in TIn>(THandle handle, TIn data, MessageMetadata metadata);

// Add data operation to vessels in the Fleet constructor
public MySystem()
{
...
ImagingCamera.AddSource(CameraFrameSource);
FrameSaver.AddSink(FrameSink);
ImagingCamera.SourceRate = 100;
...
}
```

### Creating an application
```
--- XAML
<src:SardineApplication            
xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
x:TypeArguments="ExampleApplication:MySystem"
x:Class="ExampleApplication.App"
/>

--- Code behind
using Sardine.Core.Views.WPF;

namespace ExampleApplication;

// Have the entrypoint derive from SardineApplication
public partial class App : SardineApplication<MySystem> { }
```

## License
Sardine is licensed under the [MIT License](LICENSE.md). If you are using software built with Sardine for your research, please cite our publication below.

## Citation
TODO: this

## Contact
- **Point of contact**: Lucas Martins
- **Email**: lucas.martins at neuro.fchampalimaud.org
- **Organization**: Champalimaud Foundation

## Acknowledgments
This work was developed at the Champalimaud Foundation in the Vision to Action laboratory, led by Dr. Michael Orger.

We thank those who used software built with Sardine over the years, for helping to test the framework and providing critical feedback.

Lucas Martins was supported by the PhD fellowships from the Portuguese Fundação para a Ciência e Tecnologia SFRH/BD/129843/2017 and COVID/BD/152726/2022.
