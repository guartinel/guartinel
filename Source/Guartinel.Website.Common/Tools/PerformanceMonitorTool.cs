using Guartinel.Kernel.Utility ;
using Guartinel.Kernel.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Guartinel.Website.Common.Tools {

   public static class PerformanceMonitorTool {

      private static readonly PerformanceCounter _cpuCounter = new PerformanceCounter();
      private static readonly PerformanceCounter _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
      private static readonly DriveInfo[] _drives = DriveInfo.GetDrives();


      public static float GetCPUUsage () {
         float cpuUsage = 0.0f;
         try {
            _cpuCounter.CategoryName = "Processor";
            _cpuCounter.CounterName = "% Processor Time";
            _cpuCounter.InstanceName = "_Total";
            cpuUsage = _cpuCounter.NextValue();
            } catch ( Exception e ) {
            Logger.Error($"Cannot get CPU usage. {e.GetAllMessages()}");
            }
         return cpuUsage;
         }


      public static float GetMemoryUsage () {
         float memoryUsage = 0;
         try {
            float availableMemory = _ramCounter.NextValue();
            ulong installedMemory = 1;
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if ( GlobalMemoryStatusEx(memStatus) ) {
               installedMemory = memStatus.ullTotalPhys / 1024 / 1024;
               }
            memoryUsage = ( installedMemory - availableMemory ) / installedMemory * 100;
            } catch ( Exception e ) {
            Logger.Error($"Cannot get memory usage. {e.GetAllMessages()}");
            }
         return memoryUsage;
         }


      public static float GetStorageUsage () {
         float driveUsage = 0;
         if ( !_drives[0].IsReady ) {
            return driveUsage;
            }
         try {
            if ( _drives.Length != 0 )
               driveUsage = ( _drives[0].TotalSize - _drives[0].TotalFreeSpace )
                            / (float) _drives[0].TotalSize * 100f;
            } catch ( Exception e ) {
            Logger.Error($"Cannot get storage usage. {e.GetAllMessages()}");

            }
         return driveUsage;
         }


      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
      private class MEMORYSTATUSEX {
         public uint dwLength;
         public uint dwMemoryLoad;
         public ulong ullTotalPhys;
         public ulong ullAvailPhys;
         public ulong ullTotalPageFile;
         public ulong ullAvailPageFile;
         public ulong ullTotalVirtual;
         public ulong ullAvailVirtual;
         public ulong ullAvailExtendedVirtual;

         public MEMORYSTATUSEX () {
            this.dwLength = (uint) Marshal.SizeOf(this);
            }
         }


      [return: MarshalAs(UnmanagedType.Bool)]
      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern bool GlobalMemoryStatusEx ([In, Out] MEMORYSTATUSEX lpBuffer);

      }
   }