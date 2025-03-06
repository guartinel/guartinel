

import { LOG } from "../../../../diagnostics/LoggerFactory";

export function getSchema(mongoose) {
   var hardwareSchema = mongoose.Schema({
      type: String,
      instanceId: String,
      firmware: mongoose.Schema.Types.Mixed,/*{
         type: String,
         minVersion: Number,
         path: String
      }*/
      remoteDebugURL :String
   });  
   return hardwareSchema;
}

