import { Encryption } from "../admin/configuration/MSConfig";
import * as crypto from "crypto";
import { SecurityConfig } from "../admin/configuration/MSConfig";

export function encryptText(text: string,currentEncryption:any): string {
   if (text == null || text.length === 0) {
      return text;
   }
 
   return doEncryptionOnText(text, currentEncryption);
}

export function decryptText(text: string, securityConfig: SecurityConfig) {
   if (text == null || text.length === 0) {
      return text;
   }
   let prefix = text.substring(0, 2);
   let textWithoutPrefix = text.substring(2, text.length);
   let usedEncryption = securityConfig.getEncryptionByPrefix(prefix);

   if (usedEncryption == null) {
      return text;
   }
   return doDecryiptionOnText(textWithoutPrefix, usedEncryption);
};

function doEncryptionOnText(text: string, encryption: Encryption): string {
   const cipher = crypto.createCipher('aes-256-cbc', encryption.key); //'d3w4zasdW!G+as123d$'
   let encryptedText = cipher.update(text, 'utf8', 'hex');
   encryptedText += cipher.final('hex');
   return encryption.prefix + encryptedText;
}

function doDecryiptionOnText(text: string, encryption: Encryption): string {
   const decipher = crypto.createDecipher('aes-256-cbc', encryption.key);//'d3w4zasdW!G+as123d$'
   let decryptedText = decipher.update(text, 'hex', 'utf8');
   decryptedText += decipher.final('utf8');
   return decryptedText;
}