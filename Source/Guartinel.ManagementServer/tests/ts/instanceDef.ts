export class InstanceDef {
   created: number[];
   constructor() {
      this.created = process.hrtime();
   }

}