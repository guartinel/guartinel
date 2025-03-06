/**
 * Created by DTAP on 2016.05.26..
 */
var mongoose = require('mongoose');
var accountSchema = include('guartinel/database/public/schemas/accountSchema.js');
var licenseSchema = include('guartinel/database/public/schemas/licenseSchema.js');
var controlledEmailSchema = include('guartinel/database/public/schemas/controlledEmailSchema.js');
var licenseOrderSchema = include('guartinel/database/public/schemas/licenseOrderSchema.js');
var hardwareSchema = include('guartinel/database/public/schemas/hardwareSchema.js');
var hardwareFirmwareSchema = include('guartinel/database/public/schemas/hardwareFirmwareSchema.js');

var ExecutionMeterFactory = include("diagnostics/ExecutionMeterFactory.js");
var securityTool = include('guartinel/security/tool.js');
var moment = require('moment');
var config = include('guartinel/admin/configuration/configurationController.js');

var AccountModel;
var LicenseModel;
var ControlledEmailModel;
var LicenseOrderModel;
var AccountModel;
var HardwareModel;
var HardwareFirmwareModel;

exports.toObjectId = function (stringID) {
   return mongoose.Types.ObjectId(stringID);
}

exports.getAccountModel = function () {
   return AccountModel;
};

exports.getLicenseModel = function () {
   return LicenseModel;
}

exports.getControlledEmailModel = function () {
   return ControlledEmailModel;
}

exports.getLicenseOrderModel = function () {
   return LicenseOrderModel;
}
exports.getHardwareModel = function () {
   return HardwareModel;
}

exports.getHardwareFirmwaresModel = function () {
   return hardwareFirmwaresSchema;
}
var Admin = mongoose.mongo.Admin;

exports.isConnectedToDB = function () {
   if (utils.object.isNull(connection)) {
      return false;
   }
   return connection.readyState === 1;
}
exports.getNativeAccountsConnection = function () {
   return connection.db.collection('accounts');
}

exports.disconnect = function (callback) {
   if (utils.object.isNull(connection)) {
      return callback();
   }
   connection.close(function () {
      return callback();
   });
}

exports.getConnectionState = function () {
   function result(isSuccess, state) {
      this.isSuccess = isSuccess;
      this.state = state;
   }

   if (utils.object.isNull(connection)) {
      return new result(false, "Not initialized");
   }
   if (connection.readyState == 0) {
      return new result(false, "Disconnected");
   }

   if (connection.readyState == 1) {
      return new result(true, "Connected");
   }
   if (connection.readyState == 2) {
      return new result(false, "Connecting");
   }
   if (connection.readyState == 3) {
      return new result(false, "Disconnecting");
   }
};

var isInit = true;
var connection;
exports.connect = function (callback) {
   config.getLocalConfiguration(function (err, configuration) {

      var passwordDecrypted = securityTool.decryptText(configuration.databaseConfiguration.passwordEncrypted);
      var options = {
         user: configuration.databaseConfiguration.userName,
         pass: passwordDecrypted
      }
      var url = configuration.databaseConfiguration.url;
      mongoose.Promise = global.Promise;
      connection = mongoose.createConnection(url, options); //TODO options must be enabled in PROD

      AccountModel = connection.model('account', accountSchema.getSchema(mongoose));
      LicenseModel = connection.model('license', licenseSchema.getSchema(mongoose));
      ControlledEmailModel = connection.model('controlledEmail', controlledEmailSchema.getSchema(mongoose));
      LicenseOrderModel = connection.model('licenseOrder', licenseOrderSchema.getSchema(mongoose));
      HardwareModel = connection.model('hardware', hardwareSchema.getSchema(mongoose));
      HardwareFirmwareModel = connection.model('hardwareFirmware', hardwareFirmwareSchema.getSchema(mongoose));
      connection.on('open', function (err) {
         if (err && isInit) {
            isInit = false;
            return callback(new MSError("Public DB").logMessage("Cannot connect to Public Database").innerError(err).logNow());
         }
         LOG.info("Public database connection is open.");

         if (isDebug()) {
            mongoose.set('debug', true);
         }
         if (isInit) {
            isInit = false;
            return callback();
         }

      });

      connection.on('error', function (err) {
         if (isInit) {
            isInit = false;
            return callback(new MSError("Public DB").logMessage("Cannot connect to Public Database").innerError(err).logNow());
         }
      });
   });
}


exports.getDbVersion = function getDatabaseInfo(callback) {
   var admin = new mongoose.mongo.Admin(connection.db);
   admin.buildInfo(function (err, info) {
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("Public DB: getDbVersion error")
            .innerError(err)
            .logNow());
      }
      return callback(info.version);
   });
}

exports.updateTokenForDevice = function (token, callback) {
   AccountModel.update({ 'devices.token': token }, { $set: { 'devices.$.tokenTimeStamp': moment().toISOString() } }, function (err, success) {
      if (err) {
         LOG.info(JSON.stringify(err));
         return callback(err);
      }

      return callback(null);
   });
}

exports.updateTokenForBrowserSession = function (token, callback) {
   AccountModel.update({ 'browserSessions.token': token }, { $set: { 'browserSessions.$.tokenTimeStamp': moment().toISOString() } }, function (err, success) {
      if (err) {
         LOG.info(JSON.stringify(err));
         return callback(err);
      }
      return callback(null);
   });
}
exports.getCollectionInfos = function (callback) {
   AccountModel.collection.stats(function (err, result) {
      if (err) {
         LOG.error("getCollectionInfos err:" + JSON.stringify(err));
         return callback(err);
      }

      return callback(null, result);
   });
}


