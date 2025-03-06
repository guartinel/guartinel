var helper = require('../../../helper.js'); // must be required first to used as globals
var commons = include('common/constants.js'); // must be stay to here to be used as global variable later in the app
var alertMessageBuilder = include('guartinel/utils/alertMessageBuilder.js');
var logger = include('logger.js');
var assert = require("assert");

describe('testing alert message helper', function () {
    before(function () {
        var temp = account.devices;
        account.devices = {
            elements: temp,
            id: function (searchedID) {
                return this.elements.find(function (x) {
                    if (x._id == searchedID) {
                        return true;
                    }
                    return false;
                });
            }
        }
    });

    it('testing without lookup', function () {
        var alertMessage = {
            string: "COMPUTER_SUPERVISOR.ALERT",
            parameters: [
                {
                    name: "ALERT_MESSAGE",
                    value: [{
                        name: "CPU_ALERT",
                        value: {
                            string: "COMPUTER_SUPERVISOR.CPU_ALERT",
                            parameters: [{
                                name: "CPU_LOAD",
                                value: "0.9"
                            }, {
                                name: "CPU_LOAD_MAX",
                                value: "0.5"
                            }]
                        }
                    },
                        {
                            name: "MEMORY_GB_ALERT",
                            value: {
                                string: "COMPUTER_SUPERVISOR.MEMORY_GB_ALERT",
                                parameters: [{
                                    name: "MEMORY_USAGE_GB",
                                    value: "0.9"
                                }, {
                                    name: "MEMORY_MAX_USAGE_GB",
                                    value: "0.5"
                                }]
                            }
                        }]
                },
                {
                    name: "AGENT_NAME",
                    value: "tevePata"
                }]
        };

        var result = alertMessageBuilder.build(account, alertMessage);
    });

  

    it('testing with string source ', function () {
        var alertParameters = "{\r\n  \"string\": \"\"\r\n}";
        var result = alertMessageBuilder.build(account, alertParameters);
    });


    it('testing with string source ', function () {
        var alertParameters = {"code": "COMPUTER_SUPERVISOR.NOT_AVAILABLE_ALERT"};
        var result = alertMessageBuilder.build(account, alertParameters);
    });
    it('testing with string source ', function () {
        var alertParameters = {
            "code": "WEBSITE_SUPERVISOR.ERROR_ACCESSING_WEBSITssE",
            "parameters": [{
                "name": "WEB_SITE_ADDRESS",
                "value": {"string": "https://manage.guartinel.comm"}
            }, {
                "name": "ERROR_MESSAGE",
                "value": {"string": "The remote name could not be resolved: 'manage.guartinel.comm'"}
            }]
        };
        var result = alertMessageBuilder.build(account, alertParameters);
    });

    it('testing with string source ', function () {
        var alertParameters = { "strings": [{
            "code": "COMPUTER_SUPERVISOR.AGENT_NAME",
            "parameters": [{
                "name": "AGENT_NAME",
                "lookup": "agentNameFromId",
                "value": {
                    "string": "590884d2809921541083a989"
                }
            }]
        },
            {
                "code": "COMPUTER_SUPERVISOR.ALERT_MESSAGE",
                "parameters": [{
                    "name": "PACKAGE_NAME",
                    "lookup": "packageNameFromId",
                    "value": {
                        "string": "58ff159da334280411bc0b31"
                    }
                }]
            },
            {
                "code": "COMPUTER_SUPERVISOR.MEMORY_PERCENT_ALERT",
                "parameters": [{
                    "name": "MEMORY_USAGE_PERCENT",
                    "value": {
                        "string": 10.68
                    }
                },
                    {
                        "name": "MEMORY_MAX_USAGE_PERCENT",
                        "value": {
                            "string": 1
                        }
                    }]
            }]};
        var result = alertMessageBuilder.build(account, alertParameters);
    });

    it('every placeholder test in computer supervisor', function () {
        var alertParameters =  {
            "strings": [{
                "code": "COMPUTER_SUPERVISOR.ALERT_MESSAGE",
                "parameters": [{
                    "name": "PACKAGE_NAME",
                    "value": {
                        "string": "590c259ff0e93fc4136aa2c1"
                    }
                }]
            },
                {
                    "code": "COMPUTER_SUPERVISOR.AFFECTED_AGENT",
                    "parameters": [{
                        "name": "AGENT_NAME",
                        "value": {
                            "string": "590acc0c7baa376c11a40c4f"
                        }
                    }]
                },
                {
                    "code": "COMPUTER_SUPERVISOR.CPU_ALERT",
                    "parameters": [{
                        "name": "CPU_LOAD",
                        "value": {
                            "string": 13.87
                        }
                    },
                        {
                            "name": "CPU_LOAD_MAX",
                            "value": {
                                "string": 10
                            }
                        }]
                },
                {
                    "code": "COMPUTER_SUPERVISOR.MEMORY_GB_ALERT",
                    "parameters": [{
                        "name": "MEMORY_FREE_GB",
                        "value": {
                            "string": 3.91
                        }
                    },
                        {
                            "name": "MEMORY_MIN_FREE_GB",
                            "value": {
                                "string": 26.0
                            }
                        }]
                },
                {
                    "code": "COMPUTER_SUPERVISOR.MEMORY_PERCENT_ALERT",
                    "parameters": [{
                        "name": "MEMORY_USAGE_PERCENT",
                        "value": {
                            "string": 75.38
                        }
                    },
                        {
                            "name": "MEMORY_MAX_USAGE_PERCENT",
                            "value": {
                                "string": 40
                            }
                        }]
                },
                {
                    "code": "COMPUTER_SUPERVISOR.HDD_GB_ALERT",
                    "parameters": [{
                        "name": "HDD_FREE_GB",
                        "value": {
                            "string": 27.87
                        }
                    },
                        {
                            "name": "HDD_MIN_FREE_GB",
                            "value": {
                                "string": 100.0
                            }
                        },
                        {
                            "name": "VOLUME",
                            "value": {
                                "string": "C"
                            }
                        }]
                },
                {
                    "code": "COMPUTER_SUPERVISOR.HDD_PERCENT_ALERT",
                    "parameters": [{
                        "name": "HDD_USAGE_PERCENT",
                        "value": {
                            "string": 80.97
                        }
                    },
                        {
                            "name": "HDD_MAX_USAGE_PERCENT",
                            "value": {
                                "string": 45
                            }
                        },
                        {
                            "name": "VOLUME",
                            "value": {
                                "string": "C"
                            }
                        }]
                }]
        };
        var result = alertMessageBuilder.build(account, alertParameters);
    });

    it('recovery alert test ', function () {
        var alertParameters = {
            "code": "COMMON.RECOVERY_ALERT",
            "parameters": [{
                "name": "packageNameFromId",
                "value": {
                    "string": "590ad24b7baa376c11a40cbb"
                }
            }]
        };
        var result = alertMessageBuilder.build(account, alertParameters);
    });



});


