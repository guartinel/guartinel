# API definíció

Guartinel API Definition
Server address
https://backend.guartinel.com:9090/
URI:api/getToken
Summary
Retrieve the token for the api. This call only need to be called once.  The token remain valid until this route is called again.

Request
{
	  "email":"account email",              
                "password":"account password",

}
OK Response
{
                "success":"success",
                "token":"tokenForTheAPI"
}
The provided credentials are invalid
{
                "error":"INVALID_USERNAME_OR_PASSWORD",
	 "error_uuid":"random guid"
}
The user don’t have access for guartinel API
{
                "error":"MISSING_API_ACCESS_RIGHT",
	 "error_uuid":"random guid"
}

URI:api/package/getAll
Summary
Retrieve all existing packages for the account with the name and version number.
Request
{
 "token":"token"
} 
OK Response
{
                "success":"success",
                "packages":[{
"package_name":"packageName",
"version": 2,
"is_enabled": true
}]
}
The token is invalid
{
                               "error":"INVALID_TOKEN",
	 	  "error_uuid":"random guid"
}

URI:api/package/save
Summary
If package with the same name existst on the server then it will be updated with the provided properties in the request, otherwise it will be created.
If only the is_enabled property updated in a package, then the version remain the same.
Example request for an update which will turn of the package
{
"token":"identificationToken",                
"package":{
   "package_name": "packageName",
   "is_enabled": false
}
}

Request for COMPUTER SUPERVISOR package
{
"token":"identificationToken",                
"package":{
   "package_name": "packageName",
                               "package_type": "COMPUTER_SUPERVISOR",                 
                               "configuration": {
                                               "agent_devices": ["MyServerAgentName"],
                                               "check_thresholds": {
                                                               "cpu": {
                                                                              "max_percent": 10
                                                               },
                                                               "memory": {
                                                                              "max_percent": 10
                                                               },
                                                               "hdd": [{
                                                                              "volume": "C",
                                                                              "max_percent": 55
                                                               }]
                                               },
                                               "device_categories": ["test"]
                               },
                               "check_interval_seconds": 60,
                               "alert_emails": ["alertEmail@company.com"],
                               "alert_devices": ["MyAlertDeviceName"],
                               "is_enabled": true,
                               "access": [{  //optional parameter
                                               "packageUserEmail": "otherAccount@company.com",
                                               "canRead": true,
                                               "canEdit": true,
                                               "canDisable": true,
                                               "canDelete": false
                               }]
                }
}
Request for WEBSITE SUPERVISOR package
{
"token":"identificationToken",                
"package":{
   "package_name": "packageName",
                               "package_type": "WEBSITE_SUPERVISOR",                 
                               "configuration": {
			"websites": ["https://address.com"]
                               },
                               "check_interval_seconds": 60,
                               "alert_emails": ["alertEmail@company.com"],
                               "alert_devices": ["MyAlertDeviceName"],
                               "is_enabled": true,
                               "access": [{  //optional parameter
                                               "packageUserEmail": "otherAccount@company.com",
                                               "canRead": true,
                                               "canEdit": true,
                                               "canDisable": true,
                                               "canDelete": false
                               }]
                }
}
Request for HOST SUPERVISOR package
{
"token":"identificationToken",                
"package":{
   "package_name": "packageName",
                               "package_type": "HOST_SUPERVISOR",                 
                               "configuration": {
			"hosts": ["8.8.8.8"]
                               },
                               "check_interval_seconds": 60,
                               "alert_emails": ["alertEmail@company.com"],
                               "alert_devices": ["MyAlertDeviceName"],
                               "is_enabled": true,
                               "access": [{  //optional parameter
                                               "packageUserEmail": "otherAccount@company.com",
                                               "canRead": true,
                                               "canEdit": true,
                                               "canDisable": true,
                                               "canDelete": false
                               }]
                }
}

