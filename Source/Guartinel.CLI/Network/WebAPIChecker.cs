using Guartinel.CLI.ResultSending;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using Guartinel.Kernel.Utility;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Guartinel.Kernel.Logging;
using System.Threading.Tasks;

namespace Guartinel.CLI.Network
{
    class WebAPIChecker : SendResultCommandBase
    {
        public override string Description => $"Checks an API endpoint availability and response.";

        public override string Command => "checkAPI";

        public new static class Constants
        {
            public static class Parameters
            {
                public const string URL = "url";
                public const string HTTP_METHOD = "httpMethod";
                public const string REQUEST_BODY = "requestBody";
                public const string REQUEST_BODY_CONTENT_TYPE = "requestBodyContentType";
                public const string EXPECTED_RESPONSE_CODE = "expectedResponseCode";
                public const string EXPECTED_RESPONSE = "expectedResponse";
                public const string INCLUDE_RESPONSE_IN_CHECK_RESULT = "includeResponseInCheckResult";
            }

            public static class Defaults
            {
                public const int EXPECTED_RESPONSE_CODE = 200;
                public const bool INCLUDE_RESPONSE_IN_CHECK_RESULT = true;
                public const string REQUEST_BODY_CONTENT_TYPE = ContentTypes.TEXT_PLAIN;
                public const int TIMEOUT_SECONDS = 30;
            }

            public static class Results
            {
                public const string URL = "url";
                public const string API_RESPONSE = "apiResponse";
                public const string RESPONSE_CODE = "responseCode";

            }
            public static class ContentTypes
            {
                public const string TEXT_PLAIN = "text/plain";
                public const string APPLICATION_JSON = "application/json";
                public const string APPLICATION_JAVASCRIPT = "application/javascript";
                public const string APPLICATION_XML = "application/xml";
                public const string TEXT_XML = "text/xml";
                public const string TEXT_HTML = "text/html";
                public const string FORM_DATA = "form-data";
                public const string X_WWW_FORM_URLENCODED = "x-www-form-urlencoded";
            }
            public static class HTTPMethods
            {
                public const string POST = "POST";
                public const string PUT = "PUT";
                public const string GET = "GET";
            }
        }

        public string URL => Parameters.GetStringValue(Constants.Parameters.URL, string.Empty);
        public string HTTPMethod => Parameters.GetStringValue(Constants.Parameters.HTTP_METHOD, string.Empty);
        public string RequestBody => Parameters.GetStringValue(Constants.Parameters.REQUEST_BODY, string.Empty);
        public string RequestBodyContentType => Parameters.GetStringValue(Constants.Parameters.REQUEST_BODY_CONTENT_TYPE, string.Empty);
        public int ExpectedResponseCode => Parameters.GetIntegerValue(Constants.Parameters.EXPECTED_RESPONSE_CODE, Constants.Defaults.EXPECTED_RESPONSE_CODE);
        public string ExpectedResponse => Parameters.GetStringValue(Constants.Parameters.EXPECTED_RESPONSE, string.Empty);
        public bool IncludeResponseInCheckResult => Parameters.GetBooleanValue(Constants.Parameters.INCLUDE_RESPONSE_IN_CHECK_RESULT, Constants.Defaults.INCLUDE_RESPONSE_IN_CHECK_RESULT);


