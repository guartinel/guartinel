package sysment.android.guartinel.core.persistance;

import android.annotation.TargetApi;
import android.os.Build;
import android.os.Environment;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.lang.Exception;
import java.lang.String;

import sysment.android.guartinel.GuartinelApp;
import sysment.android.guartinel.core.utils.LOG;

/**
 * Created by sysment_dev on 12/09/2016.
 */
public class FileConfigPersistance {
    private static String getConfigFilePath() {
        return GuartinelApp._context.getExternalFilesDir("") + "/guartinel.config";
    }
    private class Environments {
        public static final String TEST = "test";
    }

    @TargetApi(Build.VERSION_CODES.KITKAT)
    public static boolean isTestEnvironment() {
        String line = "";

        if (!isExternalStorageAvailable()) {
            return false;
        }
        try {
            FileInputStream fis = new FileInputStream(getConfigFilePath());
            DataInputStream in = new DataInputStream(fis);
            BufferedReader br =
                    new BufferedReader(new InputStreamReader(in));
            String strLine;
            while ((strLine = br.readLine()) != null) {
                line = strLine;
            }
            in.close();
        } catch (IOException e) {
            LOG.I("No config file for Guartinel.");
            createEmptyConfigFile();
        }
        if (line.equals(Environments.TEST)) {
            return true;
        }
        return false;
    }

    private static void createEmptyConfigFile() {
        try {
            File file = new File(getConfigFilePath());
            if (!file.exists()) {
                try {
                    file.createNewFile();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        } catch (Exception e) {

        }
    }

    private static boolean isExternalStorageAvailable() {
        String extStorageState = Environment.getExternalStorageState();
        if (Environment.MEDIA_MOUNTED.equals(extStorageState)) {
            return true;
        }
        return false;
    }
}