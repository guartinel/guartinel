/**
 * Created by DTAP on 2017.02.24..
 */

global.RouteResponse = {
    Success: function () {
        this[safeGet(commonConstants.ALL_PARAMETERS.SUCCESS)] = safeGet(commonConstants.ALL_SUCCESS_VALUES.SUCCESS);
    },
    Error: function (error) {
        this[safeGet(commonConstants.ALL_PARAMETERS.ERROR_UUID)] = error.errorUUID;
        this[safeGet(commonConstants.ALL_PARAMETERS.ERROR)] = error.error;
    }
};
