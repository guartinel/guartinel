using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Guartinel.Website.User.Misc;
using Guartinel.Website.User.Models.Account;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.User.Models.Package
{
    public class PackageModel : AuthenticationModel
    {
        [JsonProperty(PropertyName = AllParameters.PACKAGE_ID)]
        public string PackageID { get; set; }

        [JsonProperty(PropertyName = AllParameters.PACKAGE_NAME)]
        [Required(ErrorMessage = ErrorMessages.PACKAGE_NAME_REQUIRED)]
        public string PackageName { get; set; }

        [JsonProperty(PropertyName = AllParameters.PACKAGE_TYPE)]
        [Required]
        public string PackageType { get; set; }

        [JsonProperty(PropertyName = AllParameters.IS_ENABLED)]
        [Required]
        public bool IsEnabled { get; set; }

        [JsonProperty(PropertyName = AllParameters.CONFIGURATION)]
        [Required(ErrorMessage = "Missing property: " + AllParameters.CONFIGURATION)]
        public JObject Configuration { get; set; }

        [JsonProperty(PropertyName = AllParameters.CHECK_INTERVAL_SECONDS)]
        [Required(ErrorMessage = "Missing property: " + AllParameters.CHECK_INTERVAL_SECONDS)]
        public int CheckIntervalSeconds { get; set; }


        [JsonProperty(PropertyName = AllParameters.TIMEOUT_INTERVAL_SECONDS)]
        public int? TimeoutIntervalSeconds { get; set; }

        [JsonProperty(PropertyName = AllParameters.ALERT_EMAILS)]
        public string[] AlertEmails { get; set; }

        [JsonProperty(PropertyName = AllParameters.ALERT_DEVICE_IDS)]
        public string[] AlertDeviceIds { get; set; }

        [JsonProperty(PropertyName = AllParameters.ACCESS)]
        public JObject[] Access { get; set; }

        [JsonProperty(PropertyName = AllParameters.DISABLE_ALERTS)]
        public JObject DisableAlerts { get; set; }

        [JsonProperty(PropertyName = AllParameters.USE_PLAIN_ALERT_EMAIL)]
        public bool UsePlainAlertEmail { get; set; }

       [JsonProperty(PropertyName = AllParameters.FORCED_DEVICE_ALERT)]
       public bool ForcedDeviceAlert { get; set; }

   }
}
