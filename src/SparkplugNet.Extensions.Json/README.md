# SparkplugNet.Extensions.Json

Add `SparkplugNet.Extensions.Json` to a .NET 10 or .NET 11 project to read and write Sparkplug B metrics as JSON. Register the converter for the serializer you use.

## System.Text.Json

```csharp
using System.Text.Json;
using SparkplugNet.Extensions.Json;
using SparkplugNet.VersionB.Data;

var options = new JsonSerializerOptions();
options.Converters.Add(new SparkplugJsonConverter());
var metrics = JsonSerializer.Deserialize<List<Metric>>(json, options);
```

## Newtonsoft.Json

```csharp
using Newtonsoft.Json;
using SparkplugNet.Extensions.Json;
using SparkplugNet.VersionB.Data;

var settings = new JsonSerializerSettings();
settings.Converters.Add(new SparkplugNewtonsoftJsonConverter());
var metrics = JsonConvert.DeserializeObject<List<Metric>>(json, settings);
```

Use the same options or settings when serializing. The converter uses `DataType` to reconstruct `Value`, including metrics inside templates, parameters, data set cells, and property values. This JSON format is for application data. Sparkplug MQTT messages use Protobuf.

To build the package locally, run `dotnet build src/SparkplugNet.Extensions.Json -c Release` from the repository root. The package is written to the extension project's `bin/Release` directory.
