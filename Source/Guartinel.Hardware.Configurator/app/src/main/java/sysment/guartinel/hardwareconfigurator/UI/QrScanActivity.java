package sysment.guartinel.hardwareconfigurator.ui;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.graphics.Matrix;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.os.PersistableBundle;
import android.support.annotation.Nullable;
import android.util.Log;
import android.view.View;
import android.widget.Button;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.target.SimpleTarget;
import com.bumptech.glide.request.transition.Transition;
import com.esafirm.imagepicker.features.ImagePicker;
import com.esafirm.imagepicker.features.ReturnMode;
import com.esafirm.imagepicker.model.Image;
import com.google.zxing.BarcodeFormat;
import com.google.zxing.BinaryBitmap;
import com.google.zxing.DecodeHintType;
import com.google.zxing.LuminanceSource;
import com.google.zxing.MultiFormatReader;
import com.google.zxing.NotFoundException;
import com.google.zxing.RGBLuminanceSource;
import com.google.zxing.Reader;
import com.google.zxing.Result;
import com.google.zxing.ResultPoint;
import com.google.zxing.common.HybridBinarizer;
import com.google.zxing.qrcode.QRCodeReader;
import com.journeyapps.barcodescanner.BarcodeCallback;
import com.journeyapps.barcodescanner.BarcodeResult;
import com.journeyapps.barcodescanner.CaptureActivity;
import com.journeyapps.barcodescanner.CaptureManager;
import com.journeyapps.barcodescanner.DecoratedBarcodeView;

import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Arrays;
import java.util.EnumMap;
import java.util.EnumSet;
import java.util.List;
import java.util.Map;
import java.util.Random;

import sysment.guartinel.hardwareconfigurator.R;
import sysment.guartinel.hardwareconfigurator.tools.LOG;

import static sysment.guartinel.hardwareconfigurator.ui.SelectHardwareFragment.TAG;

/**
 * Created by DAVT on 2017.12.18..
 */

