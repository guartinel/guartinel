/**
 * Created by DTAP on 2017.07.28..
 */

import * as security from "../guartinel/security/tool";
export function  teve (){
   let encrypted = security.encryptText("test");
   let decrypted = security.decryptText(encrypted);
}
teve();