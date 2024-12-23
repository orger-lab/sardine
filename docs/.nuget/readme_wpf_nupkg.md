
# Sardine
Addons to Sardine that enable the generation of WPF desktop applications from Sardine Fleets.

## Quick Start

### Creating an application
```
--- XAML
<sardine:SardineApplication            
xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
xmlns:sardine="clr-namespace:Sardine.Core.Views.WPF;assembly=Sardine.Core.Views.WPF"
xmlns:fleet="clr-namespace:ExampleFleet;assembly=ExampleFleet"
x:TypeArguments="fleet:MySystem"
x:Class="ExampleApplication.App"
/>

--- Code behind
using Sardine.Core.Views.WPF;

namespace ExampleApplication;

// Have the entrypoint derive from SardineApplication
public partial class App : SardineApplication<MySystem> { }
```