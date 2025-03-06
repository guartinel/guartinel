/**
 * Created by DTAP on 2017.03.21..
 */
import { LOG } from "../../diagnostics/LoggerFactory";
import * as Errors from "../../error/Errors";
import MSInternalServerError = Errors.MSInternalServerError;
import * as cluster from "cluster";
import * as moment from "moment";
import * as uuid from "node-uuid";

export function registerGlobals() {
   global.utils = new CommonUtils();
}

class CommonUtils {
   string: String2;
   object: Object2;
   constructor() {
      this.string = new String2();
      this.object = new Object2();
   }
}

class Object2 {
   ensureAsObject(data) {
      if (this.isObject(data)) {
         return data;
      }
      try {
         return JSON.parse(data);
      } catch (err) {
         LOG.error("Cannot parse data : " + data + "Typeof: " + typeof data + " Err:" + err);
         return data;
      }
   }

   isObject(param) {
      if (param === null) {
         return false;
      }
      return ((typeof param === 'function') || (typeof param === 'object'));
   }

   isNull(param) {
      if (typeof param == 'undefined' || param == undefined || param == null) {
         return true;
      }
      return false;
   }

   isNullOrEmpty(param) {
      if (this.isNull(param)) {
         return true;
      }

      if (param.length == 0) {
         return true;
      }

      return false;
   }
   sanitizePropertyValuesByKey(interestingKeys, obj) {
      for (var key in obj) {
         if (typeof obj[key] == "object" && obj[key] !== null)
            this.sanitizePropertyValuesByKey(interestingKeys, obj[key]);
         else {
            if (interestingKeys.indexOf(key) != -1) {
               obj[key] = "***************";
            }
         }
      }
   }
}

class String2 {
   getProcessInfo(): string {
      let processLabel: string;
      if (cluster.isMaster) {
         processLabel = "MASTER";
      } else {
         processLabel = "SLAVE";
      }
      return moment().utc().toISOString() + " " + processLabel + " : " + process.pid + " | ";
   };
   generateUUID  () :string{
      return uuid.v4();
   };
   tryStringify(raw) {
      try {
         if (typeof raw === 'string' || raw instanceof String) {
            return raw;
         }
         return JSON.stringify(raw);
      } catch (err) {
         LOG.error("Cannot stringify object. Err:" + err);
      }
      return raw;
   };

   isNullOrEmpty(text: string) {
      if (text == null) {
         return true;
      }

      if (text.length === 0) {
         return true;
      }
      return false;
   }

   traceIfNull(key) {
      if (key == null) {
         LOG.error("traceIfNull -> Cannot get value." + new Error().stack);
         return "";
      }
      return key;
   }
}