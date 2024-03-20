// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The main program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SparkplugNet.Examples;

/// <summary>
/// The main program.
/// </summary>
public sealed class Program
{
    /// <summary>
    /// The cancellation token source.
    /// </summary>
    private static readonly CancellationTokenSource CancellationTokenSource = new();

    /// <summary>
    /// The version A metrics for an application.
    /// </summary>
    private static readonly List<VersionAData.KuraMetric> VersionAMetricsApplication =
    [
        new ("temperatureApplication", VersionAData.DataType.Double, 1.20),
        new ("climateactiveApplication", VersionAData.DataType.Boolean, true)
    ];

    /// <summary>
    /// The version A metrics for a node.
    /// </summary>
    private static readonly List<VersionAData.KuraMetric> VersionAMetricsNode =
    [
        new ("temperatureNode", VersionAData.DataType.Double, 1.20),
        new ("climateactiveNode", VersionAData.DataType.Boolean, true)
    ];

    /// <summary>
    /// The version A metrics for a device.
    /// </summary>
    private static readonly List<VersionAData.KuraMetric> VersionAMetricsDevice =
    [
        new ("temperatureDevice", VersionAData.DataType.Double, 1.20),
        new ("climateactiveDevice", VersionAData.DataType.Boolean, true)
    ];

    /// <summary>
    /// The version A metrics for an application.
    /// </summary>
    private static readonly List<VersionBData.Metric> VersionBMetricsApplication =
    [
        new VersionBData.Metric("temperatureApplication", VersionBData.DataType.Float, 1.20f),
        new VersionBData.Metric("climateactiveApplication", VersionBData.DataType.Boolean, true)
    ];

    /// <summary>
    /// The version A metrics for a node.
    /// </summary>
    private static readonly List<VersionBData.Metric> VersionBMetricsNode =
    [
        new VersionBData.Metric("temperatureNode", VersionBData.DataType.Float, 1.243f),
        new VersionBData.Metric("climateactiveNode", VersionBData.DataType.Boolean, true)
    ];

    /// <summary>
    /// The version A metrics for a device.
    /// </summary>
    private static readonly List<VersionBData.Metric> VersionBMetricsDevice =
    [
        new VersionBData.Metric("temperatureDevice", VersionBData.DataType.Float, 1.243f),
        new VersionBData.Metric("climateactiveDevice", VersionBData.DataType.Boolean, true)
    ];

    /// <summary>
    /// The main method.
    /// </summary>
    public static async Task Main()
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            //await RunVersionA();
            await RunVersionB();

