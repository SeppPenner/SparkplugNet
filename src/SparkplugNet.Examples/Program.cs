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
    private static readonly CancellationTokenSource CancellationTokenSource = new ();

    /// <summary>
    /// The version A metrics.
    /// </summary>
    private static readonly List<VersionAData.KuraMetric> VersionAMetrics = new ()
    {
        new ()
        {
            Name = "Test", Type = VersionAData.DataType.Double, DoubleValue = 1.20
        },
        new ()
        {
            Name = "Test2", Type = VersionAData.DataType.Bool, BoolValue = true
        }
    };

    /// <summary>
    /// The version A metrics.
    /// </summary>
    private static readonly List<VersionBData.Metric> VersionBMetrics = new ()
    {
        new VersionBData.Metric
        {
            Name = "Test", DataType = (uint)VersionBData.DataType.Double, DoubleValue = 1.20
        },
        new VersionBData.Metric
        {
            Name = "Test2",
            DataType = (uint)VersionBData.DataType.Boolean, BooleanValue = true
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

            await RunVersionA();
            // await RunVersionB();

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
        var applicationOptions = new SparkplugApplicationOptions("localhost", 1883, "application1", "user", "password", false, "scada1", TimeSpan.FromSeconds(30), true, null, null, CancellationTokenSource.Token);

        // Start an application.
        Log.Information("Starting application...");
        await application.Start(applicationOptions);
        Log.Information("Application started...");

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

        // Handle the application's disconnected event.
        application.OnDisconnected += OnVersionAApplicationDisconnected;

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
        var nodeOptions = new SparkplugNodeOptions("localhost", 1883, "node 1", "user", "password", false, "scada1", "group1", "node1", TimeSpan.FromSeconds(30), null, null, CancellationTokenSource.Token);

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
        node.OnDisconnected += OnVersionANodeDisconnected;

        // Handle the node's node command received event.
        node.NodeCommandReceived += OnVersionANodeNodeCommandReceived;

        // Handles the node's status message received event.
        node.StatusMessageReceived += OnVersionANodeStatusMessageReceived;

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
        node.DeviceBirthReceived += OnVersionANodeDeviceBirthReceived;

        // Handle the node's device command received event.
        node.DeviceCommandReceived += OnVersionANodeDeviceCommandReceived;

        // Handle the node's device death received event.
        node.DeviceDeathReceived += OnVersionANodeDeviceDeathReceived;

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
        var applicationOptions = new SparkplugApplicationOptions("localhost", 1883, "application1", "user", "password", false, "scada1", TimeSpan.FromSeconds(30), true, null, null, CancellationTokenSource.Token);

        // Start an application.
        Log.Information("Starting application...");
        await application.Start(applicationOptions);
        Log.Information("Application started...");

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

        // Handle the application's disconnected event.
        application.OnDisconnected += OnVersionBApplicationDisconnected;

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
        var nodeOptions = new SparkplugNodeOptions("localhost", 1883, "node 1", "user", "password", false, "scada1", "group1", "node1", TimeSpan.FromSeconds(30), null, null, CancellationTokenSource.Token);

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
        node.OnDisconnected += OnVersionBNodeDisconnected;

        // Handle the node's node command received event.
        node.NodeCommandReceived += OnVersionBNodeNodeCommandReceived;

        // Handles the node's status message received event.
        node.StatusMessageReceived += OnVersionBNodeStatusMessageReceived;

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
        node.DeviceBirthReceived += OnVersionBNodeDeviceBirthReceived;

        // Handle the node's device command received event.
        node.DeviceCommandReceived += OnVersionBNodeDeviceCommandReceived;

        // Handle the node's device death received event.
        node.DeviceDeathReceived += OnVersionBNodeDeviceDeathReceived;

        // Stopping a node.
        await node.Stop();
        Log.Information("Node stopped...");
    }

    /// <summary>
    /// Handles the disconnected callback for version A applications.
    /// </summary>
    private static void OnVersionAApplicationDisconnected()
    {
        // Do something.
    }

    /// <summary>
    /// Handles the disconnected callback for version B applications.
    /// </summary>
    private static void OnVersionBApplicationDisconnected()
    {
        // Do something.
    }

    /// <summary>
    /// Handles the disconnected callback for version A nodes.
    /// </summary>
    private static void OnVersionANodeDisconnected()
    {
        // Do something.
    }

    /// <summary>
    /// Handles the disconnected callback for version B nodes.
    /// </summary>
    private static void OnVersionBNodeDisconnected()
    {
        // Do something.
    }

    /// <summary>
    /// Handles the node command callback for version A nodes.
    /// </summary>
    /// <param name="metric">The received metric.</param>
    private static void OnVersionANodeNodeCommandReceived(VersionAData.KuraMetric metric)
    {
        // Do something.
    }

    /// <summary>
    /// Handles the node command callback for version B nodes.
    /// </summary>
    /// <param name="metric">The received metric.</param>
    private static void OnVersionBNodeNodeCommandReceived(VersionBData.Metric metric)
    {
        // Do something.
    }

    /// <summary>
    /// Handles the status message callback for version A nodes.
    /// </summary>
    /// <param name="status">The status.</param>
    private static void OnVersionANodeStatusMessageReceived(string status)
    {
        // Do something.
    }

    /// <summary>
    /// Handles the status message callback for version B nodes.
    /// </summary>
    /// <param name="status">The status.</param>
    private static void OnVersionBNodeStatusMessageReceived(string status)
    {
        // Do something.
    }

    /// <summary>
    /// Handles the device birth callback for version A nodes.
    /// </summary>
    /// <param name="deviceData">The received device data.</param>
    private static void OnVersionANodeDeviceBirthReceived(KeyValuePair<string, List<VersionAData.KuraMetric>> deviceData)
    {
        // Do something.
    }

    /// <summary>
    /// Handles the device birth callback for version B nodes.
    /// </summary>
    /// <param name="deviceData">The received device data.</param>
    private static void OnVersionBNodeDeviceBirthReceived(KeyValuePair<string, List<VersionBData.Metric>> deviceData)
    {
        // Do something.
    }

    /// <summary>
    /// Handles the device command callback for version A nodes.
    /// </summary>
    /// <param name="metric">The received metric.</param>
    private static void OnVersionANodeDeviceCommandReceived(VersionAData.KuraMetric metric)
    {
        // Do something.
    }

    /// <summary>
    /// Handles the device command callback for version B nodes.
    /// </summary>
    /// <param name="metric">The received metric.</param>
    private static void OnVersionBNodeDeviceCommandReceived(VersionBData.Metric metric)
    {
        // Do something.
    }

    /// <summary>
    /// Handles the device death callback for version A nodes.
    /// </summary>
    /// <param name="deviceIdentifier">The received device identifier.</param>
    private static void OnVersionANodeDeviceDeathReceived(string deviceIdentifier)
    {
        // Do something.
    }

    /// <summary>
    /// Handles the device death callback for version B nodes.
    /// </summary>
    /// <param name="deviceIdentifier">The received device identifier.</param>
    private static void OnVersionBNodeDeviceDeathReceived(string deviceIdentifier)
    {
        // Do something.
    }
}
