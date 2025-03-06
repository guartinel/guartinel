import * as instanceHolder from "./instanceHolder";
export function run() :string{
   console.log(instanceHolder.instance.created);

   return "USER1 seeing instance as :" + instanceHolder.instance.created;

}