# Sparkplug 3.0 conformance testing

Run from the repository root with Docker Compose.

```powershell
docker compose -f testing/sparkplug-tck/compose.yaml up --build --wait
```

Docker builds HiveMQ Community Edition with the official Eclipse Sparkplug 3.0.0 TCK extension and builds the TCK web console. The build verifies the downloaded release archive against its SHA-256 checksum.

1. Open `http://localhost:3000`.
2. In **MQTT Configuration**, select **Set Default Values**, then **Connect**. The console connects to `ws://localhost:8000/mqtt`.
3. Point the SparkplugNet test application at `localhost:1883` and set `SparkplugSpecificationVersion.Version30`.
4. Enter the Sparkplug Host Application ID. Select **Host Application** or **Edge Node**. For the edge profile, also enter the same group and edge node IDs as in the test application.
5. Select a test and carry out the actions shown in the console.

The MQTT broker also accepts Sparkplug 2.2 clients for integration tests. The [TCK user guide](https://github.com/eclipse-sparkplug/sparkplug/blob/master/tck/UserGuide.adoc) documents the 3.0 conformance profiles and the reporting workflow.

The TCK archive, report script, broker data, and logs are written under `local_testing/sparkplug-tck/`. The console defaults to `local_testing/sparkplug-tck/log/SparkplugTCKResults.log` for test results.

```powershell
docker compose -f testing/sparkplug-tck/compose.yaml down
```
