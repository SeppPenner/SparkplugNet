"""Run and verify the Eclipse Sparkplug 3.0.0 Host and Edge scenarios."""

import argparse
from collections import Counter
from datetime import datetime, timezone
from pathlib import Path
import re
import subprocess
import time


ROOT = Path(__file__).resolve().parents[2]
RUNNER = ROOT / "testing/sparkplug-tck/Runner"
OUTPUT = ROOT / "local_testing/sparkplug-tck"
FRAMEWORKS = ("net10.0", "net11.0")
PROTOCOLS = ("311", "500")
COMPOSE = ("docker", "compose", "-f", str(ROOT / "testing/sparkplug-tck/compose.yaml"))

# Number of distinct scenario IDs reported by the v3.0.0 extension.
SCENARIOS = {
    "host": {
        "SessionEstablishment": 41,
        "SessionTermination": 9,  # The run also checks that the host published its birth.
        "EdgeSessionTermination": 6,
        "MessageOrdering": 4,
        "SendCommand": 27,
        "MultipleBroker": 3,
    },
    "edge": {
        "SessionEstablishment": 71,
        "SessionTermination": 15,
        "SendData": 30,
        "SendComplexData": 38,
        "ReceiveCommand": 4,
        "PrimaryHost": 8,
        "MultipleBroker": 4,
    },
}
RESULT = re.compile(r"^(Monitor:)?([a-z][\w-]+): (PASS|FAIL|NOT EXECUTED|MAYBE)\b", re.MULTILINE)
LOG_NAME = re.compile(r"^TCK result file: (SparkplugTCK-[\w-]+\.log)$", re.MULTILINE)
PROTOCOL_ONLY = {
    "message-flow-phid-sparkplug-clean-session-311": "311",
    "message-flow-phid-sparkplug-clean-session-50": "500",
    "payloads-ndeath-will-message-publisher-disconnect-mqtt311": "311",
    "payloads-ndeath-will-message-publisher-disconnect-mqtt50": "500",
    "principles-persistence-clean-session-311": "311",
    "principles-persistence-clean-session-50": "500",
}


def run_case(dotnet, framework, directory, arguments, fixture_retries=2):
    executable = RUNNER / "bin" / "Release" / framework / "Runner.dll"
    process = subprocess.run(
        [dotnet, str(executable), *arguments], cwd=ROOT,
        text=True, encoding="utf-8", errors="replace", capture_output=True, timeout=75,
    )
    log_file = LOG_NAME.search(process.stdout)
    if not log_file:
        raise RuntimeError(f"{framework} {' '.join(arguments)} produced no log.\n{process.stdout}{process.stderr}")

    name = log_file[1]
    source = OUTPUT / "log" / name
    text = source.read_text(encoding="utf-8") if source.exists() else ""
    if process.returncode == 3 and fixture_retries and fixture_host_stalled(name):
        timeouts = directory / "fixture-timeouts"
        timeouts.mkdir(exist_ok=True)
        (timeouts / name).write_text(text or process.stdout + process.stderr, encoding="utf-8")
        print("The TCK's simulated host stalled during CONNECT. Restarting the local fixture.", flush=True)
        subprocess.run([*COMPOSE, "restart", "broker"], cwd=ROOT, check=True, timeout=45)
        subprocess.run([*COMPOSE, "up", "-d", "--no-build", "--wait", "--wait-timeout", "35"],
                       cwd=ROOT, check=True, timeout=60)
        return run_case(dotnet, framework, directory, arguments, fixture_retries - 1)

    (directory / name).write_text(text, encoding="utf-8")
    if process.returncode:
        raise RuntimeError(f"{framework} {' '.join(arguments)} failed.\n{process.stdout}{process.stderr}")
    if text.count("Summary Test Results for") != 1 or not re.search(r"^OVERALL: PASS(?: but INCOMPLETE)?;$", text, re.MULTILINE):
        raise RuntimeError(f"{name} does not contain one passing summary.")
    time.sleep(0.25)  # Let the extension finish its test-completion callbacks before starting the next case.
    return text


def fixture_host_stalled(log_name):
    broker_log = OUTPUT / "log" / "hivemq.log"
    if not broker_log.exists():
        return False
    recent = broker_log.read_text(encoding="utf-8", errors="replace").rsplit(log_name, 1)[-1]
    host = f"tck-{log_name.rsplit('-tck-', 1)[-1][:-4]}-host"
    return (f'Creating new host "{host}"' in recent
            and f"Host {host} successfully created" not in recent)


def verify_scenario(dotnet, framework, directory, role, scenario, expected):
    # A protocol-specific or shutdown-specific requirement may be inapplicable in
    # one run. It must pass in the same scenario's complementary run.
    results = {protocol: {} for protocol in PROTOCOLS}
    for protocol in PROTOCOLS:
        modes = ((), ("without-disconnect",)) if (role, scenario) == ("host", "SessionTermination") else ((),)
        for mode in modes:
            arguments = (role, scenario, protocol, *mode)
            print(f"{framework} {' '.join(arguments)}", flush=True)
            report = run_case(dotnet, framework, directory, arguments)
            checks = RESULT.findall(report)
            if not checks:
                raise RuntimeError(f"No assertion results for {' '.join(arguments)}.")
            for monitor, assertion, verdict in checks:
                if verdict in ("FAIL", "MAYBE"):
                    raise RuntimeError(f"{assertion}: {verdict} in {' '.join(arguments)}.")
                if not monitor:
                    results[protocol].setdefault(assertion, set()).add(verdict)

    ids = set().union(*(set(checks) for checks in results.values()))
    if len(ids) != expected or any(set(checks) != ids for checks in results.values()):
        raise RuntimeError(f"{role} {scenario} produced a different assertion list from the TCK declaration ({expected}).")

    incomplete = [
        assertion for assertion in sorted(ids)
        if any(
            "PASS" not in results[protocol][assertion]
            for protocol in PROTOCOLS if PROTOCOL_ONLY.get(assertion, protocol) == protocol
        )
    ]
    if incomplete:
        raise RuntimeError(f"{role} {scenario} did not pass: {', '.join(incomplete)}")
    return len(ids)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--dotnet", default="dotnet", help="Path to the .NET SDK executable.")
    parser.add_argument("--framework", choices=FRAMEWORKS, action="append", help="Runtime to test. Defaults to both.")
    args = parser.parse_args()
    run_id = datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%S.%fZ")
    output = OUTPUT / "runs" / run_id
    output.mkdir(parents=True)
    print(f"Reports: {output}", flush=True)
    subprocess.run([args.dotnet, "build", str(RUNNER / "Runner.csproj"), "-c", "Release"], cwd=ROOT, check=True)

    for framework in args.framework or FRAMEWORKS:
        directory = output / framework
        directory.mkdir()
        totals = Counter()
        lines = []
        for role, scenarios in SCENARIOS.items():
            for scenario, count in scenarios.items():
                passed = verify_scenario(args.dotnet, framework, directory, role, scenario, count)
                totals[role] += passed
                lines.append(f"{role} {scenario}: {passed}/{count}")
        lines.extend(f"{role}: {count}/{count} (100%)" for role, count in totals.items())
        (directory / "scenario-summary.txt").write_text("\n".join(lines) + "\n", encoding="utf-8")
        print("\n".join(lines), flush=True)


if __name__ == "__main__":
    main()
