using System;
using System.Linq;
using System.Text;

namespace Guartinel.Kernel {
   public class PerformanceInfo : IDisposable {
      //protected PerformanceCounter _cpuLoad = new PerformanceCounter ("Processor", "% Processor Time", "_Total") ;
      //protected PerformanceCounter _availableMemory = new PerformanceCounter ("Memory", "Available MBytes", "") ;
      //protected PerformanceCounter _availableDisk = new PerformanceCounter ("LogicalDisk", "Free Megabytes", "c:") ;

      // protected ulong _installedMemory ;

      public PerformanceInfo() {
         //_cpuLoad.NextValue() ;
         //_availableMemory.NextValue() ;
         //_availableDisk.NextValue() ;

         //WinAPIMemoryStatusEx memoryStatus = new WinAPIMemoryStatusEx() ;
         //if (GlobalMemoryStatusEx (memoryStatus)) {
         //   _installedMemory = memoryStatus.TotalPhysicalMemory ;
         //}
      }

      public void Dispose() {
         //_cpuLoad?.Dispose() ;
         //_availableMemory?.Dispose() ;
         //_availableDisk?.Dispose() ;
      }

      private double NormalizeValue (double value) {
         return value ;
      }

      public double GetCPULoad() {
         // return NormalizeValue (_cpuLoad.NextValue()) ;
         return NormalizeValue (12.0) ;
      }

      public double GetAvailableMemoryMBs() {
         // return NormalizeValue (_availableMemory.NextValue()) ;
         return NormalizeValue (13 * 1024) ;
      }

      public double GetAvailableMemoryPercents() {
         return NormalizeValue (GetAvailableMemoryMBs() / GetTotalMemoryMBs()) ;
      }

      public double GetTotalMemoryMBs() {
         // Convert MBs to GBs
         // return NormalizeValue (_installedMemory / 1024.0) ;
         return NormalizeValue (16 * 1024.0) ;
      }

      public double GetAvailableDiskGBs() {
         // Convert MBs to GBs
         // return NormalizeValue (_availableDisk.NextValue() / 1024.0) ;
         return NormalizeValue (2000) ;
      }

      //#region WinAPI performance info

      //[DllImport ("psapi.dll", SetLastError = true)]
      //[return: MarshalAs (UnmanagedType.Bool)]
      //public static extern bool GetPerformanceInfo ([Out] out WinAPIPerformanceInfo winAPIPerformanceInfo,
      //                                              [In] int size) ;

      //[StructLayout (LayoutKind.Sequential)]
      //public struct WinAPIPerformanceInfo {
      //   public int Size ;
      //   public IntPtr CommitTotal ;
      //   public IntPtr CommitLimit ;
      //   public IntPtr CommitPeak ;
      //   public IntPtr PhysicalTotal ;
      //   public IntPtr PhysicalAvailable ;
      //   public IntPtr SystemCache ;
      //   public IntPtr KernelTotal ;
      //   public IntPtr KernelPaged ;
      //   public IntPtr KernelNonPaged ;
      //   public IntPtr PageSize ;
      //   public int HandlesCount ;
      //   public int ProcessCount ;
      //   public int ThreadCount ;
      //}

      //public static Int64 GetPhysicalAvailableMemoryInMiB() {
      //   WinAPIPerformanceInfo pi = new WinAPIPerformanceInfo() ;
      //   if (GetPerformanceInfo (out pi, Marshal.SizeOf (pi))) {
      //      return Convert.ToInt64 ((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576)) ;
      //   } else {
      //      return -1 ;
      //   }

      //}

      //public static Int64 GetTotalMemoryInMiB() {
      //   WinAPIPerformanceInfo pi = new WinAPIPerformanceInfo() ;
      //   if (GetPerformanceInfo (out pi, Marshal.SizeOf (pi))) {
      //      return Convert.ToInt64 ((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576)) ;
      //   } else {
      //      return -1 ;
      //   }

      //}
      //#endregion

      //[StructLayout (LayoutKind.Sequential, CharSet = CharSet.Auto)]
      //private class WinAPIMemoryStatusEx {
      //   public uint Length ;
      //   public uint MemoryLoad ;
      //   public ulong TotalPhysicalMemory ;
      //   public ulong AvailablePhysicalMemory ;
      //   public ulong TotalPageFile ;
      //   public ulong AvailablePageFile ;
      //   public ulong TotalVirtual ;
      //   public ulong AvailableVirtual ;
      //   public ulong AvailableExtendedVirtual ;

      //   public WinAPIMemoryStatusEx () {
      //      Length = (uint) Marshal.SizeOf(typeof(WinAPIMemoryStatusEx));
      //   }
      //}

      //[return: MarshalAs (UnmanagedType.Bool)]
      //[DllImport ("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      //private static extern bool GlobalMemoryStatusEx ([In, Out] WinAPIMemoryStatusEx lpBuffer) ;
   }
}