        protected override List<CheckResult> Run2()
        {

         /*   using (HttpClient _httpClient = new HttpClient())
            {
                var exceptionMessage = string.Empty;

                StringContent httpContent = new StringContent(RequestBody, Encoding.UTF8, RequestBodyContentType);
                try
                {
                    Task<HttpResponseMessage> postResponse = null;
                    if (HTTPMethod.Equals(Constants.HTTPMethods.GET))
                    {
                        postResponse = _httpClient.GetAsync($@"{URL}");
                    }
                    if (HTTPMethod.Equals(Constants.HTTPMethods.POST))
                    {
                        postResponse = _httpClient.PostAsync($@"{URL}", httpContent);
                    }
                    if (HTTPMethod.Equals(Constants.HTTPMethods.PUT))
                    {
                        postResponse = _httpClient.PutAsync($@"{URL}", httpContent);
                    }

                    postResponse.Wait(TimeSpan.FromSeconds(Constants.Defaults.TIMEOUT_SECONDS));

                    if (!postResponse.Result.IsSuccessStatusCode)
                    {
                        throw new Exception(postResponse.Result.ReasonPhrase);
                    }

                    using (var response = postResponse.Result.Content.ReadAsStringAsync())
                    {
                        response.Wait(TimeSpan.FromSeconds(Constants.Defaults.TIMEOUT_SECONDS));

                        // Logger.Log($"Post returned from Management Server. {response.Result}") ;

                        if (string.IsNullOrEmpty(response.Result))
                        {
                            return new JObject();
                        }

                        if (response.Result.StartsWith(@"<!DOCTYPE html>", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return new JObject();
                        }

                        return JObject.Parse(response.Result);
                    }


                }
                catch (Exception e)
                {
                    Logger.Error($@"Error when posting request to {URL}. Values: {values}. Error: {e.GetAllMessages()}");

                    exceptionMessage = e.GetAllMessages();
                }

            }








            CheckResult result;

            var data = new JObject();
            data[Constants.Results.URL] = URL;


            if (true)
            {
                result = new CheckResult(true, $@"Size of {includePatternString} in folder '{FolderName}' is {sizeInUnit} {maxSizeUnit}.", data);
            }
            else
            {
                result = new CheckResult(false, $@"Size of {includePatternString} in folder '{FolderName}' is {sizeInUnit} {maxSizeUnit}, greater than {MaxSize} {maxSizeUnit}.", data);
            }

            Logger.Log(LogLevel.Info, $"Folder size check. Pattern: {includePatternString}, size: {sizeInUnit} {maxSizeUnit}, max size: {MaxSize} {maxSizeUnit}. Result: {sizeIsOK}");

            */return new List<CheckResult> { null };
        }

        public class WebApiCheckerCl : SendResultCommandBaseCl<WebAPIChecker>
        {
            protected override void Setup2(CommandLineApplication commandLineParser)
            {
                SetupOption(commandLineParser, WebAPIChecker.Constants.Parameters.URL, "API endpoint to call. e.g.: https://mysite.com:1234/api/test");
                SetupOption(commandLineParser, WebAPIChecker.Constants.Parameters.HTTP_METHOD, "HTTP method to use. Possible values: " + GetAllFieldValuesConcatenated(typeof(WebAPIChecker.Constants.HTTPMethods)));
                SetupOption(commandLineParser, WebAPIChecker.Constants.Parameters.REQUEST_BODY_CONTENT_TYPE, "The type of the request body. Possible values: " + GetAllFieldValuesConcatenated(typeof(WebAPIChecker.Constants.ContentTypes)));
                SetupOption(commandLineParser, WebAPIChecker.Constants.Parameters.REQUEST_BODY, "The body of the request.");
                SetupOption(commandLineParser, WebAPIChecker.Constants.Parameters.EXPECTED_RESPONSE, "The expected response code from the API e.g.: 200.");
                SetupOption(commandLineParser, WebAPIChecker.Constants.Parameters.INCLUDE_RESPONSE_IN_CHECK_RESULT, "If your api response contains sensitive data, than set this flag to false to prevent any data leak.");
            }

            public string GetAllFieldValuesConcatenated(Type classType)
            {
                string result = "";
                FieldInfo[] fields = classType.GetFields(BindingFlags.Static | BindingFlags.Public);
                for (int i = 0; i < fields.Length; i++)
                {
                    result += fields[i].GetValue(null).ToString();
                    if (i != fields.Length)
                    {
                        result += ", ";
                    }
                }
                return result;
            }
        }
    }
}
