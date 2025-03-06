import { LOG } from "./LoggerFactory";
class ExecutionMeterFactory {
   isEnabled: boolean;
   constructor(isEnabled: boolean) {
      LOG.info("ExecutionMeterFactory initialized with enabled status: " + isEnabled);
      this.isEnabled = isEnabled;
   }

   create(label: string): ExecutionMeter {
      if (!this.isEnabled) {
         return new NullMeter();
      }
      return new RealMeter(label);
   }
}

let factory: ExecutionMeterFactory;
export function configure(isEnabled: boolean) {
   factory = new ExecutionMeterFactory(isEnabled);
}

export function getExecutionMeter(label: string): ExecutionMeter {
   if (factory == null) {
      LOG.error("ExecutionMeterFactory.getExecutionMeter is called before configuration. Are configured it on init?");
      factory = new ExecutionMeterFactory(false);
   }
   return factory.create(label);
}

export interface ExecutionMeter {
   start();
   stop();
}
class NullMeter implements ExecutionMeter {
   start() { }
   stop() { }
}

class RealMeter implements ExecutionMeter {
   label: string;
   startTime: any;
   stopTime: number[];
   constructor(label: string) {
      this.label = label;
     this.start();
   }
   start() {
      this.startTime = process.hrtime();
   }

   stop() {
      this.stopTime = process.hrtime(this.startTime);
      LOG.info((this.stopTime[1] / 1000000) + " ms was the execution time of " + this.label);
   }
}
