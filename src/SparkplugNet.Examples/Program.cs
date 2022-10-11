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
public class Program
{
    /// <summary>
    /// The cancellation token source.
    /// </summary>
    private static readonly CancellationTokenSource CancellationTokenSource = new();

    /// <summary>
    /// The version A metrics.
    /// </summary>
    private static readonly List<VersionAData.KuraMetric> VersionAMetrics = new()
    {
        new ()
        {
            Name = "Test", DataType = VersionAData.DataType.Double, DoubleValue = 1.20
        },
        new ()
        {
            Name = "Test2", DataType = VersionAData.DataType.Boolean, BooleanValue = true
        }
    };

    /// <summary>
    /// The version A metrics.
    /// </summary>
    private static readonly List<VersionBData.Metric> VersionBMetrics = new()
    {
        new VersionBData.Metric
        {
            Name = "temperature", ValueCase = (uint)VersionBData.DataType.Float, FloatValue = 1.20f
        },
        new VersionBData.Metric
        {
            Name = "climateactive",
            ValueCase = (uint)VersionBData.DataType.Boolean, BooleanValue = true
        }
    };

    /// <summary>
    /// The main method.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public static async Task Main()
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

           // await RunVersionA();
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
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private static async Task RunVersionA()
    {
        await RunVersionAApplication();
        await RunVersionANode();
    }

    /// <summary>
    /// Runs a version B simulation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private static async Task RunVersionB()
    {
        await RunVersionBApplication();
        await RunVersionBNode();
    }

