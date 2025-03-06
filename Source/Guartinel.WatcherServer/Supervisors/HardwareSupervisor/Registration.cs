using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Packages ;
using Guartinel.WatcherServer.Supervisors.HardwareSupervisor.HardwareSensors ;
using HardwareTypes = Guartinel.Communication.Supervisors.HardwareSupervisor.Strings.Hardwares ;

namespace Guartinel.WatcherServer.Supervisors.HardwareSupervisor {
   public static class Registration {
      public static void Register() {
         IoC.Use.Multi.Register<Package, HardwareSupervisorPackage> (HardwareSupervisorPackage.Constants.CREATOR_IDENTIFIERS) ;

         IoC.Use.Multi.Register<HardwareSensor, CurrentChecker30A> (HardwareTypes.CurrentLevel.Max30A.FULL_TYPE,
                                                                               typeof(CurrentChecker30A).Name) ;
         IoC.Use.Multi.Register<HardwareSensor, CurrentChecker100A> (HardwareTypes.CurrentLevel.Max100A.FULL_TYPE,
                                                                                typeof(CurrentChecker100A).Name) ;

         IoC.Use.Multi.Register<HardwareSensor, VoltageSensor230V> (HardwareTypes.VoltageLevel.Max230V.OneChannel.FULL_TYPE,
                                                                                typeof(VoltageSensor230V).Name) ;
         IoC.Use.Multi.Register<HardwareSensor, VoltageChecker230V3Channel> (HardwareTypes.VoltageLevel.Max230V.ThreeChannel.FULL_TYPE,
                                                                                        typeof(VoltageChecker230V3Channel).Name) ;

         IoC.Use.Multi.Register<HardwareSensor, TemperatureSensorDht11> (HardwareTypes.Temperature.DHT11.FULL_TYPE,
                                                                                     typeof(TemperatureSensorDht11).Name) ;
         IoC.Use.Multi.Register<HardwareSensor, TemperatureSensorDht22> (HardwareTypes.Temperature.DHT22.FULL_TYPE,
                                                                                     typeof(TemperatureSensorDht22).Name) ;
         IoC.Use.Multi.Register<HardwareSensor, TemperatureSensorDs8B20>(HardwareTypes.Temperature.DS18B20.FULL_TYPE,
                                                                          typeof(TemperatureSensorDs8B20).Name);         

         IoC.Use.Multi.Register<HardwareSensor, GasMq135Sensor> (HardwareTypes.Gas.MQ135.FULL_TYPE,
                                                                   typeof(GasMq135Sensor).Name) ;
         IoC.Use.Multi.Register<HardwareSensor, LiquidSensor> (HardwareTypes.Water.Presence.FULL_TYPE,
                                                                typeof(LiquidSensor).Name) ;
      }

      public static void Unregister() {
         //Factory.Use.UnregisterCreators<ApplicationSupervisorPackage>() ;
         //Factory.Use.UnregisterCreators<ApplicationInstanceDataChecker>() ;
      }
   }
}
