import {Connection} from "mongoose";
export default class ConnectionState {
   isSuccess: boolean;
   state: string;

   constructor(connection: Connection) {
      if (connection == null) {
         this.isSuccess = false;
         this.state = "Not initialized";
         return;
      }
      if (connection.readyState === 0) {
         this.isSuccess = false;
         this.state = "Disconnected";
      }

      if (connection.readyState === 1) {
         this.isSuccess = true;
         this.state = "Connected";
      }
      if (connection.readyState === 2) {
         this.isSuccess = false;
         this.state = "Connecting";
      }
      if (connection.readyState === 3) {
         this.isSuccess = false;
         this.state = "Disconnecting";
      }
   }
}