            Log.Information("Simulation is done.");
        }
        catch (Exception ex)
        {
            Log.Error("An exception occurred: {Exception}", ex);
        }
        finally
        {
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Runs a version A simulation.
    /// </summary>
    private static async Task RunVersionA()
    {
        await RunVersionAApplication();
        await RunVersionANode();
    }

    /// <summary>
    /// Runs a version B simulation.
    /// </summary>
    private static async Task RunVersionB()
    {
        await RunVersionBApplication();
        await RunVersionBNode();
    }

    /// <summary>
    /// Runs the version A application.
    /// </summary>
    private static async Task RunVersionAApplication()
    {
        var applicationOptions = new SparkplugApplicationOptions(
             "localhost",
             1883,
             nameof(RunVersionAApplication),
             "user",
             "password",
             false,
             "scada1",
             TimeSpan.FromSeconds(30),
             SparkplugMqttProtocolVersion.V311,
             null,
             null,
             null,
             true,
             CancellationTokenSource.Token);
        var application = new VersionA.SparkplugApplication(VersionAMetricsApplication, SparkplugSpecificationVersion.Version22);

        // Start an application.
        Log.Information("Starting application...");
        await application.Start(applicationOptions);
        Log.Information("Application started...");

        // Handles the application's connected and disconnected events.
        application.Connected += OnApplicationVersionAConnected;
        application.Disconnected += OnApplicationVersionADisconnected;

        // Handles the application's device related events.
        application.DeviceBirthReceived += OnApplicationVersionADeviceBirthReceived;
        application.DeviceDataReceived += OnApplicationVersionADeviceDataReceived;
        application.DeviceDeathReceived += OnApplicationVersionADeviceDeathReceived;

        // Handles the application's node related events.
        application.NodeBirthReceived += OnApplicationVersionANodeBirthReceived;
        application.NodeDataReceived += OnApplicationVersionANodeDataReceived;
        application.NodeDeathReceived += OnApplicationVersionANodeDeathReceived;

        // Publish node commands.
        Log.Information("Publishing a node command ...");
        await application.PublishNodeCommand(VersionAMetricsApplication, "group1", "edge1");

        // Publish device commands.
        Log.Information("Publishing a device command ...");
        await application.PublishDeviceCommand(VersionAMetricsApplication, "group1", "edge1", "device1");

        // Get the known metrics from an application.
        var currentlyKnownMetrics = application.KnownMetrics;

        // Get the device states from an application.
        var currentDeviceStates = application.DeviceStates;

        // Get the node states from an application.
        var currentNodeStates = application.NodeStates;

        // Check whether an application is connected.
        var isApplicationConnected = application.IsConnected;

        // Stopping an application.
        await application.Stop();
        Log.Information("Application stopped...");
    }

    /// <summary>
    /// Runs the version A node.
    /// </summary>
    private static async Task RunVersionANode()
    {
        var nodeOptions = new SparkplugNodeOptions(
            "localhost",
            1883,
            "node 1",
            "user",
            "password",
            false,
            "scada1B",
            TimeSpan.FromSeconds(30),
            SparkplugMqttProtocolVersion.V311,
            null,
            null,
            null,
            "group1",
            "node1",
            CancellationTokenSource.Token);
        var node = new VersionA.SparkplugNode(VersionAMetricsNode, SparkplugSpecificationVersion.Version22);

        // Start a node.
        Log.Information("Starting node...");
        await node.Start(nodeOptions);
        Log.Information("Node started...");

        // Publish node metrics.
        await node.PublishMetrics(VersionAMetricsNode);

        // Get the known node metrics from a node.
        var currentlyKnownMetrics = node.KnownMetrics;

        // Check whether a node is connected.
        var isApplicationConnected = node.IsConnected;

        // Handles the node's connected and disconnected events.
        node.Connected += OnVersionANodeConnected;
        node.Disconnected += OnVersionANodeDisconnected;

        // Handles the node's device related events.
        node.DeviceBirthPublishing += OnVersionANodeDeviceBirthPublishing;
        node.DeviceCommandReceived += OnVersionANodeDeviceCommandReceived;
        node.DeviceDeathPublishing += OnVersionANodeDeviceDeathPublishing;

        // Handles the node's node command received event.
        node.NodeCommandReceived += OnVersionANodeNodeCommandReceived;

        // Handles the node's status message received event.
        node.StatusMessageReceived += OnVersionANodeStatusMessageReceived;

        // Get the known devices.
        var knownDevices = node.KnownDevices;

        // Handling devices.
        const string DeviceIdentifier = "device1";

        // Publish a device birth message.
        await node.PublishDeviceBirthMessage(VersionAMetricsDevice, DeviceIdentifier);

        // Publish a device data message.
        await node.PublishDeviceData(VersionAMetricsDevice, DeviceIdentifier);

        // Publish a device death message.
        await node.PublishDeviceDeathMessage(DeviceIdentifier);

        // Stopping a node.
        await node.Stop();
        Log.Information("Node stopped...");
    }

    /// <summary>
    /// Runs the version B application.
    /// </summary>
    private static async Task RunVersionBApplication()
    {
        var applicationOptions = new SparkplugApplicationOptions(
            "localhost",
            1883,
            nameof(RunVersionBApplication),
            "user",
            "password",
            false,
            "scada1",
            TimeSpan.FromSeconds(30),
            SparkplugMqttProtocolVersion.V311,
            null,
            null,
            null,
            true,
            CancellationTokenSource.Token);
        var application = new VersionB.SparkplugApplication(VersionBMetricsApplication, SparkplugSpecificationVersion.Version22);

        // Start an application.
        Log.Information("Starting application...");
        await application.Start(applicationOptions);
        Log.Information("Application started...");

        // Handles the application's connected and disconnected events.
        application.Connected += OnApplicationVersionBConnected;
        application.Disconnected += OnApplicationVersionBDisconnected;

        // Handles the application's device related events.
        application.DeviceBirthReceived += OnApplicationVersionBDeviceBirthReceived;
        application.DeviceDataReceived += OnApplicationVersionBDeviceDataReceived;
        application.DeviceDeathReceived += OnApplicationVersionBDeviceDeathReceived;

        // Handles the application's node related events.
        application.NodeBirthReceived += OnApplicationVersionBNodeBirthReceived;
        application.NodeDataReceived += OnApplicationVersionBNodeDataReceived;
        application.NodeDeathReceived += OnApplicationVersionBNodeDeathReceived;

        // Publish node commands.
        Log.Information("Publishing a node command ...");
        await application.PublishNodeCommand(VersionBMetricsApplication, "group1", "edge1");

        // Publish device commands.
        Log.Information("Publishing a device command ...");
        await application.PublishDeviceCommand(VersionBMetricsApplication, "group1", "edge1", "device1");

        // Get the known metrics from an application.
        var currentlyKnownMetrics = application.KnownMetrics;

        // Get the device states from an application.
        var currentDeviceStates = application.DeviceStates;

        // Get the node states from an application.
        var currentNodeStates = application.NodeStates;

        // Check whether an application is connected.
        var isApplicationConnected = application.IsConnected;

        // Stopping an application.
        await application.Stop();
        Log.Information("Application stopped...");
    }

    /// <summary>
    /// Runs the version B node.
    /// </summary>
    private static async Task RunVersionBNode()
    {
        var nodeOptions = new SparkplugNodeOptions(
            "localhost",
            1883,
            "node 1",
            "user",
            "password",
            false,
            "scada1B",
            TimeSpan.FromSeconds(30),
            SparkplugMqttProtocolVersion.V311,
            null,
            null,
            null,
            "group1",
            "node1",
            CancellationTokenSource.Token);
        var node = new VersionB.SparkplugNode(VersionBMetricsNode, SparkplugSpecificationVersion.Version22);

        // Start a node.
        Log.Information("Starting node...");
        await node.Start(nodeOptions);
        Log.Information("Node started...");

        // Publish node metrics.
        await node.PublishMetrics(VersionBMetricsNode);

        // Get the known node metrics from a node.
        var currentlyKnownMetrics = node.KnownMetrics;

        // Check whether a node is connected.
        var isApplicationConnected = node.IsConnected;

        // Handles the node's connected and disconnected events.
        node.Connected += OnVersionBNodeConnected;
        node.Disconnected += OnVersionBNodeDisconnected;

        // Handles the node's device related events.
        node.DeviceBirthPublishing += OnVersionBNodeDeviceBirthPublishing;
        node.DeviceCommandReceived += OnVersionBNodeDeviceCommandReceived;
        node.DeviceDeathPublishing += OnVersionBNodeDeviceDeathPublishing;

        // Handles the node's node command received event.
        node.NodeCommandReceived += OnVersionBNodeNodeCommandReceived;

        // Handles the node's status message received event.
        node.StatusMessageReceived += OnVersionBNodeStatusMessageReceived;

        // Get the known devices.
        var knownDevices = node.KnownDevices;

        // Handling devices.
        const string DeviceIdentifier = "device1";

        // Publish a device birth message.
        await node.PublishDeviceBirthMessage(VersionBMetricsDevice, DeviceIdentifier);

        // Publish a device data message.
        await node.PublishDeviceData(VersionBMetricsDevice, DeviceIdentifier);

        // Publish a device death message.
        await node.PublishDeviceDeathMessage(DeviceIdentifier);

        // Stopping a node.
        await node.Stop();
        Log.Information("Node stopped...");
    }

    #region VersionAEvents
    /// <summary>
    /// Handles the connected callback for version A applications.
    /// </summary>
    private static Task OnApplicationVersionAConnected(Core.SparkplugBase<VersionAData.KuraMetric>.SparkplugEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the disconnected callback for version A applications.
    /// </summary>
    private static Task OnApplicationVersionADisconnected(VersionA.SparkplugApplication.SparkplugEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device birth received callback for version A applications.
    /// </summary>
    private static Task OnApplicationVersionADeviceBirthReceived(Core.SparkplugBase<VersionAData.KuraMetric>.DeviceBirthEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device data received callback for version A applications.
    /// </summary>
    private static Task OnApplicationVersionADeviceDataReceived(VersionA.SparkplugApplication.DeviceDataEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device death received callback for version A applications.
    /// </summary>
    private static Task OnApplicationVersionADeviceDeathReceived(Core.SparkplugBase<VersionAData.KuraMetric>.DeviceEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node birth received callback for version A applications.
    /// </summary>
    private static Task OnApplicationVersionANodeBirthReceived(Core.SparkplugBase<VersionAData.KuraMetric>.NodeBirthEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node data received callback for version A applications.
    /// </summary>
    private static Task OnApplicationVersionANodeDataReceived(VersionA.SparkplugApplication.NodeDataEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node death received callback for version A applications.
    /// </summary>
    private static Task OnApplicationVersionANodeDeathReceived(Core.SparkplugBase<VersionAData.KuraMetric>.NodeEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the connected callback for version A nodes.
    /// </summary>
    private static Task OnVersionANodeConnected(Core.SparkplugBase<VersionAData.KuraMetric>.SparkplugEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the disconnected callback for version A nodes.
    /// </summary>
    private static Task OnVersionANodeDisconnected(VersionA.SparkplugNode.SparkplugEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device birth callback for version A nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionANodeDeviceBirthPublishing(VersionA.SparkplugNode.DeviceBirthEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device command callback for version A nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionANodeDeviceCommandReceived(VersionA.SparkplugNode.NodeCommandEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device death callback for version A nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionANodeDeviceDeathPublishing(VersionA.SparkplugNode.DeviceEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node command callback for version A nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionANodeNodeCommandReceived(VersionA.SparkplugNode.NodeCommandEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the status message callback for version A nodes.
    /// </summary>
    /// <param name="args">The status.</param>
    private static Task OnVersionANodeStatusMessageReceived(VersionA.SparkplugNode.StatusMessageEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }
    #endregion

    #region VersionBEvents
    /// <summary>
    /// Handles the connected callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBConnected(Core.SparkplugBase<VersionBData.Metric>.SparkplugEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the disconnected callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBDisconnected(VersionB.SparkplugApplication.SparkplugEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device birth received callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBDeviceBirthReceived(Core.SparkplugBase<VersionBData.Metric>.DeviceBirthEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device data callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBDeviceDataReceived(VersionB.SparkplugApplication.DeviceDataEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device death received callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBDeviceDeathReceived(Core.SparkplugBase<VersionBData.Metric>.DeviceEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node birth received callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBNodeBirthReceived(Core.SparkplugBase<VersionBData.Metric>.NodeBirthEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node data callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBNodeDataReceived(VersionB.SparkplugApplication.NodeDataEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node death received callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBNodeDeathReceived(Core.SparkplugBase<VersionBData.Metric>.NodeEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the connected callback for version B nodes.
    /// </summary>
    private static Task OnVersionBNodeConnected(Core.SparkplugBase<VersionBData.Metric>.SparkplugEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the disconnected callback for version B nodes.
    /// </summary>
    private static Task OnVersionBNodeDisconnected(VersionB.SparkplugNode.SparkplugEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device birth callback for version B nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionBNodeDeviceBirthPublishing(VersionB.SparkplugNode.DeviceBirthEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device command callback for version B nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionBNodeDeviceCommandReceived(VersionB.SparkplugNode.NodeCommandEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device death callback for version B nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionBNodeDeviceDeathPublishing(VersionB.SparkplugNode.DeviceEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary> 
    /// Handles the node command callback for version B nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionBNodeNodeCommandReceived(VersionB.SparkplugNode.NodeCommandEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the status message callback for version B nodes.
    /// </summary>
    /// <param name="args">The args.</param>
    private static Task OnVersionBNodeStatusMessageReceived(VersionB.SparkplugNode.StatusMessageEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }
    #endregion
}
