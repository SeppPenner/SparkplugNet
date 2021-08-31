## Basic usage

### Version A Sparkplug messages
Using applications:
```csharp
private static readonly List<KuraMetric> VersionAMetrics = new ()
{
	new ()
	{
		Name = "Test", Type = DataType.Double, DoubleValue = 1.20
	},
	new ()
	{
		Name = "Test2", Type = DataType.Bool, BoolValue = true
	}
};

Log.Information("Starting application...");
var applicationMetrics = new List<KuraMetric>(VersionAMetrics);
var application = new VersionA.SparkplugApplication(applicationMetrics, Log.Logger);
var applicationOptions = new SparkplugApplicationOptions("localhost", 1883, "application1", "user", "password", false, "scada1", TimeSpan.FromSeconds(30), true, null, null, CancellationTokenSource.Token);
await application.Start(applicationOptions);
Log.Information("Application started...");
```

Using nodes:
```csharp
private static readonly List<KuraMetric> VersionAMetrics = new ()
{
	new ()
	{
		Name = "Test", Type = DataType.Double, DoubleValue = 1.20
	},
	new ()
	{
		Name = "Test2", Type = DataType.Bool, BoolValue = true
	}
};

Log.Information("Starting node...");
var nodeMetrics = new List<KuraMetric>(VersionAMetrics);
var node = new VersionA.SparkplugNode(nodeMetrics, Log.Logger);
var nodeOptions = new SparkplugNodeOptions("localhost", 1883, "node 1", "user", "password", false, "scada1", "group1", "node1", TimeSpan.FromSeconds(30), null, null, CancellationTokenSource.Token);
await node.Start(nodeOptions);
Log.Information("Node started...");
```

Using devices with a node:
```csharp
private static readonly List<KuraMetric> VersionAMetrics = new ()
{
	new ()
	{
		Name = "Test", Type = DataType.Double, DoubleValue = 1.20
	},
	new ()
	{
		Name = "Test2", Type = DataType.Bool, BoolValue = true
	}
};

Log.Information("Starting device...");
const string DeviceIdentifier = "device1";
var deviceMetrics = new List<KuraMetric>(VersionAMetrics);
await node.PublishDeviceBirthMessage(deviceMetrics, DeviceIdentifier);
await node.PublishDeviceData(deviceMetrics, DeviceIdentifier);
await node.PublishDeviceDeathMessage(DeviceIdentifier);
Log.Information("Device started...");
```

### Version B Sparkplug messages
Using applications:
```csharp
private static readonly List<Metric> VersionBMetrics = new ()
{
	new Metric
	{
		Name = "Test", DataType = (uint)DataType.Double, DoubleValue = 1.20
	},
	new Metric
	{
		Name = "Test2",
		DataType = (uint)DataType.Boolean, BooleanValue = true
	}
};

Log.Information("Starting application...");
var applicationMetrics = new List<Metric>(VersionBMetrics);
var application = new VersionA.SparkplugApplication(applicationMetrics, Log.Logger);
var applicationOptions = new SparkplugApplicationOptions("localhost", 1883, "application1", "user", "password", false, "scada1", TimeSpan.FromSeconds(30), true, null, null, CancellationTokenSource.Token);
await application.Start(applicationOptions);
Log.Information("Application started...");
```

Using nodes:
```csharp
private static readonly List<Metric> VersionBMetrics = new ()
{
	new Metric
	{
		Name = "Test", DataType = (uint)DataType.Double, DoubleValue = 1.20
	},
	new Metric
	{
		Name = "Test2",
		DataType = (uint)DataType.Boolean, BooleanValue = true
	}
};

Log.Information("Starting node...");
var nodeMetrics = new List<Metric>(VersionBMetrics);
var node = new VersionA.SparkplugNode(nodeMetrics, Log.Logger);
var nodeOptions = new SparkplugNodeOptions("localhost", 1883, "node 1", "user", "password", false, "scada1", "group1", "node1", TimeSpan.FromSeconds(30), null, null, CancellationTokenSource.Token);
await node.Start(nodeOptions);
Log.Information("Node started...");
```

Using devices with a node:
```csharp
private static readonly List<Metric> VersionBMetrics = new ()
{
	new Metric
	{
		Name = "Test", DataType = (uint)DataType.Double, DoubleValue = 1.20
	},
	new Metric
	{
		Name = "Test2",
		DataType = (uint)DataType.Boolean, BooleanValue = true
	}
};

Log.Information("Starting device...");
const string DeviceIdentifier = "device1";
var deviceMetrics = new List<Metric>(VersionBMetrics);
await node.PublishDeviceBirthMessage(deviceMetrics, DeviceIdentifier);
await node.PublishDeviceData(deviceMetrics, DeviceIdentifier);
await node.PublishDeviceDeathMessage(DeviceIdentifier);
Log.Information("Device started...");
```