exports.isIdValid = function (id) {
   return mongoose.Types.ObjectId.isValid(id);
}

exports.getAccountByMultiplePropertyValues = function(property, values, callback) {
   //{ $or: [{ 'property': propertyValue1 }, { 'property': propertyValue2 }, { 'property': propertyValue3 }] } expected query string

   var query = {};
   query.$or = [];
   for (var i = 0; i < values.length; i++) {
      var queryPart = {};
      queryPart[property] = values[i];
      query.$or.push(queryPart);
   }
 
   var meter = ExecutionMeterFactory.getExecutionMeter("DB time " + JSON.stringify(query) + " ");
   AccountModel.findOne(query, function (err, account) {
      meter.stop();
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getAccountByMultiplePropertyValues")
            .innerError(err)
            .logNow());
      }
      callback(null, account);
   });
}



//{"email":"0167244efb5c95e96eb14c51b864c9627c21658556eb672646f79ae56907261a19"} 
exports.getAccountByProperty = function (property, propertyValue, callback) {
   var query = {};
   query[property] = propertyValue;
   var meter = ExecutionMeterFactory.getExecutionMeter("DB time " + JSON.stringify(query) + " ");
   AccountModel.findOne(query, function (err, account) {
      meter.stop();
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getAccountByProperty")
            .innerError(err)
            .logNow());
      }
      callback(null, account);
   });
}
//                                                    packages              applicationToken
exports.getAccountBySubDocumentProperty = function (subDocumentCollectionProperty, property, propertyValue, callback) {

   var propertyQuery = {};
   propertyQuery[property] = propertyValue;

   var elemMatchQuery = {}
   elemMatchQuery['$elemMatch'] = propertyQuery;

   var query = {};
   query[subDocumentCollectionProperty] = elemMatchQuery;
   var meter = ExecutionMeterFactory.getExecutionMeter("DB time " + JSON.stringify(query) + " ");

   AccountModel.findOne(query, function (err, account) {
      meter.stop();
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getAccountByProperty" + "Cannot run sub-document querry. Subdocumentcollection property: " + subDocumentCollectionProperty + " property:" + property + " propertyValue" + propertyValue)
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, account);
   });
}

exports.getMultipleAccountBySubDocumentProperty = function (subDocumentCollectionProperty, property, propertyValue, callback) {

   var propertyQuery = {};
   propertyQuery[property] = propertyValue;

   var elemMatchQuery = {}
   elemMatchQuery['$elemMatch'] = propertyQuery;

   var query = {};
   query[subDocumentCollectionProperty] = elemMatchQuery;
   var meter = ExecutionMeterFactory.getExecutionMeter("DB time " + JSON.stringify(query) + " ");

   AccountModel.find(query, function (err, account) {
      meter.stop();
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getMultipleAccountBySubDocumentProperty" + "Subodocumentcollection: " + subDocumentCollectionProperty + " property:" + property + " propertyValue" + propertyValue)
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, account);
   });
}

exports.getMultipleAccounts = function (property, propertyValue, callback) {
   var query = {};
   query[property] = propertyValue;
   var meter = ExecutionMeterFactory.getExecutionMeter("DB time " + JSON.stringify(query) + " ");
   AccountModel.find(query, function (err, accounts) {
      meter.stop();
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getMultipleAccounts Property: " + property + " propertyValue: " + propertyValue)
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, accounts);
   });
}

exports.getLicenseByProperty = function (property, propertyValue, callback) {
   var query = {};
   query[property] = propertyValue;
   LicenseModel.findOne(query, function (err, license) {
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getLicenseByProperty Property: " + property + " propertyValue: " + propertyValue)
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, license);
   });
}


exports.getMultipleLicenses = function (property, propertyValue, callback) {
   var query = {};
   query[property] = propertyValue;

   LicenseModel.find(query, function (err, licenses) {
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getMultipleLicenses Property: " + property + " propertyValue: " + propertyValue)
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, licenses);
   });
}

exports.getControlledEmailByProperty = function (property, propertyValue, callback) {
   var query = {};
   query[property] = propertyValue;
   ControlledEmailModel.findOne(query, function (err, controlledEmail) {
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getControlledEmailByProperty Property: " + property + " propertyValue: " + propertyValue)
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, controlledEmail);
   });
}
exports.getHardwareByProperty = function (property, propertyValue, callback) {
   var query = {};
   query[property] = propertyValue;
   HardwareModel.findOne(query, function (err, hardware) {
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getHardwareByProperty Property: " + property + " propertyValue: " + propertyValue)
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, hardware);
   });
}
exports.getHardwareFirmwareByProperty = function (property, propertyValue, callback) {
   var query = {};
   query[property] = propertyValue;
   HardwareFirmwareModel.findOne(query, function (err, hardwareFirmware) {
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getHardwareFirmwareByProperty Property: " + property + " propertyValue: " + propertyValue)
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, hardwareFirmware);
   });
}


exports.getLicenseOrderByProperty = function (property, propertyValue, callback) {
   var query = {};
   query[property] = propertyValue;
   LicenseOrderModel.findOne(query, function (err, licenseOrder) {
      if (err) {
         return callback(new MSInternalServerError()
            .logMessage("databaseConnector.getLicenseOrderByProperty Property: " + property + " propertyValue: " + propertyValue)
            .innerError(err)
            .severe()
            .logNow());
      }
      return callback(null, licenseOrder);
   });
}