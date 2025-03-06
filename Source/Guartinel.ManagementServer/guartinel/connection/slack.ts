/*const { WebClient } = require('@slack/client');
import { LOG } from "../../diagnostics/LoggerFactory";
import { MSInternalServerError, MSError } from "../../error/Errors";
// An access token (from your Slack app or custom integration - xoxp, xoxb, or xoxa)
const token = process.env.SLACK_TOKEN;

let slackWebClient;


export function configure(token) {
    slackWebClient = new WebClient(token);
}

export function send(channelID, alertMessage, pack, account, onFinished) {
    slackWebClient.chat.postMessage({ channel: channelID, alertMessage })
        .then((res) => {
            LOG.info("Message " + alertMessage + " sent to channel: " + channelID + " Result: " + res.ts);
            return onFinished();
        })
        .catch((err) => {
            return onFinished("Could not send message " + alertMessage + " to : " + channelID + err)
        });
}


*/