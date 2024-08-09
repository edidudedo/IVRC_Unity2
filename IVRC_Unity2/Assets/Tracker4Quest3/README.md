# 概要
Meta XRのOVRCameraRigでvive trackerを使うためのscriptです。  
このscriptを使うには、SteamVRの設定が必要です。

# 設定
1. steamvr.vrsettingsを開く（多分ここ "C:\Program Files (x86)\Steam\config" に入ってる）  
2. steamvr.vrsettingsの"steamvr" : {}　の中に次の3つを加える  
    "activateMultipleDrivers" : true,  
    "debugInputBinding" : true,  
    "forcedDriver" : "null",  
1. SteamVRを再起動する  
2. Vive TrackerをSteamVRと接続する
3. TrackerをSteamVRに接続する

# 使い方
1. TrackerManager prefabをsceneに追加する
2. TrackerManagerのTargetObjsに、TrackingしたいObjectを追加する
3. play modeに入る