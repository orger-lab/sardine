
# Sardine
Sardine is an open-source framework for developing and executing complex scientific workflows written in C# and built with .NET 8.
It is designed to rapidly create and execute data acquisition and processing desktop applications following a modular approach. 

Sardine allows the reliable execution of dynamic networks of independent modules - where each module can interface with hardware, process data, or both.
It includes integrated logging and UI management, has a low overhead, provides fault-tolerant hardware control, and minimizes downstream delays by
providing dedicated data processing queues for each module. Any .NET class can be easily adapted to a Sardine module, facilitating integration with existing codebases.

[Our ready-to-use components are available here.](https://github.com/orger-lab/sardine-components)

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
