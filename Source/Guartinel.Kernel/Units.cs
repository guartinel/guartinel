using System ;
using System.Collections.Generic ;
using System.Text ;

namespace Guartinel.Kernel {
   public enum FileSizeUnit {
      Byte,
      kB,
      MB,
      GB,
      TB
   }

   public enum TimeUnit {
      Second,
      Minute,
      Hour,
      Day
   }

   public static class UnitsEx {
      public static class Constants {
         public static readonly List<string> TRUE_VALUES = new List<string> { "true", "yes", "y" };
         public static readonly List<string> FALSE_VALUES = new List<string> { "false", "no", "n" };

         public const string INVARIANT_DATE_TIME_FORMAT = @"yyyy/MM/dd HH:mm:ss.fff";

         public const long BYTES_IN_KB = 1024;
         public const long BYTES_IN_MB = 1024 * 1024;
         public const long BYTES_IN_GB = 1024 * BYTES_IN_MB;
         public const long BYTES_IN_TB = 1024 * BYTES_IN_GB;

         public const int SIZE_ROUNDING = 2;

         public const int SECONDS_IN_MINUTE = 60;
         public const int SECONDS_IN_HOUR = SECONDS_IN_MINUTE * 60;
         public const int SECONDS_IN_DAY = SECONDS_IN_HOUR * 24;

         public const int TIME_ROUNDING = 2;
      }

      public static long ConvertSizeToBytes (long size,
                                             FileSizeUnit unit) {
         switch (unit) {
            case FileSizeUnit.kB:
               return size * Constants.BYTES_IN_KB;

            case FileSizeUnit.MB:
               return size * Constants.BYTES_IN_MB;

            case FileSizeUnit.GB:
               return size * Constants.BYTES_IN_GB;

            case FileSizeUnit.TB:
               return size * Constants.BYTES_IN_TB;
         }

         return size;
      }

      public static double ConvertSizeToUnit (long sizeInBytes,
                                              FileSizeUnit fileSizeUnit) {
         switch (fileSizeUnit) {
            case FileSizeUnit.kB:
               return Math.Round(1.0 * sizeInBytes / Constants.BYTES_IN_KB, Constants.SIZE_ROUNDING);

            case FileSizeUnit.MB:
               return Math.Round(1.0 * sizeInBytes / Constants.BYTES_IN_MB, Constants.SIZE_ROUNDING);

            case FileSizeUnit.GB:
               return Math.Round(1.0 * sizeInBytes / Constants.BYTES_IN_GB, Constants.SIZE_ROUNDING);

            case FileSizeUnit.TB:
               return Math.Round(1.0 * sizeInBytes / Constants.BYTES_IN_TB, Constants.SIZE_ROUNDING);
         }
         return sizeInBytes;
      }

      public static int ConvertTimeToSeconds (int interval,
                                              TimeUnit timeUnit) {
         switch (timeUnit) {
            case TimeUnit.Minute:
               return interval * Constants.SECONDS_IN_MINUTE;

            case TimeUnit.Hour:
               return interval * Constants.SECONDS_IN_HOUR;

            case TimeUnit.Day:
               return interval * Constants.SECONDS_IN_DAY;
         }

         return interval;
      }

      public static double ConvertTimeToUnit (int intervalInSeconds,
                                              TimeUnit timeUnit) {
         switch (timeUnit) {
            case TimeUnit.Minute:
               return Math.Round(1.0 * intervalInSeconds / Constants.SECONDS_IN_MINUTE, Constants.TIME_ROUNDING);

            case TimeUnit.Hour:
               return Math.Round(1.0 * intervalInSeconds / Constants.SECONDS_IN_HOUR, Constants.TIME_ROUNDING);

            case TimeUnit.Day:
               return Math.Round(1.0 * intervalInSeconds / Constants.SECONDS_IN_DAY, Constants.TIME_ROUNDING);
         }

         return intervalInSeconds;
      }
   }
}
