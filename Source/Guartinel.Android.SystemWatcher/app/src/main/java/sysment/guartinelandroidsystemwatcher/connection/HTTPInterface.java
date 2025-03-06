package sysment.guartinelandroidsystemwatcher.connection;

import java.io.IOException;

import okhttp3.Headers;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import okio.Buffer;
import sysment.guartinelandroidsystemwatcher.util.LogWrapper;

/**
 * Created by sysment_dev on 11/03/2016.
 */
public class HTTPInterface {
    private OkHttpClient _client;

    public HTTPInterface() {
        _client = new OkHttpClient();
    }

    public class StructuredResponse {
        public String body;
        public Headers headers;
        public String url;
        public int code;
        public Response response;

        public StructuredResponse(String url, String body, Headers headers, int code, Response response) {
            this.body = body;
            this.headers = headers;
            this.code = code;
            this.url = url;
            this.response = response;
        }
    }

    public StructuredResponse executeRequest(Request request) throws IOException {
      //  printRequestInfoToLog(request);
        Response response = _client.newCall(request).execute();

        StructuredResponse structuredResponse = new StructuredResponse(request.url().query(), response.body().string(), response.headers(), response.code(),response);
       // printResponseInfoToLog(structuredResponse);
        return structuredResponse;
    }


    private void printRequestInfoToLog(Request request) {
        try {
            LogWrapper.Inf("REQUEST-URL:" + request.url());

            Headers requestHeaders = request.headers();
            LogWrapper.Inf("HEADERS:");
            for (int i = 0, size = requestHeaders.size(); i < size; i++) {
                String header = requestHeaders.name(i) + ": " + requestHeaders.value(i);
                LogWrapper.Inf(header);
            }

            LogWrapper.Inf("REQUEST-BODY:");

            if (request.body() == null) {
                LogWrapper.Inf("Empty request body.");
                return;
            }
            final Buffer buffer = new Buffer();
            request.body().writeTo(buffer);
            LogWrapper.Inf(buffer.readUtf8());
        } catch (IOException e) {
            LogWrapper.Inf("Cannot print request informations. Exception :" + e.getMessage());
        }
    }

    private void printResponseInfoToLog(StructuredResponse response) {
        try {
            LogWrapper.Inf("RESPONSE-URL: " + response.url);

            LogWrapper.Inf("RESPONSE STATUS CODE:" + response.code);

            Headers responseHeaders = response.headers;
            LogWrapper.Inf("HEADERS:");
            for (int i = 0, size = responseHeaders.size(); i < size; i++) {
                String header = responseHeaders.name(i) + ": " + responseHeaders.value(i);
                LogWrapper.Inf(header);
            }

            LogWrapper.Inf("RESPONSE-BODY:");
            LogWrapper.Inf(response.body);
        } catch (Exception e) {
            LogWrapper.Inf("Cannot print response informations. Exception :" + e.getMessage());
        }
    }

}
