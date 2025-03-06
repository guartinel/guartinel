/**
 * Created by DTAP on 2017.08.18..
 */

var moment = require('moment');
exports.getSchema = function (mongoose) {
   var watcherServerSchema = mongoose.Schema({
      address: {
         ip: String,
         port: String
      },
      properties: {
         createdOn: { type: Date, default: Date.now },
         name: String,
         isEnabled: { type: Boolean, default: true },
         categories: [String]
      },

      msToWSAuth: {
         managementServerId: String,
         token: String
      },

      wsToMSAuth: {
         tokenTimeStamp: String,
         token: String,
         watcherServerId: String,
         passwordHash: String,
         oneTimeRegistrationToken: String
      },

      status: {
         timeStamp: Date,
         stressLevel: String,
         isAvailable: { type: Boolean, default: true },
         unavailableTimeStamp: Date,
      }
   });
   watcherServerSchema.methods.getPasswordHash = function () {
      return this.wsToMSAuth.passwordHash;
   };
   watcherServerSchema.methods.setPasswordHash = function (value) {
      this.wsToMSAuth.passwordHash = value;
   };

   watcherServerSchema.methods.getCategories = function () {
      return this.properties.categories;
   };
   watcherServerSchema.methods.getId = function () {
      return this.wsToMSAuth.watcherServerId;
   };
   watcherServerSchema.methods.getCreatedOn = function () {
      return this.properties.createdOn;
   };
   watcherServerSchema.methods.getAddress = function () {
      return this.address.ip;
   };
   watcherServerSchema.methods.getPort = function () {
      return this.address.port;
   };

   watcherServerSchema.methods.getWSToken = function () {
      return this.msToWSAuth.token;
   };
   watcherServerSchema.methods.setWSToken = function (val) {
      this.msToWSAuth.token = val;
   };
   watcherServerSchema.methods.getMSToken = function () {
      return this.wsToMSAuth.token;
   };
   watcherServerSchema.methods.getMSTokenTimeStamp = function () {
      return this.wsToMSAuth.tokenTimeStamp;
   };
   watcherServerSchema.methods.setMSToken = function (value) {
      this.wsToMSAuth.token = value;
      this.wsToMSAuth.tokenTimeStamp = moment().toISOString();
   };
   watcherServerSchema.methods.setMSTokenTimeStamp = function (value) {
      this.wsToMSAuth.tokenTimeStamp = value;
   };
   watcherServerSchema.methods.getName = function () {
      return this.properties.name;
   };
   watcherServerSchema.methods.getIsEnabled = function () {
      return this.properties.isEnabled;
   };
   watcherServerSchema.methods.getStressLevel = function () {
      return this.status.stressLevel;
   };
   watcherServerSchema.methods.getStressLevelTimeStamp = function () {
      return this.status.timeStamp;
   };
   watcherServerSchema.methods.setAddress = function (val) {
      this.address.ip = val;
   };
   watcherServerSchema.methods.setCategories = function (val) {
      this.properties.categories = val;
   };
   watcherServerSchema.methods.setPort = function (val) {
      this.address.port = val;
   };
   watcherServerSchema.methods.setName = function (val) {
      this.properties.name = val;
   };
   watcherServerSchema.methods.setIsEnabled = function (val) {
      this.properties.isEnabled = val;
   };
   watcherServerSchema.methods.setIsAvailable = function (val) {
      this.status.isAvailable = val;
   };
   watcherServerSchema.methods.getUnavailableTimeStamp = function () {
      return this.status.unavailableTimeStamp;
   };
   watcherServerSchema.methods.setUnavailableTimeStamp = function (val) {
      this.status.unavailableTimeStamp = val;
   };
   watcherServerSchema.methods.getIsAvailable = function () {
      return this.status.isAvailable;
   }
   watcherServerSchema.methods.setStressLevel = function (val) {
      this.status.stressLevel = val;
      this.status.timeStamp = moment().toISOString();
   };
   watcherServerSchema.methods.setManagementServerId = function (val) {
      this.msToWSAuth.managementServerId = val;
   };

   watcherServerSchema.methods.getManagementServerId = function () {
      return this.msToWSAuth.managementServerId;
   };
   watcherServerSchema.methods.setOneTimeRegistrationToken = function (val) {
      this.wsToMSAuth.oneTimeRegistrationToken = val;
   };

   watcherServerSchema.methods.getOneTimeRegistrationToken = function () {
      return this.wsToMSAuth.oneTimeRegistrationToken;
   };
   return watcherServerSchema;
};