public class QrScanActivity extends CaptureActivity {
    DecoratedBarcodeView decoratedBarcodeView;
    Button openFromDeviceButton;
    Activity _this;
    CaptureManager capture;
    Button torchButton;
    boolean isTorchOn = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.qr_scan_activity);
        _this = this;

        decoratedBarcodeView = (DecoratedBarcodeView) findViewById(R.id.zxing_barcode_scanner);

        openFromDeviceButton = (Button) findViewById(R.id.openFromDevice);
        torchButton = (Button) findViewById(R.id.torchButton);
        openFromDeviceButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                ImagePicker.create(QrScanActivity.this)
                        .returnMode(ReturnMode.ALL) // set whether pick and / or camera action should return immediate result or not.
                        .folderMode(true) // folder mode (false by default)
                        .toolbarFolderTitle("Folder") // folder selection title
                        .toolbarImageTitle("Tap to select") // image selection title
                        .toolbarArrowColor(Color.BLACK) // Toolbar 'up' arrow color
                        .single() // single mode
                        .limit(1) // max images can be selected (99 by default)
                        .showCamera(false) // show camera or not (true by default)
                        .enableLog(false) // disabling log
                        .start(); // start image picker activity with request code

            }
        });

        torchButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                isTorchOn = !isTorchOn;
                if(isTorchOn){
                    decoratedBarcodeView.setTorchOn();
                    torchButton.setText("Disable flash");
                    return;
                }
                torchButton.setText("Enable flash");
                decoratedBarcodeView.setTorchOff();
            }
        });

        capture = new CaptureManager(QrScanActivity.this, decoratedBarcodeView);
        capture.initializeFromIntent(getIntent(), savedInstanceState);
        capture.decode();

    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        capture.onSaveInstanceState(outState);
    }


    @Override
    protected void onDestroy() {
        super.onDestroy();
        capture.onDestroy();
    }

    @Override
    protected void onPause() {
        super.onPause();
        capture.onPause();
    }

    @Override
    protected void onResume() {
        super.onResume();
        capture.onResume();
    }


    @Override
    protected void onActivityResult(int requestCode, final int resultCode, Intent data) {
        if (ImagePicker.shouldHandle(requestCode, resultCode, data)) {
            Image image = ImagePicker.getFirstImageOrNull(data);
            LOG.I("Selected image path: " + image.getPath());

            String codeResult = getQRResultFromFileUri(Uri.fromFile(new File(image.getPath())));
            LOG.I(("SCAN RESULT :" + codeResult));
            Intent resultIntent = new Intent();
            resultIntent.putExtra("SCAN_RESULT", codeResult);
            setResult(Activity.RESULT_OK, resultIntent);
            finish();
             /*Bitmap result = null;
           Glide.with(QrScanActivity.this).asBitmap().load(image.getPath()).into(new SimpleTarget<Bitmap>() {
                @Override
                public void onResourceReady(Bitmap resource, Transition<? super Bitmap> transition) {
                    String codeResult = getQrResultFromBitmap(resource);
                    Intent resultIntent = new Intent();
                    resultIntent.putExtra("SCAN_RESULT", codeResult);
                    setResult(Activity.RESULT_OK, resultIntent);
                    finish();
                }
            });*/
            //  File file = new File(image.getPath());
            // BitmapFactory.Options bmOptions = new BitmapFactory.Options();
            //   Bitmap bitmap = BitmapFactory.decodeFile(file.getAbsolutePath(), bmOptions);
            //Bitmap resized = (Bitmap.createScaledBitmap(bitmap, bitmap.getWidth()/3, bitmap.getHeight()/3, false));

        }
    }

    private String getQRResultFromFileUri(Uri uri) {
        try {
            InputStream inputStream = QrScanActivity.this.getContentResolver().openInputStream(uri);
            Bitmap originalBitmap = BitmapFactory.decodeStream(inputStream);
            if (originalBitmap == null) {
                Log.e(TAG, "uri is not a bitmap," + uri.toString());
                return null;
            }
            String result = extractCode(originalBitmap);

            int tryCount = 0;
            while (tryCount < 4) {
                if (result != null) {
                    return result;
                }
                tryCount++;
                originalBitmap = rotateBitmap(originalBitmap, 90);
                result = extractCode(originalBitmap);
            }
        } catch (FileNotFoundException e) {
            Log.e(TAG, "can not open file" + uri.toString(), e);
            return null;
        }
        return null;
    }


    Random random = new Random();

    public String extractCode(Bitmap originalBitmap) {
        Bitmap bitmap = Bitmap.createScaledBitmap(originalBitmap, originalBitmap.getWidth() / 4, originalBitmap.getHeight() / 4, false);
        storeImage(bitmap, random.nextInt() + "");

        int width = bitmap.getWidth(), height = bitmap.getHeight();
        int[] pixels = new int[width * height];
        bitmap.getPixels(pixels, 0, width, 0, 0, width, height);
        bitmap.recycle();

        RGBLuminanceSource source = new RGBLuminanceSource(width, height, pixels);
        BinaryBitmap bBitmap = new BinaryBitmap(new HybridBinarizer(source));


        MultiFormatReader reader = new MultiFormatReader();
        Map<DecodeHintType, Object> tmpHintsMap = new EnumMap<DecodeHintType, Object>(
                DecodeHintType.class);
        tmpHintsMap.put(DecodeHintType.TRY_HARDER, Boolean.TRUE);
        tmpHintsMap.put(DecodeHintType.POSSIBLE_FORMATS,
                EnumSet.allOf(BarcodeFormat.class));
        tmpHintsMap.put(DecodeHintType.PURE_BARCODE, Boolean.FALSE);
        try {
            Result result = reader.decode(bBitmap, tmpHintsMap);
            return result.getText();
        } catch (NotFoundException e) {
            Log.e(TAG, "decode exception", e);
            return null;
        }
    }


    public Bitmap rotateBitmap(Bitmap source, float angle) {
        Matrix matrix = new Matrix();
        matrix.postRotate(angle);
        return Bitmap.createBitmap(source, 0, 0, source.getWidth(), source.getHeight(), matrix, true);
    }

   /* private String getQRResultFromFile(File file) {

        BitmapFactory.Options options = new BitmapFactory.Options();
        options.inPreferredConfig = Bitmap.Config.ARGB_8888;
        Bitmap bMap = BitmapFactory.decodeFile(file.getAbsolutePath(), options);
        String contents = null;

        int[] intArray = new int[bMap.getWidth() * bMap.getHeight()];

        bMap.getPixels(intArray, 0, bMap.getWidth(), 0, 0, bMap.getWidth(), bMap.getHeight());

        LuminanceSource source = new RGBLuminanceSource(bMap.getWidth(), bMap.getHeight(), intArray);
        BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));

        //Reader reader = new MultiFormatReader();
        Reader reader = new QRCodeReader();
        try {

            Map<DecodeHintType, Object> tmpHintsMap = new EnumMap<DecodeHintType, Object>(DecodeHintType.class);
            tmpHintsMap.put(DecodeHintType.TRY_HARDER, Boolean.TRUE);
            // tmpHintsMap.put(DecodeHintType.POSSIBLE_FORMATS, EnumSet.allOf(BarcodeFormat.class));
            List<BarcodeFormat> formats = Arrays.asList(BarcodeFormat.QR_CODE);
            tmpHintsMap.put(DecodeHintType.POSSIBLE_FORMATS, formats);
            //tmpHintsMap.put(DecodeHintType.NEED_RESULT_POINT_CALLBACK, formats);

            //tmpHintsMap.put(DecodeHintType.PURE_BARCODE, Boolean.FALSE);

            Result result = reader.decode(bitmap, tmpHintsMap);
            contents = result.getText();
        } catch (Exception e) {
            LOG.I("Error decoding barcode" + e.getMessage());
            e.printStackTrace();
        }
        return contents;
    }

    private String getQrResultFromBitmap(Bitmap bitmap) {
        String contents = null;

        int[] intArray = new int[bitmap.getWidth() * bitmap.getHeight()];
        //copy pixel data from the Bitmap into the 'intArray' array
        bitmap.getPixels(intArray, 0, bitmap.getWidth(), 0, 0, bitmap.getWidth(), bitmap.getHeight());

        LuminanceSource source = new RGBLuminanceSource(bitmap.getWidth(), bitmap.getHeight(), intArray);
        BinaryBitmap binaryMap = new BinaryBitmap(new HybridBinarizer(source));

        Reader reader = new QRCodeReader();
        try {
            Result result = reader.decode(binaryMap);
            Map<DecodeHintType, Object> tmpHintsMap = new EnumMap<DecodeHintType, Object>(DecodeHintType.class);
            tmpHintsMap.put(DecodeHintType.TRY_HARDER, Boolean.TRUE);
            tmpHintsMap.put(DecodeHintType.POSSIBLE_FORMATS, EnumSet.allOf(BarcodeFormat.class));
            tmpHintsMap.put(DecodeHintType.PURE_BARCODE, Boolean.FALSE);
            contents = result.getText();
        } catch (Exception e) {
            LOG.I("Error decoding barcode" + e.getMessage());
        }
        LOG.I("Code read: " + contents);
        return contents;
    }*/

    public boolean storeImage(Bitmap imageData, String filename) {
        // get path to external storage (SD card)

        File sdIconStorageDir = null;

        sdIconStorageDir = new File(Environment.getExternalStorageDirectory()
                .getAbsolutePath() + "/test/");
        // create storage directories, if they don't exist
        if (!sdIconStorageDir.exists()) {
            sdIconStorageDir.mkdirs();
        }
        try {
            String filePath = sdIconStorageDir.toString() + File.separator + filename + ".png";
            FileOutputStream fileOutputStream = new FileOutputStream(filePath);
            BufferedOutputStream bos = new BufferedOutputStream(fileOutputStream);
            //Toast.makeText(m_cont, "Image Saved at----" + filePath, Toast.LENGTH_LONG).show();
            // choose another format if PNG doesn't suit you
            imageData.compress(Bitmap.CompressFormat.PNG, 100, bos);
            bos.flush();
            bos.close();

        } catch (FileNotFoundException e) {
            Log.w("TAG", "Error saving image file: " + e.getMessage());
            return false;
        } catch (IOException e) {
            Log.w("TAG", "Error saving image file: " + e.getMessage());
            return false;
        }
        return true;
    }

}
