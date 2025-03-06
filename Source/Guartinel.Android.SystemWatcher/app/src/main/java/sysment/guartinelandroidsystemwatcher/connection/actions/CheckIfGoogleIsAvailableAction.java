package sysment.guartinelandroidsystemwatcher.connection.actions;

import java.io.IOException;

import sysment.guartinelandroidsystemwatcher.util.LogWrapper;
import sysment.guartinelandroidsystemwatcher.connection.HTTPInterface;

/**
 * Created by sysment_dev on 11/18/2016.
 */
public class CheckIfGoogleIsAvailableAction extends BaseAction {

    private String URL;

    public CheckIfGoogleIsAvailableAction(HTTPInterface httpInterface) {
        super(httpInterface);
        this.URL = "http://google.com";
          configureRequestBuilder();
        buildRequest();

    }

    @Override
    public void execute() {
        try {
            HTTPInterface.StructuredResponse result = _interface.executeRequest(_request);
            _isSuccess = true;
            LogWrapper.Inf("CheckIfGoogleIsAvailableAction success" + _isSuccess);
        } catch (IOException e) {
            LogWrapper.Err("Cannot execute CheckIfGoogleIsAvailableAction. E:",e);

        }
    }

    @Override
    public boolean isSuccessful() {
        return _isSuccess;
    }

    @Override
    protected void configureRequestBuilder() {
        _requestBuilder.url(URL);
        _requestBuilder.get();
    }
}
