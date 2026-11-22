# Sparkplug 3.0 conformance testing

## Results

On November 22, 2026, all 13 Eclipse Sparkplug TCK 3.0.0 scenarios for the .NET Host Application and Edge Node passed against HiveMQ CE 2026.5. Each runtime ran 28 cases across MQTT 3.1.1 and MQTT 5, including host shutdown both with and without an MQTT DISCONNECT packet.

| Profile | .NET 10 | .NET 11 |
| --- | ---: | ---: |
| Host Application | 90/90 | 90/90 |
| Edge Node | 170/170 | 170/170 |

The badge counts scenario assertions for these two .NET profiles, including multi-broker operation, message reordering, aliases, templates, datasets and properties. Each protocol-specific check must pass in its matching run.

The Eclipse `report.py` HTML export counts additional checks from the passive MQTT monitor, some of which only report a violation. It can also replace a pass with a later `NOT EXECUTED` for the other MQTT version. The runner checks the original scenario logs and rejects reported failures from the monitor. See [the Eclipse report script](https://github.com/eclipse-sparkplug/sparkplug/blob/v3.0.0/tck/report.py) and [the MQTT monitor](https://github.com/eclipse-sparkplug/sparkplug/blob/v3.0.0/tck/src/main/java/org/eclipse/sparkplug/tck/test/Monitor.java).

## Start the test environment

Start the test brokers and web console from the repository root.

```powershell
docker compose -f testing/sparkplug-tck/compose.yaml up -d --build --wait
```

Docker builds HiveMQ Community Edition with the official Eclipse Sparkplug 3.0.0 TCK extension and builds the TCK web console. The build verifies the downloaded release archive against its SHA-256 checksum. The second HiveMQ broker listens on port 1884 for the multi-broker scenarios.

## Run all scenarios

Use Python 3, the .NET 11 SDK, and the .NET 10 runtime.

```powershell
python testing/sparkplug-tck/run.py
```

The runner exercises the .NET Host Application and Edge Node with MQTT 3.1.1 and MQTT 5 on both runtimes. Run only one suite at a time because the TCK extension accepts one active test. Each run saves the original TCK logs and its per-scenario tally in `local_testing/sparkplug-tck/runs/`. Add `--framework net11.0` for a single runtime. If the TCK's simulated host stalls during MQTT CONNECT, the runner records the timeout, restarts the local test broker and retries that scenario.

The multi-broker scenarios use `AdditionalBrokers` on the host and edge node. On the edge node, `WaitForPrimaryHost` enables failover based on the primary host's STATE. The host's message-ordering scenario enables `MessageReorderTimeout`. Its command scenario enables `IncludeDataTypesInCommandMessages` because that TCK scenario checks the types in the command payload.

## Run from the TCK console

1. Open `http://localhost:3000`.
2. In **MQTT Configuration**, select **Set Default Values**, then **Connect**. The console connects to `ws://localhost:8000/mqtt`.
3. Point the SparkplugNet test application at `localhost:1883` and set `SparkplugSpecificationVersion.Version30`.
4. Enter the Sparkplug Host Application ID. Select **Host Application** or **Edge Node**. For the edge profile, also enter the same group and edge node IDs as in the test application.
5. Select a test and carry out the actions shown in the console. Use `tcp://broker-secondary:1883` as the second broker in the TCK form.

The MQTT broker also accepts Sparkplug 2.2 clients for integration tests. See the [Eclipse Sparkplug 3.0.0 TCK user guide](https://github.com/eclipse-sparkplug/sparkplug/blob/v3.0.0/tck/UserGuide.adoc) for the conformance profiles and reporting workflow.

The TCK archive, report script, broker data, and logs are written under `local_testing/sparkplug-tck/`. The console defaults to `local_testing/sparkplug-tck/log/SparkplugTCKResults.log` for test results.

```powershell
docker compose -f testing/sparkplug-tck/compose.yaml down
```
