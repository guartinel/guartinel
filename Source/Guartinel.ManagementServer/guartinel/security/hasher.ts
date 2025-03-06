import * as crypto from "crypto";

export function createHash(salt, plain) {
    return crypto.createHash('sha512').update(plain + salt).digest('hex').toUpperCase();
}

export function getCheckSum(text) {
    return crypto.createHash('md5').update(text).digest('hex');
}
