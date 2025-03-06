exports.build = function (account, stringMessageReq) {
   if (utils.object.isNull(account.language)) {
      account.language = "ENGLISH";
   }
   if (utils.object.isNullOrEmpty(stringMessageReq)) {
      return "";
   }

   if (typeof stringMessageReq === "string" && stringMessageReq.indexOf("{") == -1) { //probably a simple string message which doesnot need building
      LOG.error("Plain text message is sent for building: " + stringMessageReq);
      return stringMessageReq;
   } 

   stringMessageReq = utils.object.ensureAsObject(stringMessageReq);
   var result = stringFillingWorking(stringMessageReq);
   return result;

   function stringFillingWorking(messageStructure) {     
      var finalResult = "";
      if (utils.object.isNullOrEmpty(messageStructure)) {
         return finalResult;
      }
      if (!utils.object.isNull(messageStructure.strings)) {
         for (var i = 0; i < messageStructure.strings.length; i++) {
            var tempResult1 = stringFillingWorking(messageStructure.strings[i]);

            finalResult = finalResult + tempResult1;
         }
      }
      if (!utils.object.isNull(messageStructure.code)) {
         finalResult = languageTable[account.language][messageStructure.code];
         if (utils.object.isNull(finalResult)) {
            LOG.error("Alert message builder: messageStructure.code is not found. Code:" + messageStructure.code);
            return messageStructure.code;
         }
         if (!utils.object.isNull(messageStructure.parameters)) {
            for (var i = 0; i < messageStructure.parameters.length; i++) {
               var currentParameter = messageStructure.parameters[i];

               // try to lookup the value from lookup property
               if (!utils.object.isNull(currentParameter.lookup)) {
                  currentParameter = lookup(account, currentParameter);
               }

               var tempResult = stringFillingWorking(currentParameter.value);
               finalResult = finalResult.replaceAll('#' + currentParameter.name + '#', tempResult);
            }
         }
      }

      if (!utils.object.isNull(messageStructure.string)) {
         return messageStructure.string;
      }

      return finalResult;
   }
};

function lookup(account, currentParameter) {
   var INVALID_DEVICE_ID = "-Invalid deviceID-";
   var INVALID_PACKAGE_ID = "-Invalid packageID-";
   var INVALID_INSTANCE_ID = "-Invalid instanceID-";

   if (currentParameter.lookup === "All") {   
      var instanceNameResult = getInstanceNameFromId(account, currentParameter.value.string);
      if (instanceNameResult !== INVALID_INSTANCE_ID) {
         currentParameter.value.string = instanceNameResult;
      }

      var packageNameResult = getPackageNameFromId(account, currentParameter.value.string);
      if (packageNameResult !== INVALID_PACKAGE_ID) {
         currentParameter.value.string = packageNameResult;
      }
   }  
   if (currentParameter.lookup === "PackageNameFromID") {
      currentParameter.value.string = getPackageNameFromId(account, currentParameter.value.string);
   }

   if (currentParameter.lookup === "InstanceNameFromID") {
      currentParameter.value.string = getInstanceNameFromId(account, currentParameter.value.string);
   }

   function getPackageNameFromId(account, packageId) {
      var foundPackage = account.packages.id(packageId);
      if (!utils.object.isNull(foundPackage)) {
         return foundPackage.packageName;
      } else {
         return INVALID_PACKAGE_ID;
      }
   }

   function getInstanceNameFromId(account, instanceID) {
      var foundInstance;
      if (!utils.object.isNull(account.packages)) {
         account.packages.forEach(function (pack, index) {
            if (!utils.object.isNull(pack.configuration.instances)) {
               pack.configuration.instances.forEach(function (instance, instanceIndex) {
                  if (instance.id === instanceID) {
                     foundInstance = instance;
                  }
               });
            }
         });
      }
      if (!utils.object.isNull(foundInstance)) {
         return foundInstance.name;
      } else {
         return INVALID_INSTANCE_ID;
      }
   }

   return currentParameter;
}
