
export function getSchema(mongoose) {
   var hardwareFirmwareSchema = mongoose.Schema({
      type: String,
      minVersion: Number,    
      path: String
   });
   return hardwareFirmwareSchema;
}
;
