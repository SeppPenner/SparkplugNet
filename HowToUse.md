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

// Handle the application's disconnected event.
application.OnDisconnected += OnApplicationDisconnected;

// Handle the application's node data received event.
application.OnNodeDataReceived += OnVersionANodeDataReceived;

// Handle the application's device data received event.
application.OnDeviceDataReceived += OnVersionADeviceDataReceived;

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

private void OnApplicationDisconnected()
{
    // Do something.
}

private static void OnVersionANodeDataReceived(KuraMetric metric)
{
    // Do something.
}

private static void OnVersionADeviceDataReceived(KuraMetric metric)
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

// Handle the application's disconnected event.
application.OnDisconnected += OnApplicationDisconnected;

// Handle the application's node data received event.
application.OnNodeDataReceived += OnVersionBNodeDataReceived;

// Handle the application's device data received event.
application.OnDeviceDataReceived += OnVersionBDeviceDataReceived;

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

private void OnApplicationDisconnected()
{
    // Do something.
}

private static void OnVersionBNodeDataReceived(Metric metric)
{
    // Do something.
}

private static void OnVersionBDeviceDataReceived(Metric metric)
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

## Wait for the primary host

Set `WaitForPrimaryHost` on the node options and use the same `ScadaHostIdentifier` as the primary application.

```csharp
var options = new SparkplugNodeOptions(
    brokerAddress: "localhost",
    clientId: "edge1",
    scadaHostIdentifier: "host1",
    groupIdentifier: "group1",
    edgeNodeIdentifier: "edge1",
    cancellationToken: cancellationToken)
{
    WaitForPrimaryHost = true
};

await node.Start(options);
```

With this option, `Start` waits for an online `STATE` and publishes the birth messages before returning. The cancellation token or `Stop()` can end a pending wait. After the primary host goes offline, the node reconnects and waits again. Calls that publish device births or metric data require the node to have published its birth with the host online.

## Use several brokers

Both the application and the node accept further brokers in `AdditionalBrokers`. All brokers use the same MQTT settings as the configured one.

```csharp
var applicationOptions = new SparkplugApplicationOptions(
    brokerAddress: "broker1",
    clientId: "host1",
    scadaHostIdentifier: "host1",
    isPrimaryApplication: true,
    cancellationToken: cancellationToken)
{
    AdditionalBrokers = [new SparkplugBrokerEndpoint("broker2", 1883)]
};

var nodeOptions = new SparkplugNodeOptions(
    brokerAddress: "broker1",
    clientId: "edge1",
    scadaHostIdentifier: "host1",
    groupIdentifier: "group1",
    edgeNodeIdentifier: "edge1",
    cancellationToken: cancellationToken)
{
    WaitForPrimaryHost = true,
    PrimaryHostOnlineTimeout = TimeSpan.FromSeconds(5),
    AdditionalBrokers = [new SparkplugBrokerEndpoint("broker2", 1883)]
};
```

The application connects to all brokers at the same time and publishes its `STATE` on each of them. The node is connected to one broker at a time. It starts with the configured broker and moves on to the next one in the list if the primary host does not report online there within `PrimaryHostOnlineTimeout`, so `AdditionalBrokers` on a node requires `WaitForPrimaryHost = true`. `PublishOfflineState()` publishes the offline `STATE` of the application on every connected broker, for a shutdown that closes the connections itself. `Stop()` remains the normal way to shut down.

## Further Sparkplug 3.0 options of the application

- `MessageReorderTimeout`: When set, the application holds back messages that arrive out of sequence and waits this long for the missing ones. If they do not arrive, it requests a rebirth. `null` (the default) disables the reordering.
- `IncludeDataTypesInCommandMessages`: When `true`, NCMD and DCMD messages carry the data type of each metric, for receivers that require it. The default is `false`.

## Connection via WebSocket to MQTT server
As discussed in https://github.com/SeppPenner/SparkplugNet/issues/52, I don't want to stop using 1883 as default port. Therefore, when using WebSockets, you need to specify the complete url (e.g. `mqtt.eclipseprojects.io:443/mqtt` or maybe `test.something.com:443`) as option for `BrokerAddress`. `Port` is ignored in this case.
