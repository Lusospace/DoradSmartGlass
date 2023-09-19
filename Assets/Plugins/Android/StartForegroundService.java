package com.example.ssd1333test;
import android.content.ComponentName;
import android.content.Intent;
import android.content.Context;

public final class StartForegroundService {
    public static void startService(Context context) {
        Intent intent = new Intent();
        intent.setComponent(new ComponentName("com.example.ssd1333test", "com.example.ssd1333test.SSD1333loaderLoadTable"));
        context.startForegroundService(intent);
    }
	public static void sendImagetoService(Context context) {
        Intent intent = new Intent();
        intent.setComponent(new ComponentName("com.example.ssd1333test", "com.example.ssd1333test.SSD1333loaderLogoLS"));
        context.startForegroundService(intent);
    }
}
