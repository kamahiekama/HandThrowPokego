package com.example.touchprogram;

import android.hardware.input.InputManager;
import android.os.SystemClock;
import android.view.InputEvent;
import android.view.MotionEvent;

import androidx.core.view.InputDeviceCompat;

import java.lang.reflect.Method;

public class TouchServer2 {

    private static int touchDeviceId = 0;

    private static final class MyInputAgent{
        private static MyInputAgent instance = null;

        static{
            try {
                instance = new MyInputAgent();
            } catch (Exception ex){
                ex.printStackTrace();
            }
        }

        private static MyInputAgent getInstance(){
            return instance;
        }

        private InputManager inputManager;
        private Method injectInputEventMethod;
        private MotionEvent motionEvent;

        private MyInputAgent() throws Exception{
            inputManager = (InputManager)InputManager.class.getDeclaredMethod("getInstance").invoke(null, new Object[0]);
            injectInputEventMethod = InputManager.class.getDeclaredMethod("injectInputEvent", new Class[]{InputEvent.class, int.class});

            MotionEvent.class.getDeclaredMethod("obtain", long.class, long.class, int.class, int.class,
                    MotionEvent.PointerProperties[].class, MotionEvent.PointerCoords[].class,
                    int.class, int.class, float.class, float.class, int.class, int.class, int.class, int.class)
                    .setAccessible(true);
        }

        private void injectMotionEvent(int src, int act, int x, int y, int id) throws Exception{
            int pc = 0;
            int nc = 1;
            if (motionEvent == null){
                long down_time = SystemClock.uptimeMillis();
                motionEvent = MotionEvent.obtain(down_time, down_time, MotionEvent.ACTION_CANCEL, 0, 0, 0);
            } else {
                pc = motionEvent.getPointerCount();
                nc = pc + 1;
                for(int i = 0; i < pc; i++){
                    if (id == motionEvent.getPointerId(i)){
                        nc = pc;
                        break;
                    }
                }
            }

            MotionEvent.PointerProperties[] pointerProperties = new MotionEvent.PointerProperties[nc];
            MotionEvent.PointerCoords[] pointerCoords = new MotionEvent.PointerCoords[nc];
            for(int i = 0; i < pc; i++){
                pointerProperties[i] = new MotionEvent.PointerProperties();
                motionEvent.getPointerProperties(i, pointerProperties[i]);
                pointerCoords[i] = new MotionEvent.PointerCoords();
                motionEvent.getPointerCoords(i, pointerCoords[i]);
            }

            int newIndex = pc;
            if (pc == nc){
                for(int i = 0; i < pc; i++){
                    if (id == motionEvent.getPointerId(i)){
                        newIndex = i;
                    }
                }
            } else {
                pointerProperties[newIndex] = new MotionEvent.PointerProperties();
                pointerCoords[newIndex] = new MotionEvent.PointerCoords();
            }

            pointerProperties[newIndex].id = id;
            pointerProperties[newIndex].toolType = MotionEvent.TOOL_TYPE_FINGER;

            pointerCoords[newIndex].x = x;
            pointerCoords[newIndex].y = y;
            pointerCoords[newIndex].pressure = 0.3f;
            pointerCoords[newIndex].size = 0.1f;
            pointerCoords[newIndex].touchMajor = 3.0f;
            pointerCoords[newIndex].touchMinor = 3.0f;
            pointerCoords[newIndex].toolMajor = 3.0f;
            pointerCoords[newIndex].toolMinor = 3.0f;
            pointerCoords[newIndex].orientation = 0.8f;

            int pointerAction = act;
            if (1 < nc){
                if (MotionEvent.ACTION_DOWN == act){
                    pointerAction = MotionEvent.ACTION_POINTER_DOWN | (newIndex << MotionEvent.ACTION_POINTER_INDEX_SHIFT);
                } else if (MotionEvent.ACTION_UP == act){
                    pointerAction = MotionEvent.ACTION_POINTER_UP | (newIndex << MotionEvent.ACTION_POINTER_INDEX_SHIFT);
                }
            }

            motionEvent = MotionEvent.obtain(
                    motionEvent.getDownTime(), SystemClock.uptimeMillis(),
                    pointerAction, nc, pointerProperties, pointerCoords,
                    0, 0, 1.0f, 1.0f, touchDeviceId, 0, src, 0);

            injectInputEventMethod.invoke(inputManager, new Object[]{motionEvent, 0});

            if (act == MotionEvent.ACTION_UP){
                pc = motionEvent.getPointerCount();
                nc -= 1;
                if (0 < nc){
                    MotionEvent.PointerProperties[] newPointerProperties = new MotionEvent.PointerProperties[nc];
                    MotionEvent.PointerCoords[] newPointerCoords = new MotionEvent.PointerCoords[nc];
                    int setCount = 0;
                    for(int i = 0; i < pc; i++){
                        if (id != motionEvent.getPointerId(i)){
                            newPointerProperties[setCount] = new MotionEvent.PointerProperties();
                            motionEvent.getPointerProperties(i, newPointerProperties[setCount]);
                            newPointerCoords[setCount] = new MotionEvent.PointerCoords();
                            motionEvent.getPointerCoords(i, newPointerCoords[setCount]);
                            setCount++;
                        }
                    }

                    motionEvent = MotionEvent.obtain(
                            motionEvent.getDownTime(), motionEvent.getEventTime(),
                            MotionEvent.ACTION_CANCEL, nc, newPointerProperties, newPointerCoords,
                            0, 0, 1.0f, 1.0f, touchDeviceId, 0, src, 0);
                } else {
                    motionEvent = null;
                }
            }
        }
    }

    private static void Motion(int act, int x, int y, int id){
        try{
            MyInputAgent.getInstance().injectMotionEvent(InputDeviceCompat.SOURCE_TOUCHSCREEN, act, x, y, id);
            System.out.println("act: " + act + " x: " + x + " y: " + y);
        } catch (Exception ex){
            ex.printStackTrace();
        }
    }

    public static void main(String[] args) throws InterruptedException {
        int x = 500;
        int y = 2000;
        Motion(MotionEvent.ACTION_DOWN, x, y, 0);
        Thread.sleep(200);
        for(int i = 0; i < 10; i++){
            y -= 20 * (i + 1);
            Motion(MotionEvent.ACTION_MOVE, x, y, 0);
            Thread.sleep(15);
        }
        Motion(MotionEvent.ACTION_UP, x, y, 0);
    }
}
