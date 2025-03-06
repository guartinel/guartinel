import * as uuid from "node-uuid";
import { Const } from "../common/constants";
import { LOG } from "../diagnostics/LoggerFactory";

export class MSError {
   protected _errorLabel: string; // this is publicly sent like: INTERNAL_SERVER_ERROR
   protected _logMessage: string; // this is private:like cannot connect to database because exception and stack
   protected _UUID: string;
   protected _innerErr: Error;
   protected _errorDetails: any;
   constructor(errorLabel: string, errorDetails?: string) {
      this._errorLabel = errorLabel;
      this._UUID = uuid.v4();
      this._errorDetails = errorDetails;
   }
   getFullContent(): string {
      let serializedInnerError: string = "-";
      if (this.getInnerError() == null) {
         serializedInnerError = "-";
      } else {
         serializedInnerError = JSON.stringify(this.getInnerError());
      }
      let resultMessage = global.utils.string.getProcessInfo() + " UUID: " + this.getUUID() + " Label: " + this.getErrorLabel() + " Message: " + this.getLogMessage() + " InnerCause: " + serializedInnerError;
      return resultMessage;
   }
   getDetails(): string {
      return this._errorDetails;
   }
   getErrorLabel(): string {
      return this._errorLabel;
   }
   getLogMessage(): string {
      return this._logMessage;
   }
   getUUID(): string {
      return this._UUID;
   }
   getInnerError(): Error {
      return this._innerErr;
   }
   details(details: string): MSError {
      this._errorDetails = details;
      return this;
   }
   innerError(err: Error): MSError {
      this._innerErr = err;
      return this;
   }
   logMessage(msg: string): MSError {
      this._logMessage = msg;
      return this;
   }
   uuid(_uuid: string): MSError {
      this._UUID = _uuid;
      return this;
   }
   logNow(): MSError {
      LOG.logMSError(this);
      return this;
   }
   severe(): MSError {
      SevereErrors.push(this);
      return this;
   }
}

export class MSInternalServerError extends MSError {
   constructor() {
      super(Const.commonConstants.ALL_ERROR_VALUES.INTERNAL_SYSTEM_ERROR);
   }
}
export class LicenseError extends MSError {
   constructor(details: string) {
      super(Const.commonConstants.ALL_ERROR_VALUES.LICENSE_ERROR, details);
   }
}
export let SevereErrors: Array<MSError> = new Array<MSError>();

export function registerGlobals() {
   global.MSError = MSError;
   global.MSInternalServerError = MSInternalServerError;
   global.MSSevereError = MSError;
}
