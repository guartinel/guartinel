import * as express from "express";
import * as bodyParser from "body-parser";
import { LOG } from "../../diagnostics/LoggerFactory";
import { Const } from "../../common/constants";
import { MSError } from "../../error/Errors";

export let URL = global.managementServerUrls.EMAIL_SUPERVISOR_REGISTER_MEASUREMENT;

export function route(req: any, res: any): void {
    
   let parameters = {
      token: req.body[Const.commonConstants.ALL_PARAMETERS.TOKEN],
      measurement: req.body[Const.commonConstants.ALL_PARAMETERS.MEASUREMENT]
   }
   global.myRequestValidation(parameters, req, res, function (requestErr) {
      if (!requestErr) {
         processRequest(parameters, function (result) {
            return res.send(result);
         });
      }
   });
}

function processRequest(parameters:any,callback:any) {

}