# Basic usage

## Version A Sparkplug messages
Using applications:
```csharp
private readonly List<KuraMetric> VersionAMetrics = new ()
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

var applicationMetrics = new List<KuraMetric>(VersionAMetrics);
var application = new VersionA.SparkplugApplication(applicationMetrics, Log.Logger);
var applicationOptions = new SparkplugApplicationOptions("localhost", 1883, "application1", "user", "password", false, "scada1", TimeSpan.FromSeconds(30), true, null, null, new CancellationToken());

// Start an application.
await application.Start(applicationOptions);

// Publish node commands.
await application.PublishNodeCommand(applicationMetrics, "group1", "edge1");

// Publish device commands.
await application.PublishDeviceCommand(applicationMetrics, "group1", "edge1", "device1");

// Get the known metrics from an application.
var currentlyKnownMetrics = application.KnownMetrics;

// Get the device states from an application.
var currentDeviceStates = application.DeviceStates;

// Get the node states from an application.
var currentNodeStates = application.NodeStates;

// Check whether an application is connected.
var isApplicationConnected = application.IsConnected;

// Handle the application's disconnected event.
application.OnDisconnected += OnApplicationDisconnected;

private void OnApplicationDisconnected()
{
	// Do something.
}

// Stopping an application.
await application.Stop();
```

Using nodes:
```csharp
private readonly List<KuraMetric> VersionAMetrics = new ()
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

var nodeMetrics = new List<KuraMetric>(VersionAMetrics);
var node = new VersionA.SparkplugNode(nodeMetrics, Log.Logger);
var nodeOptions = new SparkplugNodeOptions("localhost", 1883, "node 1", "user", "password", false, "scada1", "group1", "node1", TimeSpan.FromSeconds(30), null, null, new CancellationToken());

// Start a node.
await node.Start(nodeOptions);

// Publish node metrics.
await node.PublishMetrics(nodeMetrics);

// Get the known node metrics from a node.
var currentlyKnownMetrics = node.KnownMetrics;

// Check whether a node is connected.
var isApplicationConnected = node.IsConnected;

// Handle the node's disconnected event.
node.OnDisconnected += OnVersionANodeDisconnected;

private void OnVersionANodeDisconnected()
{
	// Do something.
}

// Handle the node's node command received event.
node.NodeCommandReceived += OnVersionANodeNodeCommandReceived;

private void OnVersionANodeNodeCommandReceived(Metric metric)
{
	// Do something.
}

// Handles the node's status message received event.
node.StatusMessageReceived += OnVersionANodeStatusMessageReceived;

private void OnVersionANodeStatusMessageReceived(string status)
{
	// Do something.
}

// Get the known devices.
var knownDevices = node.KnownDevices;

// Handling devices.
const string DeviceIdentifier = "device1";
var deviceMetrics = new List<KuraMetric>(VersionAMetrics);

// Publish a device birth message.
await node.PublishDeviceBirthMessage(deviceMetrics, DeviceIdentifier);

// Publish a device data message.
await node.PublishDeviceData(deviceMetrics, DeviceIdentifier);

// Publish a device death message.
await node.PublishDeviceDeathMessage(DeviceIdentifier);

// Handle the node's device birth received event.
node.DeviceBirthReceived += OnVersionANodeDeviceBirthReceived;

private void OnVersionANodeDeviceBirthReceived(KeyValuePair<string, List<KuraMetric>> deviceData)
{
	// Do something.
}

// Handle the node's device command received event.
node.DeviceCommandReceived += OnVersionANodeDeviceCommandReceived;

private void OnVersionANodeDeviceCommandReceived(KuraMetric metric)
{
	// Do something.
}

// Handle the node's device death received event.
node.DeviceDeathReceived += OnVersionANodeDeviceDeathReceived;

private void OnVersionANodeDeviceDeathReceived(string deviceIdentifier)
{
	// Do something.
}

// Stopping a node.
await node.Stop();
```