    /// <summary>
    /// Runs the version A application.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private static async Task RunVersionAApplication()
    {
        var applicationMetrics = new List<VersionAData.KuraMetric>(VersionAMetrics);
        var application = new VersionA.SparkplugApplication(applicationMetrics, Log.Logger);

        var applicationOptions = new SparkplugApplicationOptions("localhost", 1883, nameof(RunVersionAApplication), "user", "password", false, "scada1", TimeSpan.FromSeconds(30), true, null, null, CancellationTokenSource.Token);

        // Start an application.
        Log.Information("Starting application...");
        await application.Start(applicationOptions);
        Log.Information("Application started...");

        // Handle the application's disconnected event.
        application.DisconnectedAsync += ApplicationVersionA_DisconnectedAsync;

        // Handle the application's node data received event.
        application.NodeDataReceivedAsync += OnVersionANodeDataReceived;

        // Handle the application's device data received event.
        application.DeviceDataReceivedAsync += OnVersionADeviceDataReceived;

        // Publish node commands.
        Log.Information("Publishing a node command ...");
        await application.PublishNodeCommand(applicationMetrics, "group1", "edge1");

        // Publish device commands.
        Log.Information("Publishing a device command ...");
        await application.PublishDeviceCommand(applicationMetrics, "group1", "edge1", "device1");

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
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private static async Task RunVersionANode()
    {
        var nodeMetrics = new List<VersionAData.KuraMetric>(VersionAMetrics);
        var node = new VersionA.SparkplugNode(nodeMetrics, Log.Logger);
        var nodeOptions = new SparkplugNodeOptions("localhost", 1883, "node 1", "user", "password", false, "scada1A", "group1", "node1", TimeSpan.FromSeconds(30), null, null, CancellationTokenSource.Token);

        // Start a node.
        Log.Information("Starting node...");
        await node.Start(nodeOptions);
        Log.Information("Node started...");

        // Publish node metrics.
        await node.PublishMetrics(nodeMetrics);

        // Get the known node metrics from a node.
        var currentlyKnownMetrics = node.KnownMetrics;

        // Check whether a node is connected.
        var isApplicationConnected = node.IsConnected;

        // Handle the node's disconnected event.
        node.DisconnectedAsync += OnVersionANodeDisconnected;

        // Handle the node's node command received event.
        node.NodeCommandReceivedAsync += OnVersionANodeNodeCommandReceived;

        // Handles the node's status message received event.
        node.StatusMessageReceivedAsync += OnVersionANodeStatusMessageReceived;

        // Get the known devices.
        var knownDevices = node.KnownDevices;

        // Handling devices.
        const string DeviceIdentifier = "device1";
        var deviceMetrics = new List<VersionAData.KuraMetric>(VersionAMetrics);

        // Publish a device birth message.
        await node.PublishDeviceBirthMessage(deviceMetrics, DeviceIdentifier);

        // Publish a device data message.
        await node.PublishDeviceData(deviceMetrics, DeviceIdentifier);

        // Publish a device death message.
        await node.PublishDeviceDeathMessage(DeviceIdentifier);

        // Handle the node's device birth received event.
        node.DeviceBirthPublishingAsync += OnVersionANodeDeviceBirthReceived;

        // Handle the node's device command received event.
        node.DeviceCommandReceivedAsync += OnVersionANodeDeviceCommandReceived;

        // Handle the node's device death received event.
        node.DeviceDeathPublishingAsync += OnVersionANodeDeviceDeathReceived;

        // Stopping a node.
        await node.Stop();
        Log.Information("Node stopped...");
    }

    /// <summary>
    /// Runs the version B application.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private static async Task RunVersionBApplication()
    {
        var applicationMetrics = new List<VersionBData.Metric>(VersionBMetrics);
        var application = new VersionB.SparkplugApplication(applicationMetrics, Log.Logger);
        var applicationOptions = new SparkplugApplicationOptions("localhost", 1883, nameof(RunVersionBApplication), "user", "password", false, "scada1", TimeSpan.FromSeconds(30), true, null, null, CancellationTokenSource.Token);

        // Start an application.
        Log.Information("Starting application...");
        await application.Start(applicationOptions);
        Log.Information("Application started...");

        // Handle the application's disconnected event.
        application.DisconnectedAsync += ApplicationVersionB_DisconnectedAsync;

        // Handle the application's node data received event.
        application.NodeDataReceivedAsync += OnVersionBNodeDataReceived;

        // Handle the application's device data received event.
        application.DeviceDataReceivedAsync += OnVersionBDeviceDataReceived;

        // Publish node commands.
        Log.Information("Publishing a node command ...");
        await application.PublishNodeCommand(applicationMetrics, "group1", "edge1");

        // Publish device commands.
        Log.Information("Publishing a device command ...");
        await application.PublishDeviceCommand(applicationMetrics, "group1", "edge1", "device1");

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
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private static async Task RunVersionBNode()
    {
        var nodeMetrics = new List<VersionBData.Metric>(VersionBMetrics);
        var node = new VersionB.SparkplugNode(nodeMetrics, Log.Logger);
        var nodeOptions = new SparkplugNodeOptions("localhost", 1883, "node 1", "user", "password", false, "scada1B", "group1", "node1", TimeSpan.FromSeconds(30), null, null, CancellationTokenSource.Token);

        // Start a node.
        Log.Information("Starting node...");
        await node.Start(nodeOptions);
        Log.Information("Node started...");

        // Publish node metrics.
        await node.PublishMetrics(nodeMetrics);

        // Get the known node metrics from a node.
        var currentlyKnownMetrics = node.KnownMetrics;

        // Check whether a node is connected.
        var isApplicationConnected = node.IsConnected;

        // Handle the node's disconnected event.
        node.DisconnectedAsync += OnVersionBNodeDisconnected;

        // Handle the node's node command received event.
        node.NodeCommandReceivedAsync += OnVersionBNodeNodeCommandReceived;

        // Handles the node's status message received event.
        node.StatusMessageReceivedAsync += OnVersionBNodeStatusMessageReceived;

        // Get the known devices.
        var knownDevices = node.KnownDevices;

        // Handling devices.
        const string DeviceIdentifier = "device1";
        var deviceMetrics = new List<VersionBData.Metric>(VersionBMetrics);

        // Publish a device birth message.
        await node.PublishDeviceBirthMessage(deviceMetrics, DeviceIdentifier);

        // Publish a device data message.
        await node.PublishDeviceData(deviceMetrics, DeviceIdentifier);

        // Publish a device death message.
        await node.PublishDeviceDeathMessage(DeviceIdentifier);

        // Handle the node's device birth received event.
        node.DeviceBirthPublishingAsync += OnVersionBNodeDeviceBirthReceived;

        // Handle the node's device command received event.
        node.DeviceCommandReceivedAsync += OnVersionBNodeDeviceCommandReceived;

        // Handle the node's device death received event.
        node.DeviceDeathPublishingAsync += OnVersionBNodeDeviceDeathReceived;

        // Stopping a node.
        await node.Stop();
        Log.Information("Node stopped...");
    }

    /// <summary>
    /// Handles the disconnected callback for version A applications.
    /// </summary>
    private static Task ApplicationVersionA_DisconnectedAsync(VersionA.SparkplugApplication.SparkplugEventArgs arg)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the disconnected callback for version B applications.
    /// </summary>
    private static Task ApplicationVersionB_DisconnectedAsync(VersionB.SparkplugApplication.SparkplugEventArgs arg)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node data callback for version A applications.
    /// </summary>
    private static Task OnVersionANodeDataReceived(VersionA.SparkplugApplication.NodeDataEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node data callback for version B applications.
    /// </summary>
    private static Task OnVersionBNodeDataReceived(VersionB.SparkplugApplication.NodeDataEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device data callback for version A applications.
    /// </summary>
    private static Task OnVersionADeviceDataReceived(VersionA.SparkplugApplication.DeviceDataEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device data callback for version B applications.
    /// </summary>
    private static Task OnVersionBDeviceDataReceived(VersionB.SparkplugApplication.DeviceDataEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the disconnected callback for version A nodes.
    /// </summary>
    private static Task OnVersionANodeDisconnected(VersionA.SparkplugNode.SparkplugEventArgs arg)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the disconnected callback for version B nodes.
    /// </summary>
    private static Task OnVersionBNodeDisconnected(VersionB.SparkplugNode.SparkplugEventArgs arg)
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
    /// Handles the node command callback for version B nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionBNodeNodeCommandReceived(VersionB.SparkplugNode.NodeCommandEventArgs args)
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

    /// <summary>
    /// Handles the status message callback for version B nodes.
    /// </summary>
    /// <param name="args">The args.</param>
    private static Task OnVersionBNodeStatusMessageReceived(VersionB.SparkplugNode.StatusMessageEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device birth callback for version A nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionANodeDeviceBirthReceived(VersionA.SparkplugNode.DeviceBirthEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device birth callback for version B nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionBNodeDeviceBirthReceived(VersionB.SparkplugNode.DeviceBirthEventArgs args)
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
    /// Handles the device command callback for version B nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionBNodeDeviceCommandReceived(VersionB.SparkplugNode.NodeCommandEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device death callback for version A nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionANodeDeviceDeathReceived(VersionA.SparkplugNode.DeviceEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device death callback for version B nodes.
    /// </summary>
    /// <param name="args">The received args.</param>
    private static Task OnVersionBNodeDeviceDeathReceived(VersionB.SparkplugNode.DeviceEventArgs args)
    {
        // Do something.
        return Task.CompletedTask;
    }
}
