import { MSError } from "../../error/Errors";
import { Const} from "../../common/constants";

export class SuccessResponse {
   constructor() {
      this[Const.commonConstants.ALL_PARAMETERS.SUCCESS] = Const.commonConstants.ALL_SUCCESS_VALUES.SUCCESS;
   }
}

export class ErrorResponse {
   constructor(error:MSError) {
      this[Const.commonConstants.ALL_PARAMETERS.ERROR] = error.getErrorLabel();
      this[Const.commonConstants.ALL_PARAMETERS.ERROR_UUID] = error.getUUID();
      this[Const.commonConstants.ALL_PARAMETERS.ERROR_DETAILS] = error.getDetails();
   }
}

export function registerGlobals() {
   global.SuccessResponse = SuccessResponse;
   global.ErrorResponse = ErrorResponse;
}

