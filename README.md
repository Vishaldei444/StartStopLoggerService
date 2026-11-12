# Windows Logger Services

A C# project that builds one executable for two Windows services. Each service can be started and stopped independently.

## Services

| Service | What it does | Log file |
| --- | --- | --- |
| `StartStopLoggerService` | Records the date and time when the service starts and stops. | `start-stop.log` |
| `TimeLoggerService` | Records the current time at startup and every 10 seconds while the service is running. | `time.log` |

Both log files are created in `%ProgramData%\LoggerServices`.

## Requirements

- Windows
- Visual Studio 2022 with the **.NET desktop development** workload
- .NET Framework 4.8 targeting pack
- Administrator access to register and control the services

## Build and Install

1. Open `StartStopLoggerService.sln` in Visual Studio.
2. Select **Release** and choose **Build → Build Solution**.
3. Copy the full path to `StartStopLoggerService.exe` from the project's `bin\Release` folder.
4. Open **Command Prompt as administrator**. Replace the example path below with your executable's full path, then run the commands.

```cmd
sc.exe create StartStopLoggerService binPath= "\"C:\Path\To\StartStopLoggerService.exe\"" start= demand
sc.exe create TimeLoggerService binPath= "\"C:\Path\To\StartStopLoggerService.exe\" --time-logger" start= demand
```

The `--time-logger` argument selects `TimeLoggerService` when Windows launches the executable.

## Run and Verify

Start `StartStopLoggerService`, wait for it to start, then stop it and check the log.

```cmd
sc.exe start StartStopLoggerService
sc.exe stop StartStopLoggerService
type "%ProgramData%\LoggerServices\start-stop.log"
```

The log should show `Service started` and `Service stopped`, each with a timestamp.

Start `TimeLoggerService`, wait about 20 seconds, then check the log.

```cmd
sc.exe start TimeLoggerService
type "%ProgramData%\LoggerServices\time.log"
```

The log should show `Current time` entries about 10 seconds apart. Stop the service with `sc.exe stop TimeLoggerService` and confirm that no new entries appear after the service reaches `STOPPED`.

## Output Screenshot

![Command Prompt showing the Start/Stop Logger and Time Logger output](screenshots/logger-services-output.png)
