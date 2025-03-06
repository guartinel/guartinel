package sysment.android.guartinel.ui.main.fragments;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.DownloadManager;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.location.LocationManager;
import android.net.Uri;
import android.net.http.SslError;
import android.os.Bundle;
import android.os.Environment;
import android.support.annotation.Nullable;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AlertDialog;
import android.support.v7.view.ContextThemeWrapper;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import android.webkit.CookieManager;
import android.webkit.DownloadListener;
import android.webkit.JavascriptInterface;
import android.webkit.SslErrorHandler;
import android.webkit.WebResourceError;
import android.webkit.WebResourceRequest;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.RelativeLayout;
import android.widget.Toast;

import java.util.Calendar;
import java.util.Date;

import sysment.android.guartinel.BuildConfig;
import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.R;
import sysment.android.guartinel.core.utils.LOG;
import sysment.android.guartinel.core.network.WifiUtility;
import sysment.android.guartinel.ui.main.MainActivity;
import sysment.android.guartinel.ui.main.fragments.configureSensor.ConfigureDevicesDialogFragment;

import static android.content.Context.DOWNLOAD_SERVICE;
import static android.content.Context.LOCATION_SERVICE;

/**
 * Created by sysment_dev on 03/07/2017.
 */
public class ManageAccountFragment extends Fragment {
    public static final String TAG = "ManageAccount";
    private WebView mWebView;
    private RelativeLayout loadingPanel;

    public ManageAccountFragment() {
        setRetainInstance(true);
    }

