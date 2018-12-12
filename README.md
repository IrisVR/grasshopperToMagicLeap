ar-prototype-ivd - MagicLeap
===

Prototype/samples intended for MagicLeap.
Without hardware, we are only able to test in the simulator (MLRemote) which interacts the Unity Editor. This means the dependences/dlls might have differences when compiled for the platform once hardware is available.

#### Samples
  - ivdloading: Used to use common ivdloading code and dependencies on MagicLeap

#### Dev Requirements
  1. Create an account at magic leap dev portal - you often have to do two factor authentication for ... everything.
    - Notice that there are personal accounts vs MagicLeap parter programs with better support. Setup can use either as you need an account to interact with SDK.
  2. Install the PackageManager
      https://creator.magicleap.com/downloads/lumin-sdk/overview
  3. From the PackageManager, it'll point you to a specific Version of Unity that you'll need.
      - (Unity 2018.1.0f2-MLTP6 is current version
      - Install this along side your other Unity Versions.
  4. Install:
      - Lumin SDK
      - Unity API Documentation
      - Magic Leap Unity Package

#### Dependencies

  1. We pulled the sqlite binaries from the android prebuilt binaries to see if they would work and they did:
  https://www.sqlite.org/download.html
    1. Download and extract https://www.sqlite.org/2018/sqlite-android-3240000.aar
    2. Navigate to jni\arm64-v8a\libsqliteX.so since the MagicLeap is a tegra x2 (https://en.wikipedia.org/wiki/Tegra#Tegra_X2) we used the arm64-v8a binary and dropped it into MagicLeap\Assets\Plugins\sqlite3.so

#### Dev process overview for ivdloading
  These steps are a shortcut reference to the online ML documentation for integrating with Unity.
  1. Open up the MagicLeap package manager.
  2. Open up the MagicLeap SDK in explorer.
  3. Navigate to MLRemote and launch it.
    i.e.
    C:\Users\kevin\MagicLeap\mlsdk\v0.15.0\VirtualDevice\bin\UIFrontend\MLRemote.exe
  4. Click on Start Simulator
  5. In the Simulator import/load a sample room:
    i.e.
    C:\Users\kevin\MagicLeap\mlsdk\v0.15.0\VirtualDevice\data\VirtualRooms\ExampleRooms\CouchRoom.room
  6. In Unity if th platform and SDK are set, use the menu to enable Live Interaction which will cause Unity to restart.
  6. When hitting play, the visuals in Unity will be sent to the MLRemote viewer.