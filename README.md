# Tray Monitor

<img src="https://raw.githubusercontent.com/strayge/tray-monitor/master/screenshots/readme_tray.png" width=70%>

See your system's load in the Windows tray.

This is a fork of [strayge/tray-monitor](https://github.com/strayge/tray-monitor). See [What's different](#whats-different-in-this-fork).

Separate icons for:
- battery status (percentage, remaining time and charging status)
- CPU (including color marking for throttling)
- GPU
- memory
- network I/O
- disk I/O

A single click on the Memory icon opens a balloon with the top processes.
<img src="https://raw.githubusercontent.com/strayge/tray-monitor/master/screenshots/readme_balloon.png" width=50%>

A double click on the CPU or Memory icon opens Task Manager; on the Disk icon it opens Resource Monitor.

All colors, update intervals and some other parameters can be changed in the settings window (right click on an icon).
<img src="https://raw.githubusercontent.com/strayge/tray-monitor/master/screenshots/readme_settings.png" width=70%>

## What's different in this fork

- **Battery:** at 100% the icon shows **"Fu"** instead of "100", in its own color (default `#116417`, changeable in settings as *Foreground (Full)*).
- **Installer:** pick icons from a checklist with an "All icons" box; running icons are closed before updating; uninstall removes autostart shortcuts and offers to delete saved settings.
- **Double-click:** the CPU and Memory icons open Task Manager, the Disk icon opens Resource Monitor (this was disabled in the original).

## Installing

- [Download the latest release](https://github.com/chris-v8/tray-monitor/releases)
- Run `TrayMonitor-Setup-<version>.exe` and pick the icons you want (no admin rights needed; installs to `%LOCALAPPDATA%\Programs\TrayMonitor`)
- Or use the portable zip and run the `.exe` files directly
- Autostart can be turned on in the installer or later from an icon's right-click menu

<img src="https://raw.githubusercontent.com/strayge/tray-monitor/master/screenshots/readme_menu.png" width=20%>

The installer upgrades an existing TrayMonitor installation, including the original one. Settings are kept.

## Building

Requirements:
- Visual Studio 2022/2026 or Build Tools with the **.NET desktop development** workload (.NET Framework 4.8)
- [Inno Setup 6](https://jrsoftware.org/isinfo.php) for the installer

Build all icons, the installer and the portable zip:

```bat
setup\build_all.bat
```

Output goes to `bin\Release\`.

## License

MIT, see [LICENSE](LICENSE). Original work by [strayge](https://github.com/strayge/tray-monitor).