    public void goBack() {
        if(mWebView == null){
            return;
        }
        mWebView.post(new Runnable() {
            @Override
            public void run() {
                mWebView.loadUrl("javascript:angular.element(document.getElementById(\"userPageMenu\")).scope().onBackButtonPressed()");
            }
        });
    }

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.manage_account_fragment, container, false);
      /* if(mWebView != null && loadingPanel != null &&  isWebViewConfigured){
           return view;
       }*/
        mWebView = (WebView) view.findViewById(R.id.manageAccountWebView);
        loadingPanel = (RelativeLayout) view.findViewById(R.id.loadingPanel);
        setLoadingInProgress();
        setupWebView();
        return view;
    }

    public class GuartinelWebAppInterface {
        Context context;

        public GuartinelWebAppInterface(Context context) {
            this.context = context;
        }

        @JavascriptInterface
        public void addNewHardwareSensor() {
            mWebView.post(new Runnable() {
                @Override
                public void run() {
                    LocationManager locationManager = (LocationManager) getActivity().getSystemService(LOCATION_SERVICE);

                    if (!locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER)) {
                        Toast.makeText(getActivity(), "Please enable gps so we can find accesspoints arround your device.", Toast.LENGTH_SHORT).show();
                        return;
                    }
                    FragmentTransaction ft = getFragmentManager().beginTransaction();
                    Fragment prev = getFragmentManager().findFragmentByTag("dialog");
                    if (prev != null) {
                        ft.remove(prev);
                    }
                    ft.addToBackStack(null);
                    WifiUtility.saveCurrentWifiSSID(getActivity());
                    ConfigureDevicesDialogFragment dialogFragment = new ConfigureDevicesDialogFragment();
                    dialogFragment.show(ft, "dialog");
                }
            });
        }


        @JavascriptInterface
        public void onWebAppLoaded() {
            mWebView.post(new Runnable() {
                @Override
                public void run() {
                    //   mWebView.loadUrl("javascript:angular.element(document.getElementById(\"loginHelper\")).scope()._deviceLogin(\"" + GuartinelApp.getDataStore().getEmail() + "\",\"" + GuartinelApp.getDataStore().getUserPassword() + "\")");
                }
            });
        }
    }

    @Override
    public void onPause() {
        if (mWebView != null) {
            mWebView.onPause();
        }
        super.onPause();
    }

    @Override
    public void onResume() {
        if (mWebView != null) {
            mWebView.onResume();
        }
        super.onResume();
    }

    private boolean isWebViewConfigured = false;
    private String _lastDownloadURL;
    Context context;

    private void setupWebView() {
        mWebView.setWebViewClient(new WebViewClient() {
            public void onPageStarted(WebView view, String url, Bitmap favicon) {
                // setLoadingFinished();
            }

            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                view.loadUrl(url);
                return true;
            }

            public void onPageFinished(WebView view, String url) {
                setLoadingFinished();
            }

            @Override
            public void onReceivedError(WebView view, WebResourceRequest request, WebResourceError error) {
                super.onReceivedError(view, request, error);
                setLoadingFinished();
            }


            public void onReceivedError(WebView view, int errorCode, String description, String failingUrl) {
                setLoadingFinished();
            }

            @Override
            public void onReceivedSslError(WebView view, final SslErrorHandler handler, SslError error) {
                @SuppressLint("RestrictedApi") AlertDialog.Builder builder = new AlertDialog.Builder(new ContextThemeWrapper(getActivity(), R.style.AlertDialogCustom)); //alert for confirm to delete
                builder.setMessage("There is a problem with the SSL certificate. Do you want to continue?");    //set message

                builder.setPositiveButton("Continue", new DialogInterface.OnClickListener() { //when click on DELETE
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        handler.proceed();
                        return;
                    }
                }).setNegativeButton("Stop", new DialogInterface.OnClickListener() {  //not removing items if cancel is done
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        handler.cancel();
                        return;
                    }
                }).setCancelable(false).show();  //show alert dialog
            }
        });
        String cookieString = "userToken=%22" + GuartinelApp.getDataStore().getToken() + "%22; path=/";
        CookieManager.getInstance().setCookie(BuildConfig.WEBSITE_ADDRESS, cookieString);
        mWebView.getSettings().setAppCacheEnabled(true);
        mWebView.getSettings().setJavaScriptEnabled(true);
        mWebView.getSettings().setDomStorageEnabled(true);
        mWebView.setLayerType(View.LAYER_TYPE_HARDWARE, null);
        mWebView.addJavascriptInterface(new GuartinelWebAppInterface(getContext()), "android");
        mWebView.loadUrl(BuildConfig.WEBSITE_ADDRESS);
        context = getContext();
        final Activity activity = getActivity();
        mWebView.setDownloadListener(new DownloadListener() {
            public void onDownloadStart(String url, String userAgent,
                                        String contentDisposition, String mimetype,
                                        long contentLength) {
                if (!doWeHaveEnoughPermission(context)) {
                    _lastDownloadURL = url;
                    askPermission(context, activity);
                    return;
                }
                downloadURL(url);
            }
        });
        isWebViewConfigured = true;
    }

    private void downloadURL(String url) {
        url = url.replace("blob:", "");
        DownloadManager.Request request = new DownloadManager.Request(
                Uri.parse(url));
        String fileName = "download";
        if (url.contains("GDPR")) {
            fileName = "GuartinelPrivacyGDPR.pdf";
        }else{
            fileName = "GuartinelPackageStatistics.csv";
        }

        request.allowScanningByMediaScanner();
        request.setNotificationVisibility(DownloadManager.Request.VISIBILITY_VISIBLE_NOTIFY_COMPLETED); //Notify client once download is completed!
        request.setDestinationInExternalPublicDir(Environment.DIRECTORY_DOWNLOADS, fileName);

        DownloadManager dm = (DownloadManager) context.getSystemService(DOWNLOAD_SERVICE);
        dm.enqueue(request);
        Toast.makeText(context, "Downloading File", //To notify the Client that the file is being downloaded
                Toast.LENGTH_LONG).show();
    }

    public void retryDownload() {
        if (!doWeHaveEnoughPermission(context)) {
            askPermission(context, getActivity());
            return;
        }
        downloadURL(_lastDownloadURL);
    }

    private boolean doWeHaveEnoughPermission(Context context) {
        if (ContextCompat.checkSelfPermission(context, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
            return false;
        }
        return true;
    }


    private void askPermission(Context context, final Activity activity) {
        if (ActivityCompat.shouldShowRequestPermissionRationale(activity,
                Manifest.permission.ACCESS_FINE_LOCATION)) {
            new AlertDialog.Builder(new ContextThemeWrapper(context, R.style.AlertDialogCustom))
                    .setTitle("Please provide external storage write permission")
                    .setMessage("This is needed to save the file.")
                    .setPositiveButton("ok", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialogInterface, int i) {
                            //Prompt the user once explanation has been shown
                            ActivityCompat.requestPermissions(activity,
                                    new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE},
                                    MainActivity.REQUEST_WRITE_EXTERNAL_STORAGE_CODE);
                        }
                    })
                    .create()
                    .show();
            return;
        } else {
            ActivityCompat.requestPermissions(activity,
                    new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE},
                    MainActivity.REQUEST_WRITE_EXTERNAL_STORAGE_CODE);
        }
    }


    public void refreshToken() {
        LOG.I("ManagementFragment=>refreshToken");
        LOG.I("Token: " + GuartinelApp.getDataStore().getToken());
        String cookieString = "userToken=%22" + GuartinelApp.getDataStore().getToken() + "%22; path=/";
        CookieManager.getInstance().setCookie(BuildConfig.WEBSITE_ADDRESS, cookieString);
    }

    private void setLoadingFinished() {
        loadingPanel.setVisibility(View.GONE);
        mWebView.setVisibility(View.VISIBLE);
    }

    private void setLoadingInProgress() {
        loadingPanel.setVisibility(View.VISIBLE);
        mWebView.setVisibility(View.GONE);
    }
}
