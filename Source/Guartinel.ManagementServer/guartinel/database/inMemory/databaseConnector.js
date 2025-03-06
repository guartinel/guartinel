/**
 * Created by DTAP on 2017.08.22..
 */
var redis = require('redis');
var client = redis.createClient({ enable_offline_queue: false, max_attempts: 99999, retry_max_delay: 30000 });

exports.getConnectionState = function () {
   function result(success, state) {
      this.success = success;
      this.state = state;
   }
   if (isReady) {
      return new result(true, "Connected");
   }
   return new result(false, "Disconnected");
}
var isReady = false;
client.on('connect', function () {
   LOG.info("Redis. Client connected.")
});
client.on('error', function (err) {
   isReady = false;
   LOG.error("Redis. There was an error in redis connection " + err);
});
client.on('ready', function () {
   isReady = true;
   LOG.error("Redis. Connection is ready!");
});

exports.isKeyExists = function (key, onResult) {
   if (!isReady) {
      return onResult(false);
   }
   client.exists(key, function (err, reply) {
      if (err) {
         var error = new MSInternalServerError()
            .logMessage("Redis.isKeyExists Cannot get key due error:")
            .severe()
            .innerError(err)
            .logNow();	
      }
      if (reply === 1) {
         return onResult(true);
      }
      return onResult(false);
   });
};

exports.addKey = function (key, expirySec, onFinish) {
   if (!isReady) {
      return onFinish();
   }
   client.setex(key, expirySec, key, function (err, result) {
      if (err) {
         LOG.error("Redis.addKey Cannot add key due error: " + err);
      }
      return onFinish();
   });
};