## Version B Sparkplug messages
Using applications:
```csharp
private readonly List<Metric> VersionBMetrics = new ()
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

var applicationMetrics = new List<Metric>(VersionBMetrics);
var application = new VersionB.SparkplugApplication(applicationMetrics, Log.Logger);
var applicationOptions = new SparkplugApplicationOptions("localhost", 1883, "application1", "user", "password", false, "scada1", TimeSpan.FromSeconds(30), true, null, null, new CancellationToken());

// Start an application.
await application.Start(applicationOptions);

// Publish node commands.
await application.PublishNodeCommand(applicationMetrics, "group1", "edge1");

// Publish device commands.
await application.PublishDeviceCommand(applicationMetrics, "group1", "edge1", "device1");

// Get the known metrics from an application.
var currentlyKnownMetrics = application.KnownMetrics;

// Get the device states from an application.
var currentDeviceStates = application.DeviceStates;

// Get the node states from an application.
var currentNodeStates = application.NodeStates;

// Check whether an application is connected.
var isApplicationConnected = application.IsConnected;

// Handle the application's disconnected event.
application.OnDisconnected += OnApplicationDisconnected;

private void OnApplicationDisconnected()
{
	// Do something.
}

// Stopping an application.
await application.Stop();
```

Using nodes:
```csharp
private readonly List<Metric> VersionBMetrics = new ()
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

var nodeMetrics = new List<Metric>(VersionBMetrics);
var node = new VersionB.SparkplugNode(nodeMetrics, Log.Logger);
var nodeOptions = new SparkplugNodeOptions("localhost", 1883, "node 1", "user", "password", false, "scada1", "group1", "node1", TimeSpan.FromSeconds(30), null, null, new CancellationToken());

// Start a node.
await node.Start(nodeOptions);

// Publish node metrics.
await node.PublishMetrics(nodeMetrics);

// Get the known node metrics from a node.
var currentlyKnownMetrics = node.KnownMetrics;

// Check whether a node is connected.
var isApplicationConnected = node.IsConnected;

// Handle the node's disconnected event.
node.OnDisconnected += OnVersionBNodeDisconnected;

private void OnVersionBNodeDisconnected()
{
	// Do something.
}

// Handle the node's node command received event.
node.NodeCommandReceived += OnVersionBNodeNodeCommandReceived;

private void OnVersionBNodeNodeCommandReceived(Metric metric)
{
	// Do something.
}

// Handles the node's status message received event.
node.StatusMessageReceived += OnVersionBNodeStatusMessageReceived;

private void OnVersionBNodeStatusMessageReceived(string status)
{
	// Do something.
}

// Get the known devices.
var knownDevices = node.KnownDevices;

// Handling devices.
const string DeviceIdentifier = "device1";
var deviceMetrics = new List<Metric>(VersionBMetrics);

// Publish a device birth message.
await node.PublishDeviceBirthMessage(deviceMetrics, DeviceIdentifier);

// Publish a device data message.
await node.PublishDeviceData(deviceMetrics, DeviceIdentifier);

// Publish a device death message.
await node.PublishDeviceDeathMessage(DeviceIdentifier);

// Handle the node's device birth received event.
node.DeviceBirthReceived += OnVersionBNodeDeviceBirthReceived;

private void OnVersionBNodeDeviceBirthReceived(KeyValuePair<string, List<Metric>> deviceData)
{
	// Do something.
}

// Handle the node's device command received event.
node.DeviceCommandReceived += OnVersionBNodeDeviceCommandReceived;

private void OnVersionBNodeDeviceCommandReceived(Metric metric)
{
	// Do something.
}

// Handle the node's device death received event.
node.DeviceDeathReceived += OnVersionBNodeDeviceDeathReceived;

private void OnVersionBNodeDeviceDeathReceived(string deviceIdentifier)
{
	// Do something.
}

// Stopping a node.
await node.Stop();
```