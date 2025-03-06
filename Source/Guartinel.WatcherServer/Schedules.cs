using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.Kernel.Utility ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.WatcherServer {
   /// <summary>
   /// Schedule base object.
   /// </summary>
   public abstract class Schedule {
      public static class Constants {
         public static class Parameters {
            public const string TYPE = "type" ;
            public const string IS_ENABLED = "is_enabled" ;
            public const string INTERVAL_IN_SECONDS = "interval_in_seconds";
         }
      }

      protected bool _isEnabled ;
      public bool IsEnabled {
         get => _isEnabled ;
         set => _isEnabled = value ;
      }

      protected TimeSpan _interval = TimeSpan.FromSeconds (0) ;
      public TimeSpan Interval {
         get => _interval ;
         set => _interval = value ;
      }


      public static Schedule CreateSchedule (ConfigurationData configuration) {
         Schedule result = IoC.Use.Multi.GetInstance<Schedule>(configuration [Constants.Parameters.TYPE]) ;

         var intervalInSeconds = configuration.AsIntegerNull(Constants.Parameters.INTERVAL_IN_SECONDS);

         result.Configure (configuration.AsBoolean (Constants.Parameters.IS_ENABLED),
                           intervalInSeconds == null ? TimeSpan.FromSeconds (0): TimeSpan.FromSeconds(intervalInSeconds.Value)) ;
         result.SpecificConfigure (configuration) ;

         return result ;
      }      

      protected void Configure (bool isEnabled,
                                TimeSpan interval) {
         _isEnabled = isEnabled ;
         _interval = interval ;
      }

      protected abstract void SpecificConfigure (ConfigurationData configuration) ;

      public ConfigurationData ToConfigurationData() {
         ConfigurationData result = new ConfigurationData() ;
         var data = result.AsJObject ;
         data [Constants.Parameters.IS_ENABLED] = _isEnabled ;
         data [Constants.Parameters.INTERVAL_IN_SECONDS] = (int) _interval.TotalSeconds ;

         AddSpecificConfiguration (result) ;
         return result ;
      }

      protected abstract void AddSpecificConfiguration (ConfigurationData configuration);

      public bool IsScheduled (DateTime dateTime) {
         if (!IsEnabled) return false ;

         return IsScheduled1 (dateTime) ;
      }

      protected abstract bool IsScheduled1 (DateTime dateTime) ;

      public DateTime? FindScheduleEnd() {
         if (!IsScheduled (DateTime.UtcNow)) return null ;

         return FindScheduleEnd1() ;
      }

      protected abstract DateTime FindScheduleEnd1();
   }

   public class ScheduleOnce : Schedule {
      public new static class Constants {
         public static readonly List<string> CREATOR_IDENTIFIERS = new List<string> { "once" };

         public static class Parameters {
            public const string START = "date_time" ;
         }
      }

      private DateTime? _start = null ;

      public DateTime? Start {
         get => _start ;
         set => _start = value ;
      }

      protected override void SpecificConfigure (ConfigurationData configuration) {
         _start = configuration.AsDateTimeNull (Constants.Parameters.START) ;
      }

      protected override void AddSpecificConfiguration (ConfigurationData configuration) {
         configuration.AsJObject [Schedule.Constants.Parameters.TYPE] = Constants.CREATOR_IDENTIFIERS [0] ;
         configuration.AsJObject [Constants.Parameters.START] = _start ;
      }

      protected override bool IsScheduled1 (DateTime dateTime) {
         if (_start == null) return false ;

         return (dateTime >= _start.Value) &&
                (dateTime <= _start.Value.Add (_interval)) ;
      }

      protected override DateTime FindScheduleEnd1() {
         if (_start == null) return DateTime.UtcNow ;
         
         return _start.Value.Add (_interval) ;
      }
   }

   public class ScheduleDaily : Schedule {
      public new static class Constants {
         public static readonly List<string> CREATOR_IDENTIFIERS = new List<string> { "daily" };

         public static class Parameters {
            public const string START_TIME = "time" ;
         }
      }

      private TimeSpan? _startTime = null ;

      public TimeSpan? StartTime {
         get => _startTime;
         set => _startTime = value ;
      }

      protected override void SpecificConfigure (ConfigurationData configuration) {
         _startTime = configuration.AsTimeSpanNull (Constants.Parameters.START_TIME) ;
      }

      protected override void AddSpecificConfiguration (ConfigurationData configuration) {
         configuration.AsJObject [Schedule.Constants.Parameters.TYPE] = Constants.CREATOR_IDENTIFIERS [0] ;

         configuration.AsJObject [Constants.Parameters.START_TIME] = _startTime ;
      }

      protected override bool IsScheduled1 (DateTime dateTime) {
         if (_startTime == null) return false ;

         return (dateTime >= dateTime.Date.Add (_startTime.Value)) &&
                (dateTime <= dateTime.Date.Add (_startTime.Value).Add (_interval)) ;
      }

      protected override DateTime FindScheduleEnd1 () {
         if (_startTime == null) return DateTime.UtcNow ;

         return DateTime.UtcNow.Date.Add (_startTime.Value).Add (_interval) ;
      }
   }

   public class ScheduleWeekly : Schedule {
      public new static class Constants {
         public static readonly List<string> CREATOR_IDENTIFIERS = new List<string> { "weekly" };

         public static class Parameters {
            public const string START_TIME = "time";
            public const string DAYS = "days";
         }
      }

      private TimeSpan? _startTime = null ;
      public TimeSpan? StartTime {
         get => _startTime;
         set => _startTime = value;
      }

      private readonly List<DayOfWeek> _daysOfWeek = new List<DayOfWeek>();
      public List<DayOfWeek> DaysOfWeek => _daysOfWeek ;

      protected override void SpecificConfigure (ConfigurationData configuration) {
         configuration.AsJObject[Schedule.Constants.Parameters.TYPE] = Constants.CREATOR_IDENTIFIERS[0];

         var startTime = configuration.AsTimeSpanNull (Constants.Parameters.START_TIME) ;
         _startTime = startTime ;

         _daysOfWeek.Clear();
         var days = configuration.GetChild (Constants.Parameters.DAYS) ;

         foreach (var dayOfWeek in EnumEx.GetValues<DayOfWeek>()) {
            if (days.AsBoolean (dayOfWeek.ToString().ToLowerInvariant())) {
               _daysOfWeek.Add (dayOfWeek) ;
            }
         }
         // "days": {
         //   "sunday": true,
         //   "friday": true,
         //   "wednesday": true,
         //   "monday": true
         // }
      }

      protected override void AddSpecificConfiguration (ConfigurationData configuration) {
         configuration.AsJObject[Schedule.Constants.Parameters.TYPE] = Constants.CREATOR_IDENTIFIERS[0];

         configuration.AsJObject [Constants.Parameters.START_TIME] = _startTime ;
         JObject days = new JObject() ;

         foreach (var dayOfWeek in _daysOfWeek) {
            days [dayOfWeek.ToString().ToLowerInvariant()] = true ;
         }
         configuration.AsJObject [Constants.Parameters.DAYS] = days ;
      }

      protected override bool IsScheduled1 (DateTime dateTime) {
         if (_startTime == null) return false ;

         return (_daysOfWeek.Contains (dateTime.DayOfWeek)) &&
                (dateTime >= dateTime.Date.Add (_startTime.Value)) &&
                (dateTime <= dateTime.Date.Add (_startTime.Value).Add (_interval)) ;
      }

      protected override DateTime FindScheduleEnd1() {         
         if (!_daysOfWeek.Any()) return DateTime.MaxValue ;
         if (_startTime == null) return DateTime.UtcNow ;

         var date = DateTime.UtcNow.Date ;
         while (true) {
            DayOfWeek dayOfWeek = date.DayOfWeek ;
            if (!_daysOfWeek.Contains (date.DayOfWeek)) {
               date = date.AddDays (1) ;
               continue ;
            }

            return date.Add(_startTime.Value).Add(_interval) ;
         }
      }
   }

   /// <summary>
   /// Contains schedules of possible different types.
   /// </summary>

   public class Schedules {
      protected List<Schedule> _schedules = new List<Schedule>() ;

      public static class Constants {
         public static class Parameters {
            public const string SCHEDULES = "schedules" ;
         }
      }

      public static void Register () {
         IoC.Use.Multi.Register<Schedule, ScheduleOnce>(ScheduleOnce.Constants.CREATOR_IDENTIFIERS);
         IoC.Use.Multi.Register<Schedule, ScheduleDaily>(ScheduleDaily.Constants.CREATOR_IDENTIFIERS);
         IoC.Use.Multi.Register<Schedule, ScheduleWeekly>(ScheduleWeekly.Constants.CREATOR_IDENTIFIERS);

         //Factory.Use.RegisterCreator(ComputerStatusChecker.GetCreator());
      }

      public Schedules Configure (ConfigurationData configurationData) {
         JArray schedules = (JArray) configurationData.AsJObject [Constants.Parameters.SCHEDULES] ;
         if (schedules == null) return this ;

         foreach (var token in schedules) {
            var schedule = (JObject) token;
            if (schedule == null) continue ;

            _schedules.Add (Schedule.CreateSchedule (new ConfigurationData (schedule))) ;
         }

         return this ;
      }
      public ConfigurationData ToConfigurationData() {
         ConfigurationData result = new ConfigurationData() ;
         JArray schedules = new JArray() ;
         
         foreach (var schedule in _schedules) {
            schedules.Add (schedule.ToConfigurationData().Data.AsJObject) ; 
         }

         result.Data.AsJObject [Constants.Parameters.SCHEDULES] = schedules ;

         return result ;
      }

      public bool IsScheduled (DateTime dateTime) {
         return _schedules.Any (x => x.IsScheduled (dateTime)) ;
      }

      public bool IsScheduled() {
         return IsScheduled (DateTime.Now) ;
      }

      public DateTime? FindScheduleEnd() {
         if (!_schedules.Any()) return null ;

         return _schedules.Select (x1 => x1.FindScheduleEnd()).OrderBy (x2 => x2).LastOrDefault() ;
      }
   }
}
