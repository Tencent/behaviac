package com.tencent.behaviac.behaviac_android;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.widget.TextView;

public class MainActivity extends AppCompatActivity {

    // Used to load the libraries on application startup.
    static {
        System.loadLibrary("behaviac_gcc_debug");
        System.loadLibrary("tutorial_11_gcc_debug");
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Example of a call to a native method
        TextView tv = (TextView) findViewById(R.id.sample_text);
        tv.setText(TestMain(getApplication().getAssets()));
    }

    /**
     * A native method that is implemented by the native library,
     * which is packaged with this application.
     */
    public native String TestMain(android.content.res.AssetManager assetManager);
}