function ObjectId(string) {
    return string;
}
function ISODate(date) {
}
var account = {
    "_id" : ObjectId("590a54f37baa376c11a40bcc"),
    "firstName" : "béla",
    "lastName" : "moqs",
    "email" : "moqs001@gmail.com",
    "passwordHash" : "FC2F2F877F7B3DA69D068B2C29B80E4E3C4951580CE79F8862FCCE5686D6FF83E8B4D9F4964F4DFF409C3C5357B40D8BD08BCEB86D6E2CE15E291DE733227E1E",
    "licenses" : [
        {
            "license" : {
                "_id" : ObjectId("57b301dc98fd416808b6911b"),
                "name" : "free",
                "caption" : "Free",
                "maximumPackages" : 22,
                "maximumDevices" : 22,
                "minimumCheckIntervalSec" : 30,
                "maximumAlertsPerHour" : 10,
                "__v" : 0,
                "prices" : [
                    {
                        "price" : 0,
                        "interval" : 999
                    }
                ],
                "categories" : [
                    "free",
                    "default"
                ],
                "createdOn" : ISODate("2016-08-16T12:06:52.698Z")
            },
            "startDate" : "2017-05-03T22:08:51.944Z",
            "expiryDate" : "2117-05-03T22:08:51.944Z",
            "_id" : ObjectId("590a54f37baa376c11a40bcd"),
            "payment" : {
                "createdOn" : ISODate("2017-05-03T22:08:51.953Z")
            },
            "createdOn" : ISODate("2017-05-03T22:08:51.953Z")
        }
    ],
    "devices" : [
        {
            "passwordHash" : "63100F674F26F2A476482E949F556CCF6892C5636AC496E1BAC191DAB656AA9705145DEA3D0F269CD56E34F14D0A1BB98F2942EA2FC224F757DEC3BFBD353094",
            "deviceType" : "windows_agent_device",
            "name" : "buldozer",
            "token" : "395d08938dd2bf9684f29e2540326890b48de2dafc722000cc46189521872feed05d5ea50a132c47781ae4904a3d00a61e342b81fb58e92d4dd089ada745ed07",
            "tokenTimeStamp" : "2017-05-05T07:30:45.894Z",
            "properties" : "{\r\n  \"system\": \".NET 4.6 on Windows 8.1 64 - bit or later\",\r\n  \"Environment.MachineName\": \"BULDOZER\",\r\n  \"Environment.OSVersion\": \"Microsoft Windows NT 6.1.7601 Service Pack 1\",\r\n  \"Environment.UserName\": \"DAVT\",\r\n  \"Environment.SystemDirectory\": \"C:\\\\Windows\\\\system32\",\r\n  \"Environment.CurrentDirectory\": \"G:\\\\Desktop\\\\Guartinel.Agent.ThinInstaller.exe\\\\inst\\\\bin\",\r\n  \"Environment.Is64BitOperatingSystem\": true,\r\n  \"Environment.Is64BitProcess\": true,\r\n  \"GuartinelAgentVersion\": \"0.9.2.10\"\r\n}",
            "_id" : ObjectId("590acc0c7baa376c11a40c4f"),
            "alerts" : [],
            "createdOn" : ISODate("2017-05-04T06:37:00.506Z"),
            "categories" : [
                "test1"
            ]
        },
        {
            "passwordHash" : "C7CCCFAA097879C01D10839AFED43CD55F60842E737FA4C24B6B69F47D732E35FAD394ACB8CEA7756563C9CDA65B37571C4E32B619D098EAE750EAD1ED34F91D",
            "deviceType" : "android_device",
            "name" : "lgg4",
            "token" : "89e1fd193a3234a1c66880463aad6213022ad2bcde09da1f988a6261f865c3607fee95ae280fd7206c3103c191bd15b8c76b8b475ee2e0ea15fbcf18b733820e",
            "tokenTimeStamp" : "2017-05-05T07:24:50.772Z",
            "properties" : "{\"BuildInfo\":{\"VERSION.RELEASE\":\"6.0\",\"VERSION.INCREMENTAL\":\"1620215065b6f\",\"VERSION.SDK_INT\":23,\"FINGERPRINT\":\"lge\\/p1_global_com\\/p1:6.0\\/MRA58K\\/1620215065b6f:user\\/release-keys\",\"BOARD\":\"msm8992\",\"BRAND\":\"lge\",\"DEVICE\":\"p1\",\"MANUFACTURER\":\"LGE\",\"MODEL\":\"LG-H815\"},\"GuartinelVersion\":\"0.9.0.7\"}",
            "gcmId" : "eHk8nqaNyGc:APA91bFGsD7TscnveihA0ndU7bd1uWj0b-XfCJFZHGNbriEWuiRclLo_mv4ocKFZ3pWKm6C7R_QpmvqsREWa8irCWU52YSVqwBM2VRzAKiFSRL_A7xqR1Odte_W-UxnOdii-_8dDQJ6e",
            "_id" : ObjectId("590ad9657baa376c11a40ede"),
            "alerts" : [
                {
                    "alertID" : "2d0b7799-872b-45df-846f-9d2bacfab351",
                    "watcherServerID" : "Nxk8MpV9yfEz4A",
                    "packageID" : "590ad24b7baa376c11a40cbb",
                    "instanceID" : "590acc0c7baa376c11a40c4f",
                    "_id" : ObjectId("590b421cf0e93fc4136a4dc7"),
                    "createdOn" : ISODate("2017-05-04T15:00:44.185Z")
                }
            ],
            "createdOn" : ISODate("2017-05-04T07:33:57.518Z")
        },
        {
            "passwordHash" : "8C7C7CF469094D0F1855E22343BD3B7B168313651F17B6E129D058211AA583256B188A5B60A1519F9FFDFF3D136B201D98C497987AFC10A4ED47356B5051C9EE",
            "deviceType" : "linux_agent_device",
            "name" : "shield",
            "token" : "43d9767efdadc43121828b61edfb891320a87820bf78b9170b0731f38b7d2fa6331d49078badb258fa739dd89fe937f4fce57dfd0f229ad394aeacf696a80fa3",
            "tokenTimeStamp" : "2017-05-05T07:23:38.771Z",
            "properties" : "{\"java.runtime.name\":\"Java(TM) SE Runtime Environment\",\"sun.boot.library.path\":\"/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/arm\",\"java.vm.version\":\"25.65-b01\",\"java.vm.vendor\":\"Oracle Corporation\",\"java.vendor.url\":\"http://java.oracle.com/\",\"path.separator\":\":\",\"java.vm.name\":\"Java HotSpot(TM) Client VM\",\"file.encoding.pkg\":\"sun.io\",\"user.country\":\"GB\",\"sun.java.launcher\":\"SUN_STANDARD\",\"sun.os.patch.level\":\"unknown\",\"java.vm.specification.name\":\"Java Virtual Machine Specification\",\"user.dir\":\"/home/admin\",\"java.runtime.version\":\"1.8.0_65-b17\",\"java.awt.graphicsenv\":\"sun.awt.X11GraphicsEnvironment\",\"java.endorsed.dirs\":\"/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/endorsed\",\"os.arch\":\"arm\",\"java.io.tmpdir\":\"/tmp\",\"line.separator\":\"\\n\",\"java.vm.specification.vendor\":\"Oracle Corporation\",\"os.name\":\"Linux\",\"sun.jnu.encoding\":\"UTF-8\",\"java.library.path\":\"/usr/java/packages/lib/arm:/lib:/usr/lib\",\"java.specification.name\":\"Java Platform API Specification\",\"java.class.version\":\"52.0\",\"sun.management.compiler\":\"HotSpot Client Compiler\",\"os.version\":\"4.4.34+\",\"user.home\":\"/home/admin\",\"sun.arch.abi\":\"gnueabihf\",\"user.timezone\":\"Etc/UTC\",\"java.awt.printerjob\":\"sun.print.PSPrinterJob\",\"file.encoding\":\"UTF-8\",\"java.specification.version\":\"1.8\",\"java.class.path\":\"GuartinelAgentInstaller.jar\",\"user.name\":\"admin\",\"java.vm.specification.version\":\"1.8\",\"sun.java.command\":\"GuartinelAgentInstaller.jar\",\"java.home\":\"/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre\",\"sun.arch.data.model\":\"32\",\"user.language\":\"en\",\"java.specification.vendor\":\"Oracle Corporation\",\"awt.toolkit\":\"sun.awt.X11.XToolkit\",\"java.vm.info\":\"mixed mode\",\"java.version\":\"1.8.0_65\",\"java.ext.dirs\":\"/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/ext:/usr/java/packages/lib/ext\",\"sun.boot.class.path\":\"/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/resources.jar:/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/rt.jar:/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/sunrsasign.jar:/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/jsse.jar:/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/jce.jar:/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/charsets.jar:/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/lib/jfr.jar:/usr/lib/jvm/jdk-8-oracle-arm32-vfp-hflt/jre/classes\",\"java.vendor\":\"Oracle Corporation\",\"file.separator\":\"/\",\"java.vendor.url.bug\":\"http://bugreport.sun.com/bugreport/\",\"sun.io.unicode.encoding\":\"UnicodeLittle\",\"sun.cpu.endian\":\"little\",\"sun.cpu.isalist\":\"\",\"GuartinelAgentVersion\":\"0.9.0.4\",\"LinuxDistribution\":\"Linux shield 4.4.34+ #930 Wed Nov 23 15:12:30 GMT 2016 armv6l GNU/Linux\"}",
            "_id" : ObjectId("590af7474dbafebc0586b735"),
            "alerts" : [],
            "createdOn" : ISODate("2017-05-04T09:41:27.725Z"),
            "categories" : [
                "test1"
            ]
        }
    ],
    "accessiblePackageIds" : [],
    "packages" : [
        {
            "isEnabled" : false,
            "packageType" : "COMPUTER_SUPERVISOR",
            "packageName" : "testAgentPackage",
            "checkIntervalSeconds" : 30,
            "configuration" : {
                "timeout_in_seconds" : 60,
                "device_categories" : [
                    "test1"
                ],
                "check_thresholds" : {
                    "memory" : {
                        "max_percent" : 99
                    }
                },
                "agent_devices" : []
            },
            "lastModificationTimeStamp" : "2017-05-05T07:23:42.919Z",
            "watcherServerId" : null,
            "applicationToken" : "EMPTY",
            "_id" : ObjectId("590ad24b7baa376c11a40cbb"),
            "instances" : [],
            "access" : [
                {
                    "packageUserEmail" : "moqs001@gmail.com",
                    "canEdit" : true,
                    "canDelete" : true,
                    "canDisable" : true,
                    "canRead" : true,
                    "_id" : ObjectId("590ad24b7baa376c11a40cbc")
                }
            ],
            "isDeleted" : false,
            "states" : [
                {
                    "_id" : ObjectId("590c2881f0e93fc4136aa657"),
                    "timeStamp" : ISODate("2017-05-05T07:23:45.531Z"),
                    "message" : "Everything is OK.",
                    "state" : "ok"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-05T07:23:45.471Z"),
                    "_id" : ObjectId("590c2881f0e93fc4136aa655")
                },
                {
                    "_id" : ObjectId("590c2863f0e93fc4136aa633"),
                    "timeStamp" : ISODate("2017-05-05T07:23:15.436Z"),
                    "message" : "Everything is OK.",
                    "state" : "ok"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-05T07:23:15.377Z"),
                    "_id" : ObjectId("590c2863f0e93fc4136aa631")
                },
                {
                    "_id" : ObjectId("590c2845f0e93fc4136aa609"),
                    "timeStamp" : ISODate("2017-05-05T07:22:45.467Z"),
                    "message" : "Everything is OK.",
                    "state" : "ok"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-05T07:22:45.408Z"),
                    "_id" : ObjectId("590c2845f0e93fc4136aa607")
                },
                {
                    "_id" : ObjectId("590c2827f0e93fc4136aa5e0"),
                    "timeStamp" : ISODate("2017-05-05T07:22:15.461Z"),
                    "message" : "Everything is OK.",
                    "state" : "ok"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-05T07:22:15.392Z"),
                    "_id" : ObjectId("590c2827f0e93fc4136aa5de")
                },
                {
                    "_id" : ObjectId("590c2809f0e93fc4136aa5b8"),
                    "timeStamp" : ISODate("2017-05-05T07:21:45.434Z"),
                    "message" : "Everything is OK.",
                    "state" : "ok"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-05T07:21:45.365Z"),
                    "_id" : ObjectId("590c2809f0e93fc4136aa5b6")
                }
            ],
            "measurements" : [
                {
                    "_id" : ObjectId("590c287bf0e93fc4136aa650"),
                    "timeStamp" : ISODate("2017-05-05T07:23:39.630Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.12,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 17.26
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.02
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:38.798Z"),
                    "_id" : ObjectId("590c287af0e93fc4136aa64d")
                },
                {
                    "_id" : ObjectId("590c2876f0e93fc4136aa649"),
                    "timeStamp" : ISODate("2017-05-05T07:23:34.624Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.09,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 19.13
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.02
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:33.715Z"),
                    "_id" : ObjectId("590c2875f0e93fc4136aa647")
                },
                {
                    "_id" : ObjectId("590c2871f0e93fc4136aa643"),
                    "timeStamp" : ISODate("2017-05-05T07:23:29.583Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.1,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 15.49
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.02
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:28.622Z"),
                    "_id" : ObjectId("590c2870f0e93fc4136aa641")
                },
                {
                    "_id" : ObjectId("590c286cf0e93fc4136aa63d"),
                    "timeStamp" : ISODate("2017-05-05T07:23:24.592Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.1,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 18.2
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.01
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:23.514Z"),
                    "_id" : ObjectId("590c286bf0e93fc4136aa63b")
                },
                {
                    "_id" : ObjectId("590c2867f0e93fc4136aa637"),
                    "timeStamp" : ISODate("2017-05-05T07:23:19.565Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.19,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 13.72
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.01
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:18.466Z"),
                    "_id" : ObjectId("590c2866f0e93fc4136aa636")
                }
            ],
            "alertEmails" : [
                "moqs001@gmail.com"
            ],
            "alertDeviceIds" : [
                "590ad9657baa376c11a40ede"
            ]
        },
        {
            "isEnabled" : false,
            "packageType" : "COMPUTER_SUPERVISOR",
            "packageName" : "shieldPackage",
            "checkIntervalSeconds" : 30,
            "configuration" : {
                "agent_devices" : [
                    "590af7474dbafebc0586b735"
                ],
                "check_thresholds" : {
                    "memory" : {
                        "max_percent" : 95
                    }
                },
                "device_categories" : [],
                "timeout_in_seconds" : 60
            },
            "lastModificationTimeStamp" : "2017-05-04T14:51:40.022Z",
            "watcherServerId" : null,
            "applicationToken" : "EMPTY",
            "_id" : ObjectId("590af86c4dbafebc0586b78e"),
            "instances" : [],
            "access" : [
                {
                    "packageUserEmail" : "moqs001@gmail.com",
                    "canEdit" : true,
                    "canDelete" : true,
                    "canDisable" : true,
                    "canRead" : true,
                    "_id" : ObjectId("590af86c4dbafebc0586b78f")
                }
            ],
            "isDeleted" : false,
            "states" : [
                {
                    "_id" : ObjectId("590b3ffef0e93fc4136a4cf4"),
                    "timeStamp" : ISODate("2017-05-04T14:51:42.977Z"),
                    "message" : "Everything is OK.",
                    "state" : "ok"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-04T14:51:12.974Z"),
                    "_id" : ObjectId("590b3fe0f0e93fc4136a4ce4")
                },
                {
                    "_id" : ObjectId("590b3fc2f0e93fc4136a4cd2"),
                    "timeStamp" : ISODate("2017-05-04T14:50:42.951Z"),
                    "message" : "Everything is OK.",
                    "state" : "ok"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-04T14:50:12.944Z"),
                    "_id" : ObjectId("590b3fa4f0e93fc4136a4cc1")
                },
                {
                    "_id" : ObjectId("590b3f86f0e93fc4136a4cb0"),
                    "timeStamp" : ISODate("2017-05-04T14:49:42.918Z"),
                    "message" : "Everything is OK.",
                    "state" : "ok"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-04T14:49:14.216Z"),
                    "_id" : ObjectId("590b3f6af0e93fc4136a4ca1")
                },
                {
                    "_id" : ObjectId("590b3f5ef0e93fc4136a4c97"),
                    "timeStamp" : ISODate("2017-05-04T14:49:02.897Z"),
                    "message" : "",
                    "state" : "unknown"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-04T14:47:28.689Z"),
                    "_id" : ObjectId("590b3f004dbafebc0586e41f")
                },
                {
                    "_id" : ObjectId("590b3ee24dbafebc0586e40a"),
                    "timeStamp" : ISODate("2017-05-04T14:46:58.639Z"),
                    "message" : "Everything is OK.",
                    "state" : "ok"
                },
                {
                    "state" : "ok",
                    "message" : "Everything is OK.",
                    "timeStamp" : ISODate("2017-05-04T14:46:28.683Z"),
                    "_id" : ObjectId("590b3ec44dbafebc0586e3f3")
                }
            ],
            "measurements" : [
                {
                    "_id" : ObjectId("590b3ff9f0e93fc4136a4cee"),
                    "timeStamp" : ISODate("2017-05-04T14:51:37.295Z"),
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.11
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.5
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    }
                },
                {
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 2.5,
                                "total_gb" : 7.12,
                                "volume" : "/dev/root"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "devtmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0,
                                "total_gb" : 0,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.06,
                                "volume" : "/dev/mmcblk0p1"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 2.84,
                                "total_gb" : 7.12,
                                "volume" : "/"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 0.03,
                            "total_gb" : 0.42
                        },
                        "cpu" : {
                            "value" : 0.09
                        },
                        "agent_id" : "590af7474dbafebc0586b735"
                    },
                    "timeStamp" : ISODate("2017-05-04T14:51:32.128Z"),
                    "_id" : ObjectId("590b3ff4f0e93fc4136a4cec")
                },
                {
                    "_id" : ObjectId("590b3feef0e93fc4136a4cea"),
                    "timeStamp" : ISODate("2017-05-04T14:51:26.854Z"),
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.09
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.5
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    }
                },
                {
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 2.5,
                                "total_gb" : 7.12,
                                "volume" : "/dev/root"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "devtmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0,
                                "total_gb" : 0,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.06,
                                "volume" : "/dev/mmcblk0p1"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 2.84,
                                "total_gb" : 7.12,
                                "volume" : "/"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 0.03,
                            "total_gb" : 0.42
                        },
                        "cpu" : {
                            "value" : 0.11
                        },
                        "agent_id" : "590af7474dbafebc0586b735"
                    },
                    "timeStamp" : ISODate("2017-05-04T14:51:21.743Z"),
                    "_id" : ObjectId("590b3fe9f0e93fc4136a4ce8")
                },
                {
                    "_id" : ObjectId("590b3fe4f0e93fc4136a4ce6"),
                    "timeStamp" : ISODate("2017-05-04T14:51:16.601Z"),
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.08
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.5
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    }
                },
                {
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 2.5,
                                "total_gb" : 7.12,
                                "volume" : "/dev/root"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "devtmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0,
                                "total_gb" : 0,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.06,
                                "volume" : "/dev/mmcblk0p1"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 2.84,
                                "total_gb" : 7.12,
                                "volume" : "/"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 0.03,
                            "total_gb" : 0.42
                        },
                        "cpu" : {
                            "value" : 0.12
                        },
                        "agent_id" : "590af7474dbafebc0586b735"
                    },
                    "timeStamp" : ISODate("2017-05-04T14:51:11.524Z"),
                    "_id" : ObjectId("590b3fdff0e93fc4136a4cdf")
                },
                {
                    "_id" : ObjectId("590b3fdaf0e93fc4136a4cdd"),
                    "timeStamp" : ISODate("2017-05-04T14:51:06.332Z"),
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.1
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.5
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    }
                },
                {
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 2.5,
                                "total_gb" : 7.12,
                                "volume" : "/dev/root"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "devtmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0,
                                "total_gb" : 0,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.06,
                                "volume" : "/dev/mmcblk0p1"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 2.84,
                                "total_gb" : 7.12,
                                "volume" : "/"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 0.03,
                            "total_gb" : 0.42
                        },
                        "cpu" : {
                            "value" : 0.11
                        },
                        "agent_id" : "590af7474dbafebc0586b735"
                    },
                    "timeStamp" : ISODate("2017-05-04T14:51:00.740Z"),
                    "_id" : ObjectId("590b3fd4f0e93fc4136a4cdb")
                },
                {
                    "_id" : ObjectId("590b3fcff0e93fc4136a4cd9"),
                    "timeStamp" : ISODate("2017-05-04T14:50:55.624Z"),
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.09
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.5
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    }
                },
                {
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 2.5,
                                "total_gb" : 7.12,
                                "volume" : "/dev/root"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "devtmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0,
                                "total_gb" : 0,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.21,
                                "total_gb" : 0.21,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.06,
                                "volume" : "/dev/mmcblk0p1"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 0.04,
                                "total_gb" : 0.04,
                                "volume" : "tmpfs"
                            },
                            {
                                "free_gb" : 2.84,
                                "total_gb" : 7.12,
                                "volume" : "/"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 0.03,
                            "total_gb" : 0.42
                        },
                        "cpu" : {
                            "value" : 0.15
                        },
                        "agent_id" : "590af7474dbafebc0586b735"
                    },
                    "timeStamp" : ISODate("2017-05-04T14:50:50.551Z"),
                    "_id" : ObjectId("590b3fcaf0e93fc4136a4cd7")
                }
            ],
            "alertEmails" : [
                "moqs001@gmail.com"
            ],
            "alertDeviceIds" : [
                "590ad9657baa376c11a40ede"
            ]
        },
        {
            "isEnabled" : false,
            "packageType" : "COMPUTER_SUPERVISOR",
            "packageName" : "buldozerOfflinePack",
            "checkIntervalSeconds" : 30,
            "configuration" : {
                "timeout_in_seconds" : 60,
                "device_categories" : [],
                "check_thresholds" : {
                    "memory" : {
                        "max_percent" : 86
                    }
                },
                "agent_devices" : [
                    "590acc0c7baa376c11a40c4f",
                    "590af7474dbafebc0586b735"
                ]
            },
            "lastModificationTimeStamp" : "2017-05-05T07:23:43.647Z",
            "watcherServerId" : null,
            "applicationToken" : "EMPTY",
            "_id" : ObjectId("590c24fef0e93fc4136aa248"),
            "instances" : [],
            "access" : [
                {
                    "packageUserEmail" : "moqs001@gmail.com",
                    "canEdit" : true,
                    "canDelete" : true,
                    "canDisable" : true,
                    "canRead" : true,
                    "_id" : ObjectId("590c24fef0e93fc4136aa249")
                }
            ],
            "isDeleted" : false,
            "states" : [
                {
                    "_id" : ObjectId("590c2881f0e93fc4136aa656"),
                    "timeStamp" : ISODate("2017-05-05T07:23:45.501Z"),
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "timeStamp" : ISODate("2017-05-05T07:23:45.438Z"),
                    "_id" : ObjectId("590c2881f0e93fc4136aa654")
                },
                {
                    "_id" : ObjectId("590c2863f0e93fc4136aa632"),
                    "timeStamp" : ISODate("2017-05-05T07:23:15.406Z"),
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "timeStamp" : ISODate("2017-05-05T07:23:15.347Z"),
                    "_id" : ObjectId("590c2863f0e93fc4136aa630")
                },
                {
                    "_id" : ObjectId("590c2845f0e93fc4136aa608"),
                    "timeStamp" : ISODate("2017-05-05T07:22:45.437Z"),
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "timeStamp" : ISODate("2017-05-05T07:22:45.376Z"),
                    "_id" : ObjectId("590c2845f0e93fc4136aa606")
                },
                {
                    "_id" : ObjectId("590c2827f0e93fc4136aa5df"),
                    "timeStamp" : ISODate("2017-05-05T07:22:15.427Z"),
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "timeStamp" : ISODate("2017-05-05T07:22:15.345Z"),
                    "_id" : ObjectId("590c2827f0e93fc4136aa5dd")
                },
                {
                    "_id" : ObjectId("590c2809f0e93fc4136aa5b7"),
                    "timeStamp" : ISODate("2017-05-05T07:21:45.399Z"),
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: buldozerOfflinePack . Affected Agent: shield . Memory usage is 92.86%, but it should be below 86%.",
                    "timeStamp" : ISODate("2017-05-05T07:21:45.330Z"),
                    "_id" : ObjectId("590c2809f0e93fc4136aa5b5")
                }
            ],
            "measurements" : [
                {
                    "_id" : ObjectId("590c287bf0e93fc4136aa64f"),
                    "timeStamp" : ISODate("2017-05-05T07:23:39.579Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.12,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 17.26
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.02
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:38.839Z"),
                    "_id" : ObjectId("590c287af0e93fc4136aa64e")
                },
                {
                    "_id" : ObjectId("590c2876f0e93fc4136aa64b"),
                    "timeStamp" : ISODate("2017-05-05T07:23:34.685Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.09,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 19.13
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.02
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:33.751Z"),
                    "_id" : ObjectId("590c2875f0e93fc4136aa648")
                },
                {
                    "_id" : ObjectId("590c2871f0e93fc4136aa645"),
                    "timeStamp" : ISODate("2017-05-05T07:23:29.644Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.1,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 15.49
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.02
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:28.654Z"),
                    "_id" : ObjectId("590c2870f0e93fc4136aa642")
                },
                {
                    "_id" : ObjectId("590c286cf0e93fc4136aa63e"),
                    "timeStamp" : ISODate("2017-05-05T07:23:24.643Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.1,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 18.2
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.01
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:23.552Z"),
                    "_id" : ObjectId("590c286bf0e93fc4136aa63c")
                },
                {
                    "_id" : ObjectId("590c2867f0e93fc4136aa639"),
                    "timeStamp" : ISODate("2017-05-05T07:23:19.638Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.87,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.35,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 4.19,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 13.72
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590af7474dbafebc0586b735",
                        "cpu" : {
                            "value" : 0.01
                        },
                        "memory" : {
                            "total_gb" : 0.42,
                            "free_gb" : 0.03
                        },
                        "hdd" : [
                            {
                                "volume" : "/dev/root",
                                "total_gb" : 7.12,
                                "free_gb" : 2.49
                            },
                            {
                                "volume" : "devtmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0,
                                "free_gb" : 0
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.21,
                                "free_gb" : 0.21
                            },
                            {
                                "volume" : "/dev/mmcblk0p1",
                                "total_gb" : 0.06,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "tmpfs",
                                "total_gb" : 0.04,
                                "free_gb" : 0.04
                            },
                            {
                                "volume" : "/",
                                "total_gb" : 7.12,
                                "free_gb" : 2.84
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:23:18.427Z"),
                    "_id" : ObjectId("590c2866f0e93fc4136aa635")
                }
            ],
            "alertEmails" : [
                "moqs001@gmail.com"
            ],
            "alertDeviceIds" : [
                "590ad9657baa376c11a40ede"
            ]
        },
        {
            "isEnabled" : true,
            "packageType" : "COMPUTER_SUPERVISOR",
            "packageName" : "testPlaceholders",
            "checkIntervalSeconds" : 30,
            "configuration" : {
                "timeout_in_seconds" : 60,
                "device_categories" : [],
                "check_thresholds" : {
                    "hdd" : [
                        {
                            "min_free_gb" : 100,
                            "max_percent" : 45,
                            "volume" : "C"
                        }
                    ],
                    "memory" : {
                        "max_percent" : 40,
                        "min_free_gb" : 26
                    },
                    "cpu" : {
                        "max_percent" : 10
                    }
                },
                "agent_devices" : [
                    "590acc0c7baa376c11a40c4f"
                ]
            },
            "lastModificationTimeStamp" : "2017-05-05T07:24:04.675Z",
            "watcherServerId" : "Nxk8MpV9yfEz4A",
            "applicationToken" : "EMPTY",
            "_id" : ObjectId("590c259ff0e93fc4136aa2c1"),
            "instances" : [],
            "access" : [
                {
                    "packageUserEmail" : "moqs001@gmail.com",
                    "canEdit" : true,
                    "canDelete" : true,
                    "canDisable" : true,
                    "canRead" : true,
                    "_id" : ObjectId("590c259ff0e93fc4136aa2c2")
                }
            ],
            "isDeleted" : false,
            "states" : [
                {
                    "_id" : ObjectId("590c2a25f0e93fc4136aa70f"),
                    "timeStamp" : ISODate("2017-05-05T07:30:45.220Z"),
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "timeStamp" : ISODate("2017-05-05T07:30:15.225Z"),
                    "_id" : ObjectId("590c2a07f0e93fc4136aa702")
                },
                {
                    "_id" : ObjectId("590c29e9f0e93fc4136aa6f4"),
                    "timeStamp" : ISODate("2017-05-05T07:29:45.209Z"),
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "timeStamp" : ISODate("2017-05-05T07:29:15.203Z"),
                    "_id" : ObjectId("590c29cbf0e93fc4136aa6e7")
                },
                {
                    "_id" : ObjectId("590c29adf0e93fc4136aa6d9"),
                    "timeStamp" : ISODate("2017-05-05T07:28:45.199Z"),
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "timeStamp" : ISODate("2017-05-05T07:28:15.209Z"),
                    "_id" : ObjectId("590c298ff0e93fc4136aa6cc")
                },
                {
                    "_id" : ObjectId("590c2971f0e93fc4136aa6be"),
                    "timeStamp" : ISODate("2017-05-05T07:27:45.188Z"),
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "timeStamp" : ISODate("2017-05-05T07:27:15.167Z"),
                    "_id" : ObjectId("590c2953f0e93fc4136aa6b1")
                },
                {
                    "_id" : ObjectId("590c2935f0e93fc4136aa6a3"),
                    "timeStamp" : ISODate("2017-05-05T07:26:45.164Z"),
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "state" : "alerting"
                },
                {
                    "state" : "alerting",
                    "message" : "There is a problem with package: testPlaceholders . Affected Agent: buldozer . CPU load is 13.87, but it should be below 10. Memory usage is #MEMORY_USAGE_GB# GB, but it should be below #MEMORY_MAX_USAGE_GB# GB. Memory usage is 75.38%, but it should be below 40%. HDD usage is #HDD_USAGE_GB# GB, but it should be below #HDD_MAX_USAGE_GB# GB on volume C. HDD usage is 80.97 %, but it should be below 45 %.",
                    "timeStamp" : ISODate("2017-05-05T07:26:15.171Z"),
                    "_id" : ObjectId("590c2917f0e93fc4136aa696")
                }
            ],
            "measurements" : [
                {
                    "_id" : ObjectId("590c2a25f0e93fc4136aa710"),
                    "timeStamp" : ISODate("2017-05-05T07:30:45.924Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.86,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.25,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 3.79,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 14.03
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590acc0c7baa376c11a40c4f",
                        "cpu" : {
                            "value" : 13.02
                        },
                        "memory" : {
                            "total_gb" : 15.88,
                            "free_gb" : 3.44
                        },
                        "hdd" : [
                            {
                                "volume" : "C",
                                "total_gb" : 146.48,
                                "free_gb" : 27.86
                            },
                            {
                                "volume" : "D",
                                "total_gb" : 596.17,
                                "free_gb" : 418.34
                            },
                            {
                                "volume" : "E",
                                "total_gb" : 2794.39,
                                "free_gb" : 1334.68
                            },
                            {
                                "volume" : "F",
                                "total_gb" : 302.67,
                                "free_gb" : 95.25
                            },
                            {
                                "volume" : "G",
                                "total_gb" : 1862.92,
                                "free_gb" : 325.78
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:30:40.921Z"),
                    "_id" : ObjectId("590c2a20f0e93fc4136aa70d")
                },
                {
                    "_id" : ObjectId("590c2a1bf0e93fc4136aa70b"),
                    "timeStamp" : ISODate("2017-05-05T07:30:35.912Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.86,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.25,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 3.44,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 13.17
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590acc0c7baa376c11a40c4f",
                        "cpu" : {
                            "value" : 15.31
                        },
                        "memory" : {
                            "total_gb" : 15.88,
                            "free_gb" : 3.44
                        },
                        "hdd" : [
                            {
                                "volume" : "C",
                                "total_gb" : 146.48,
                                "free_gb" : 27.86
                            },
                            {
                                "volume" : "D",
                                "total_gb" : 596.17,
                                "free_gb" : 418.34
                            },
                            {
                                "volume" : "E",
                                "total_gb" : 2794.39,
                                "free_gb" : 1334.68
                            },
                            {
                                "volume" : "F",
                                "total_gb" : 302.67,
                                "free_gb" : 95.25
                            },
                            {
                                "volume" : "G",
                                "total_gb" : 1862.92,
                                "free_gb" : 325.78
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:30:30.935Z"),
                    "_id" : ObjectId("590c2a16f0e93fc4136aa709")
                },
                {
                    "_id" : ObjectId("590c2a11f0e93fc4136aa707"),
                    "timeStamp" : ISODate("2017-05-05T07:30:25.895Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.86,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.26,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 3.47,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 13.45
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590acc0c7baa376c11a40c4f",
                        "cpu" : {
                            "value" : 14.26
                        },
                        "memory" : {
                            "total_gb" : 15.88,
                            "free_gb" : 3.48
                        },
                        "hdd" : [
                            {
                                "volume" : "C",
                                "total_gb" : 146.48,
                                "free_gb" : 27.86
                            },
                            {
                                "volume" : "D",
                                "total_gb" : 596.17,
                                "free_gb" : 418.34
                            },
                            {
                                "volume" : "E",
                                "total_gb" : 2794.39,
                                "free_gb" : 1334.68
                            },
                            {
                                "volume" : "F",
                                "total_gb" : 302.67,
                                "free_gb" : 95.26
                            },
                            {
                                "volume" : "G",
                                "total_gb" : 1862.92,
                                "free_gb" : 325.78
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:30:20.898Z"),
                    "_id" : ObjectId("590c2a0cf0e93fc4136aa705")
                },
                {
                    "_id" : ObjectId("590c2a07f0e93fc4136aa703"),
                    "timeStamp" : ISODate("2017-05-05T07:30:15.888Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.86,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.26,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 3.48,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 14.11
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590acc0c7baa376c11a40c4f",
                        "cpu" : {
                            "value" : 13.97
                        },
                        "memory" : {
                            "total_gb" : 15.88,
                            "free_gb" : 3.48
                        },
                        "hdd" : [
                            {
                                "volume" : "C",
                                "total_gb" : 146.48,
                                "free_gb" : 27.86
                            },
                            {
                                "volume" : "D",
                                "total_gb" : 596.17,
                                "free_gb" : 418.34
                            },
                            {
                                "volume" : "E",
                                "total_gb" : 2794.39,
                                "free_gb" : 1334.68
                            },
                            {
                                "volume" : "F",
                                "total_gb" : 302.67,
                                "free_gb" : 95.26
                            },
                            {
                                "volume" : "G",
                                "total_gb" : 1862.92,
                                "free_gb" : 325.78
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:30:10.860Z"),
                    "_id" : ObjectId("590c2a02f0e93fc4136aa700")
                },
                {
                    "_id" : ObjectId("590c29fdf0e93fc4136aa6fe"),
                    "timeStamp" : ISODate("2017-05-05T07:30:05.866Z"),
                    "data" : {
                        "hdd" : [
                            {
                                "free_gb" : 27.86,
                                "total_gb" : 146.48,
                                "volume" : "C"
                            },
                            {
                                "free_gb" : 418.34,
                                "total_gb" : 596.17,
                                "volume" : "D"
                            },
                            {
                                "free_gb" : 1334.68,
                                "total_gb" : 2794.39,
                                "volume" : "E"
                            },
                            {
                                "free_gb" : 95.26,
                                "total_gb" : 302.67,
                                "volume" : "F"
                            },
                            {
                                "free_gb" : 325.78,
                                "total_gb" : 1862.92,
                                "volume" : "G"
                            }
                        ],
                        "memory" : {
                            "free_gb" : 3.48,
                            "total_gb" : 15.88
                        },
                        "cpu" : {
                            "value" : 13.64
                        },
                        "agent_id" : "590acc0c7baa376c11a40c4f"
                    }
                },
                {
                    "data" : {
                        "agent_id" : "590acc0c7baa376c11a40c4f",
                        "cpu" : {
                            "value" : 13.72
                        },
                        "memory" : {
                            "total_gb" : 15.88,
                            "free_gb" : 3.48
                        },
                        "hdd" : [
                            {
                                "volume" : "C",
                                "total_gb" : 146.48,
                                "free_gb" : 27.86
                            },
                            {
                                "volume" : "D",
                                "total_gb" : 596.17,
                                "free_gb" : 418.34
                            },
                            {
                                "volume" : "E",
                                "total_gb" : 2794.39,
                                "free_gb" : 1334.68
                            },
                            {
                                "volume" : "F",
                                "total_gb" : 302.67,
                                "free_gb" : 95.26
                            },
                            {
                                "volume" : "G",
                                "total_gb" : 1862.92,
                                "free_gb" : 325.78
                            }
                        ]
                    },
                    "timeStamp" : ISODate("2017-05-05T07:30:00.851Z"),
                    "_id" : ObjectId("590c29f8f0e93fc4136aa6fc")
                }
            ],
            "alertEmails" : [
                "moqs001@gmail.com"
            ],
            "alertDeviceIds" : [
                "590ad9657baa376c11a40ede"
            ]
        }
    ],
    "browserSessions" : [
        {
            "token" : "9a8a47639fc7e680029535ed95ce3bb2f0fe28ca5ce32cdd0ebf8b8c723dc4a5b2a5a7dfad15588ca99bce62603f837f667d4fe83dea8f8ecb5752bb1b87084e",
            "tokenTimeStamp" : ISODate("2017-05-05T07:30:48.534Z"),
            "_id" : ObjectId("590b3fc5f0e93fc4136a4cd5")
        },
        {
            "token" : "45b9d766cef9885f29bf93880664d4f2516a6395fe2f4ba0462a903465f0f388418e92e25dfd9efecaecba82ef3d9a5fa9abba628fd140a2111cfef7eabf658a",
            "tokenTimeStamp" : ISODate("2017-05-05T07:30:35.803Z"),
            "_id" : ObjectId("590c1f11f0e93fc4136a9f86")
        }
    ],
    "activationInfo" : {
        "activationCode" : "l34jLVFpQPLawBVwn59rVe8tns0",
        "expiryDate" : ISODate("2017-07-02T22:08:51.933Z"),
        "isActivated" : true
    },
    "language" : "ENGLISH",
    "createdOn" : ISODate("2017-05-03T22:08:51.945Z"),
    "__v" : 179
};
