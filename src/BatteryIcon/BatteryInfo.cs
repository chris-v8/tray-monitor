using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace BatteryIcon
{
    // Battery readings straight from Windows. Capacities are in mWh, rates in mW.
    static class BatteryInfo
    {
        // ---- live state of all batteries combined (one cheap call) ----

        public struct State
        {
            public bool PowerOnline;
            // > 0 while charging, < 0 while discharging, null when Windows doesn't know
            public int? RateMw;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SYSTEM_BATTERY_STATE
        {
            public byte AcOnLine;
            public byte BatteryPresent;
            public byte Charging;
            public byte Discharging;
            public byte Spare1;
            public byte Spare2;
            public byte Spare3;
            public byte Tag;
            public uint MaxCapacity;
            public uint RemainingCapacity;
            public int Rate;
            public uint EstimatedTime;
            public uint DefaultAlert1;
            public uint DefaultAlert2;
        }

        const int SystemBatteryState = 5;
        const int BATTERY_UNKNOWN_RATE = unchecked((int)0x80000000);

        [DllImport("powrprof.dll")]
        static extern uint CallNtPowerInformation(int informationLevel, IntPtr inputBuffer, int inputBufferLength,
            out SYSTEM_BATTERY_STATE outputBuffer, int outputBufferLength);

        public static bool TryGetState(out State state)
        {
            state = new State();
            SYSTEM_BATTERY_STATE raw;
            if (CallNtPowerInformation(SystemBatteryState, IntPtr.Zero, 0, out raw, Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE))) != 0)
            {
                return false;
            }
            state.PowerOnline = raw.AcOnLine != 0;
            if (raw.BatteryPresent != 0 && raw.Rate != BATTERY_UNKNOWN_RATE)
            {
                state.RateMw = raw.Rate;
            }
            return true;
        }

        // ---- capacities, asked from each battery's driver ----

        public struct Capacity
        {
            public long DesignMwh;
            public long FullChargeMwh;
        }

        static readonly Guid GUID_DEVCLASS_BATTERY = new Guid("72631E54-78A4-11D0-BCF7-00AA00B7B32A");
        const int DIGCF_PRESENT = 0x2;
        const int DIGCF_DEVICEINTERFACE = 0x10;
        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x1;
        const uint FILE_SHARE_WRITE = 0x2;
        const uint OPEN_EXISTING = 3;
        const uint IOCTL_BATTERY_QUERY_TAG = 0x294040;
        const uint IOCTL_BATTERY_QUERY_INFORMATION = 0x294044;
        const int BatteryInformationLevel = 0;
        const int BatteryDeviceNameLevel = 4;
        const int BatteryManufactureNameLevel = 6;
        // capacities are reported in percent instead of mWh
        const uint BATTERY_CAPACITY_RELATIVE = 0x40000000;

        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct BATTERY_QUERY_INFORMATION
        {
            public uint BatteryTag;
            public int InformationLevel;
            public int AtRate;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct BATTERY_INFORMATION
        {
            public uint Capabilities;
            public byte Technology;
            public byte Reserved1;
            public byte Reserved2;
            public byte Reserved3;
            public uint Chemistry;
            public uint DesignedCapacity;
            public uint FullChargedCapacity;
            public uint DefaultAlert1;
            public uint DefaultAlert2;
            public uint CriticalBias;
            public uint CycleCount;
        }

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, int flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid,
            int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDeviceInterfaceDetailW")]
        static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, out int requiredSize, IntPtr deviceInfoData);

        [DllImport("setupapi.dll")]
        static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFile(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes,
            uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(SafeFileHandle device, uint ioControlCode, ref uint inBuffer, int inBufferSize,
            out uint outBuffer, int outBufferSize, out int bytesReturned, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(SafeFileHandle device, uint ioControlCode, ref BATTERY_QUERY_INFORMATION inBuffer, int inBufferSize,
            out BATTERY_INFORMATION outBuffer, int outBufferSize, out int bytesReturned, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(SafeFileHandle device, uint ioControlCode, ref BATTERY_QUERY_INFORMATION inBuffer, int inBufferSize,
            [Out] byte[] outBuffer, int outBufferSize, out int bytesReturned, IntPtr overlapped);

        // Sums all batteries. Returns false when no battery reports its capacity in mWh.
        public static bool TryGetCapacity(out Capacity capacity)
        {
            var total = new Capacity();
            ForEachBattery((battery, tag) =>
            {
                var query = new BATTERY_QUERY_INFORMATION { BatteryTag = tag, InformationLevel = BatteryInformationLevel };
                BATTERY_INFORMATION info;
                int returned;
                if (DeviceIoControl(battery, IOCTL_BATTERY_QUERY_INFORMATION,
                        ref query, Marshal.SizeOf(typeof(BATTERY_QUERY_INFORMATION)),
                        out info, Marshal.SizeOf(typeof(BATTERY_INFORMATION)), out returned, IntPtr.Zero)
                    && (info.Capabilities & BATTERY_CAPACITY_RELATIVE) == 0)
                {
                    total.DesignMwh += info.DesignedCapacity;
                    total.FullChargeMwh += info.FullChargedCapacity;
                }
            });
            capacity = total;
            return capacity.DesignMwh > 0;
        }

        // Manufacturer and model name as the battery reports them, e.g. "SMP 5B10W51867".
        // Several batteries are joined with ", ". Null when none reports a name.
        public static string GetModel()
        {
            var names = new List<string>();
            ForEachBattery((battery, tag) =>
            {
                string name = String.Join(" ", new[] {
                    QueryString(battery, tag, BatteryManufactureNameLevel),
                    QueryString(battery, tag, BatteryDeviceNameLevel),
                }.Where(part => !String.IsNullOrEmpty(part)));
                if (name != "")
                {
                    names.Add(name);
                }
            });
            return names.Count > 0 ? String.Join(", ", names) : null;
        }

        private static string QueryString(SafeFileHandle battery, uint tag, int informationLevel)
        {
            var query = new BATTERY_QUERY_INFORMATION { BatteryTag = tag, InformationLevel = informationLevel };
            byte[] buffer = new byte[512];
            int returned;
            if (!DeviceIoControl(battery, IOCTL_BATTERY_QUERY_INFORMATION,
                    ref query, Marshal.SizeOf(typeof(BATTERY_QUERY_INFORMATION)),
                    buffer, buffer.Length, out returned, IntPtr.Zero))
            {
                return null;
            }
            return Encoding.Unicode.GetString(buffer, 0, returned).Split('\0')[0].Trim();
        }

        // Opens every battery the system knows about and passes it with its tag
        // (the id the battery driver wants in each query).
        private static void ForEachBattery(Action<SafeFileHandle, uint> query)
        {
            Guid batteryClass = GUID_DEVCLASS_BATTERY;
            IntPtr deviceSet = SetupDiGetClassDevs(ref batteryClass, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            if (deviceSet == new IntPtr(-1))
            {
                return;
            }
            try
            {
                var interfaceData = new SP_DEVICE_INTERFACE_DATA();
                interfaceData.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));
                for (int i = 0; SetupDiEnumDeviceInterfaces(deviceSet, IntPtr.Zero, ref batteryClass, i, ref interfaceData); i++)
                {
                    string path = GetDevicePath(deviceSet, ref interfaceData);
                    if (path == null)
                    {
                        continue;
                    }
                    using (SafeFileHandle battery = CreateFile(path, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE,
                        IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero))
                    {
                        if (battery.IsInvalid)
                        {
                            continue;
                        }
                        uint wait = 0;
                        uint tag;
                        int returned;
                        if (DeviceIoControl(battery, IOCTL_BATTERY_QUERY_TAG, ref wait, sizeof(uint), out tag, sizeof(uint), out returned, IntPtr.Zero) && tag != 0)
                        {
                            query(battery, tag);
                        }
                    }
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(deviceSet);
            }
        }

        private static string GetDevicePath(IntPtr deviceSet, ref SP_DEVICE_INTERFACE_DATA interfaceData)
        {
            int size;
            SetupDiGetDeviceInterfaceDetail(deviceSet, ref interfaceData, IntPtr.Zero, 0, out size, IntPtr.Zero);
            if (size <= 0)
            {
                return null;
            }
            IntPtr detail = Marshal.AllocHGlobal(size);
            try
            {
                // SP_DEVICE_INTERFACE_DETAIL_DATA: cbSize, then the path; cbSize is 8 on 64-bit, 6 on 32-bit
                Marshal.WriteInt32(detail, IntPtr.Size == 8 ? 8 : 6);
                if (!SetupDiGetDeviceInterfaceDetail(deviceSet, ref interfaceData, detail, size, out size, IntPtr.Zero))
                {
                    return null;
                }
                return Marshal.PtrToStringUni(new IntPtr(detail.ToInt64() + 4));
            }
            finally
            {
                Marshal.FreeHGlobal(detail);
            }
        }
    }
}
