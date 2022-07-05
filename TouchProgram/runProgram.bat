adb kill-server
adb start-server
adb shell "pm list package -f -3 | grep com.example.touchprogram"
adb shell "CLASSPATH=/data/app/~~1XZyoT7sO4cOZohptvWWJw==/com.example.touchprogram-C55Id1b_NybI0m6TQsD36A==/base.apk /system/bin/app_process /system/bin com.example.touchprogram.TouchServer5"
PAUSE