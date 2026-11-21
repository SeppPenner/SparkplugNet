# SparkplugNet for .NET nanoFramework

This project supports Sparkplug B Edge Nodes on stable .NET nanoFramework 1.x. It runs over MQTT 3.1.1 on IPv4 TCP and includes node and attached device lifecycles, command callbacks, metric aliases, rebirth, and the Sparkplug 2.2 and 3.0 Primary Host STATE formats.

The codec supports scalar metrics, byte and file values, and explicit nulls. Box values using the matching C# types, such as `short` for `Int16`, `long` for `Int64`, and Unix milliseconds as `ulong` for `DateTime`. Topics, groups, and edge and device IDs use Sparkplug B. This implementation is scoped to the Edge Node role and plain TCP. Templates, datasets, arrays, metadata, property sets, and TLS are outside its current scope.

In Sparkplug 3.0, data messages omit metric data types and fill missing metric timestamps with the payload timestamp. Pass the source's birth metrics to `SparkplugBCodec.Decode` when decoding a payload with omitted types. The Edge Node resolves incoming commands against the metrics of the addressed node or device.

## Build

Use the [nanoFramework Visual Studio extension](https://docs.nanoframework.net/content/getting-started-guides/getting-started-managed.html) and run these commands from the repository root in a Visual Studio Developer PowerShell with NuGet CLI available.

```powershell
nuget restore src/SparkplugNet.nanoFramework/packages.config -PackagesDirectory packages
msbuild src/SparkplugNet.nanoFramework/SparkplugNet.nanoFramework.nfproj /p:Configuration=Release
```

Add the `.nfproj` as a project reference in a nanoFramework application and deploy it to a device with networking enabled. Configure the network and UTC clock before connecting. Use a broker address reachable from the device.

## Publish metrics

```csharp
using SparkplugNet.NanoFramework;

var options = new MqttConnectionOptions("192.168.1.50", "plant-1-sensor-1");
using (var node = new SparkplugNode(options, "Plant 1", "Sensor 1"))
{
    node.Connect(new[] { new Metric("temperature", MetricDataType.Float, 20.5f) { Alias = 1 } });
    node.PublishNodeData(new[] { new Metric("temperature", MetricDataType.Float, 21.0f) });

    while (node.IsConnected)
    {
        node.Poll(1000);
    }
}
```

Call `Poll` regularly on the same thread as the other node methods. It processes commands and keeps the MQTT session alive. When you specify a Primary Host ID, wait for `IsOnline` before publishing data. If the Primary Host goes offline, the client closes the session. The application can call `Connect` again to start a new session. Save `NextBirthDeathSequence` and pass it as `initialBdSeq` to preserve the birth/death sequence across restarts.

Run the desktop test project to check codec interoperability with the standard SparkplugNet protobuf implementation. The [HiveMQ CE and Eclipse Sparkplug TCK setup](../../testing/sparkplug-tck/README.md) provides a broker for exercising the Edge Node.
