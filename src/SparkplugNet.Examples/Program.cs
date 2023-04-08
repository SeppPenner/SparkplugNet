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
    /// The version B metrics for an application.
    /// </summary>
    private static readonly List<VersionBData.Metric> VersionBMetricsApplication = new()
    {
        new VersionBData.Metric
        {
            Name = "temperatureApplication", ValueCase = (uint)VersionBData.DataType.Float, FloatValue = 1.20f
        },
        new VersionBData.Metric
        {
            Name = "climateactiveApplication",
            ValueCase = (uint)VersionBData.DataType.Boolean, BooleanValue = true
        }
    };

    /// <summary>
    /// The version B metrics for a node.
    /// </summary>
    private static readonly List<VersionBData.Metric> VersionBMetricsNode = new()
    {
        new VersionBData.Metric
        {
            Name = "temperatureNode", ValueCase = (uint)VersionBData.DataType.Float, FloatValue = 1.243f
        },
        new VersionBData.Metric
        {
            Name = "climateactiveNode",
            ValueCase = (uint)VersionBData.DataType.Boolean, BooleanValue = true
        }
    };

    /// <summary>
    /// The version B metrics for a device.
    /// </summary>
    private static readonly List<VersionBData.Metric> VersionBMetricsDevice = new()
    {
        new VersionBData.Metric
        {
            Name = "temperatureDevice", ValueCase = (uint)VersionBData.DataType.Float, FloatValue = 1.243f
        },
        new VersionBData.Metric
        {
            Name = "climateactiveDevice",
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
    /// Runs a version B simulation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private static async Task RunVersionB()
    {
        await RunVersionBApplication();
        await RunVersionBNode();
    }

    /// <summary>
    /// Runs the version B application.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
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
        var application = new VersionB.SparkplugApplication(VersionBMetricsApplication, Log.Logger);

        // Start an application.
        Log.Information("Starting application...");
        await application.Start(applicationOptions);
        Log.Information("Application started...");

        // Handles the application's connected and disconnected events.
        application.ConnectedAsync += OnApplicationVersionBConnected;
        application.DisconnectedAsync += OnApplicationVersionBDisconnected;

        // Handles the application's device related events.
        application.DeviceBirthReceivedAsync += OnApplicationVersionBDeviceBirthReceived;
        application.DeviceDataReceivedAsync += OnApplicationVersionBDeviceDataReceived;
        application.DeviceDeathReceivedAsync += OnApplicationVersionBDeviceDeathReceived;

        // Handles the application's node related events.
        application.NodeBirthReceivedAsync += OnApplicationVersionBNodeBirthReceived;
        application.NodeDataReceivedAsync += OnApplicationVersionBNodeDataReceived;
        application.NodeDeathReceivedAsync += OnApplicationVersionBNodeDeathReceived;

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
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
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
        var node = new VersionB.SparkplugNode(VersionBMetricsNode, Log.Logger);

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
        node.ConnectedAsync += OnVersionBNodeConnected;
        node.DisconnectedAsync += OnVersionBNodeDisconnected;

        // Handles the node's device related events.
        node.DeviceBirthPublishingAsync += OnVersionBNodeDeviceBirthPublishing;
        node.DeviceCommandReceivedAsync += OnVersionBNodeDeviceCommandReceived;
        node.DeviceDeathPublishingAsync += OnVersionBNodeDeviceDeathPublishing;

        // Handles the node's node command received event.
        node.NodeCommandReceivedAsync += OnVersionBNodeNodeCommandReceived;

        // Handles the node's status message received event.
        node.StatusMessageReceivedAsync += OnVersionBNodeStatusMessageReceived;

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

    #region VersionBEvents
    /// <summary>
    /// Handles the connected callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBConnected(Core.SparkplugBase<VersionBData.Metric>.SparkplugEventArgs arg)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the disconnected callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBDisconnected(VersionB.SparkplugApplication.SparkplugEventArgs arg)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the device birth received callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBDeviceBirthReceived(Core.SparkplugBase<VersionBData.Metric>.DeviceBirthEventArgs arg)
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
    private static Task OnApplicationVersionBDeviceDeathReceived(Core.SparkplugBase<VersionBData.Metric>.DeviceEventArgs arg)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the node birth received callback for version B applications.
    /// </summary>
    private static Task OnApplicationVersionBNodeBirthReceived(Core.SparkplugBase<VersionBData.Metric>.NodeBirthEventArgs arg)
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
    private static Task OnApplicationVersionBNodeDeathReceived(Core.SparkplugBase<VersionBData.Metric>.NodeEventArgs arg)
    {
        // Do something.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the connected callback for version B nodes.
    /// </summary>
    private static Task OnVersionBNodeConnected(Core.SparkplugBase<VersionBData.Metric>.SparkplugEventArgs arg)
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
