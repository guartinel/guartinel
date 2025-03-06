using System;
using System.ComponentModel.DataAnnotations ;
using System.Linq;
using Guartinel.Website.Common.Error ;
using Newtonsoft.Json;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.Admin.Models {
    public class AdminTokenModel {
        [JsonProperty(PropertyName = AllParameters.TOKEN)]
        [Required(ErrorMessage = ErrorMessages.TOKEN_REQUIRED)]
        public string Token { get; set; }
        }
    }