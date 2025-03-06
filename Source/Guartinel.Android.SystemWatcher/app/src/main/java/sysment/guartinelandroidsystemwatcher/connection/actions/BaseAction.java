package sysment.guartinelandroidsystemwatcher.connection.actions;

import okhttp3.Request;
import okhttp3.Response;
import sysment.guartinelandroidsystemwatcher.connection.HTTPInterface;

/**
 * Created by sysment_dev on 11/03/2016.
 */
public abstract class BaseAction {
    protected HTTPInterface _interface;
    protected Request _request;
    protected Request.Builder _requestBuilder;
    protected Response _response;
    protected boolean _isSuccess = false;


    public BaseAction(HTTPInterface httpInterface) {
        _interface = httpInterface;
        _requestBuilder = new Request.Builder();
    }

    public abstract void execute();


    public abstract boolean isSuccessful();

    protected abstract void configureRequestBuilder();

    protected  void buildRequest() {
        _requestBuilder.addHeader("Accept-Language", "hu,en;q=0.8")
                .addHeader("Cache-Control", "max-age=0")
                .addHeader("Connection", "keep-alive")
               // .addHeader("Host", "www.mydcfxbroker.com")
                .addHeader("Upgrade-Insecure-Requests", "1")
                .addHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36");
        _request = _requestBuilder.build();
    }
}
//LoginAction =  new Actions.Login(_httpInterface,"teve","pata");
//if (
//
//