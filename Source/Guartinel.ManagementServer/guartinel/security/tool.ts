import * as crypto from "crypto";
import * as base64url from "base64-url";
import * as configurationController from "../admin/configuration/configurationController";
import {Encryption} from "../admin/configuration/MSConfig";
import * as encryptor from "./encryptor";
import * as hasher from "./hasher";
import { LOG } from "../../diagnostics/LoggerFactory";

export function generatePasswordHash(salt, password) {
   return hasher.createHash(salt, password);
};
export function getCheckSum(text) {
    return hasher.getCheckSum(text);
}

export function generateToken() {
   return base64url.encode(crypto.randomBytes(50));
};

export function generateRandomString(length?) {
   if (length == null) {
      length = 20;
   }
   const result = base64url.encode(crypto.randomBytes(length));
   return result;
};


export function encryptArray(array: Array<string>) {
   if (array == null) {
      return array;
   }
  let resultArray = new Array<string>();
   for (let i = 0; i < array.length; i++) {
      let currentItem = array[i];
      resultArray.push(encryptText(currentItem));
   }
   return resultArray;
}

export function decryptArray(array: Array<string>) {
   if (array == null) {
      return array;
   }
    let resultArray = new Array<string>();
   for (let i = 0; i < array.length; i++) {
      let currentItem = array[i];
      resultArray.push(decryptText(currentItem));
   }
   return resultArray;
}

export function encryptObject(object: object) {
   if (object == null) {
      return object;
   }
   let serilization = JSON.stringify(object);
   return encryptText(serilization);
}

export function decryptObject(serialized: string) {
   if (serialized == null) {
      return serialized;
    }
   if (typeof (serialized) != "string") {
      return serialized;
   }
   let decryiptedSerialization = decryptText(serialized);
   return JSON.parse(decryiptedSerialization);
}

export function encryptText(text: string): string {
   let currentEncryption = configurationController.getBaseConfig().security.getCurrentEncryption();
   return encryptor.encryptText(text, currentEncryption);
}

export function decryptText(text: string) {
   if (text == null || text.length === 0) {
      return text;
   }
   return encryptor.decryptText(text, configurationController.getBaseConfig().security);
};