Request for APPLICATION SUPERVISOR package
NOTE: The ’application_token’ which is required to send measurements is returned when the package is created. It cannot be modified until the package persist.
{
"token":"identificationToken",                
"package":{
   "package_name": "packageName",
                  "package_type": "APPLICATION_SUPERVISOR",                 
                  "configuration": {
		"instances": [{
			"name":"instance name", 
			"id":"instanceID", 										 "is_heartbeat":false // if this is true, then Guartinel will alert if the instance is down }],
	                  },
                               "check_interval_seconds": 60,
                               "alert_emails": ["alertEmail@company.com"],
                               "alert_devices": ["MyAlertDeviceName"],
                               "is_enabled": true,
                               "access": [{  //optional parameter
                                               "packageUserEmail": "otherAccount@company.com",
                                               "canRead": true,
                                               "canEdit": true,
                                               "canDisable": true,
                                               "canDelete": false
                               }]
                }
}


OK Response
{
                "success": "success",
                "version": 0,
                "application_token": "token" // only returned if the request created a new package	
}
The token is invalid
{
                "error ":"INVALID_TOKEN",
	  "error_uuid":"random guid"

}

The package is has an invalid format
{
                "error":"INVALID_PACKAGE_OBJECT",
	 "error_uuid":"random guid"
}

URI:api/package/delete
Summary
Delete an existing package.

Request
{
	"token":"identificationToken", 
               "package_name": "packageName",

}
OK Response
{
                "success": "success"
}
Invalid token response
{

	 "error_uuid":"random guid",
                "error": "INVALID_TOKEN"
}
The package is not found
{

	 "error_uuid":"random guid",
                "error": "INVALID_PACKAGE_NAME"
} 
URI:api/package/getVersion
Summary
Retrieve the package version on the server which has the provided package name.
Request
{
                "token": "identificationToken",
                "package_name": "packageName"
}
OK Response
{
                "success": "success",
                "version": 4
}
Invalid token response
{ 
                "error_uuid":"random guid",
                "error": "INVALID_TOKEN"
}


The package is not found
{
               "error_uuid":"random guid",
                "error": "INVALID_PACKAGE_NAME"
}
URI:api/package/getPackage
Summary
Retrieve the package configuration with the package name.

Request
{
	"token":"identificationToken", 
               "package_name": "packageName",

}
OK Response
{
"success": "success",
"package": {
                               "package_name": "packageName",
                               "version": 4,
                               "package_type": "COMPUTER_SUPERVISOR",
                               "configuration": {
                                               "agent_devices": ["MyServerAgentName"],
                                               "check_thresholds": {
                                                               "cpu": {
                                                                              "max_percent": 10
                                                               },
                                                               "memory": {
                                                                              "max_percent": 10
                                                               },
                                                               "hdd": [{
                                                                              "volume": "C",
                                                                              "max_percent": 55
                                                               }]
                                               },
                                               "device_categories": ["test"]
                               },
                               "check_interval_seconds": 60,
                               "alert_emails": ["alertEmail@company.com"],
                               "alert_devices": ["MyAlertDeviceName"],
                               "is_enabled": true,
                               "access": [{
                                               "packageUserEmail": "otherAccount@company.com",
                                               "canRead": true,
                                               "canEdit": true,
                                               "canDisable": true,
                                               "canDelete": false
                               }]
                }
}
Invalid token response
{
                "error_uuid":"random guid",
                "error": "INVALID_TOKEN"
}
The package is not found
{
               "error_uuid":"random guid",
                "error": "INVALID_PACKAGE_NAME"
}


URI: applicationSupervisor/registerMeasurement
Summary
This route is only used for application supervisor package to register measurements from the application instances.
If the package with the matching application token don’t have an instance with the provided ID, then this instance will be added to the package as a new instance.

Request
{
	"application_token":"unique application_token", 
               "measurement": { 
			"success": "SUCCESS" // if the value is error then the package will be alerted
			"message": "The problem which is caused the error state"
			},
	"instance_name": "This name will be used on  the UI",
	"instance_id": 1, // This number identifies the instance inside the package
	"is_heartbeat":false// if this is true, then Guartinel will alert if the instance is down

}
OK Response
{
    "success": "SUCCESS"
}

If the application token is invalid
{
    "error_uuid": "6b623246-db30-4641-a8cf-658f659946c8",
    "error": "INVALID_TOKEN"
}