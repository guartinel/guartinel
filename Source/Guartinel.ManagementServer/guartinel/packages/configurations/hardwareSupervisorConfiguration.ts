import { PluginPackageConfigurationBase } from "./pluginPackageConfiguration";
import { MSError } from "../../../error/Errors";
import { Const } from "../../../common/constants";
import * as securityTool from "../../security/tool";
import { LOG } from "../../../diagnostics/LoggerFactory";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;


export class HardwareSupervisorConfiguration extends PluginPackageConfigurationBase {
    hardware_token: string;
    instances = [];

    constructor() {
        super();
    }

    initFromObject(object: any): MSError {
        this.instances = object.instances;
        return null;
    }

    createFromJSON(json: any): MSError {
        // instances are not obligatory so we wont raise error if it is null
        if (!utils.object.isNull(json[traceIfNull(Const.pluginConstants.INSTANCES)])) {
            this.instances = json[Const.pluginConstants.INSTANCES];
        }

        return null; // everything is OK!
    }

    updateFromJSON(json: any): MSError {
        if (!utils.object.isNull(json[traceIfNull(Const.pluginConstants.INSTANCES)])) {
            this.instances = json[Const.pluginConstants.INSTANCES];
        }

        return null; // everything is OK!
    }

    getInstance(instanceId) {
        for (let instanceIndex = 0; instanceIndex < this.instances.length; instanceIndex++) {
            if (this.instances[instanceIndex].instance_id === instanceId) {
                return this.instances[instanceIndex];
            }
        }
        return null;
    }

    addInstance(instanceId, instanceName, type) {
        this.instances.push({
            name: instanceName,
            instance_id: instanceId,
            hardware_type: type,
        });
    }

    getPackagePartCount(): number {
        if (utils.object.isNull(this.instances)) {
            return 0;
        }
        return this.instances.length;
    }
    maskSensitiveInfo() { }

}