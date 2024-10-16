
# Local Device Finder
[日本語](./README_JA.md) | English

Local Device Finder is a C# library for detecting devices on a local network. It sends discovery messages using UDP broadcast and multicast, then waits for responses from the devices.

## Features
- Detect devices on the local network.
- Works in both Editor and Runtime modes.
- If ports are set up properly, it is possible to test both sending and receiving on a single PC.
- Responses are customizable. In the sample, only the device name is returned as a response, but you can add any information to the response.
- Supports both UDP broadcast and multicast, with multicast reducing network load.
- It is possible to send data individually to detected devices.

## Installation

To install the `Local Device Finder` package using OpenUPM, run the following command in your project's root folder:

```
openupm add com.afjk.com.afjk.local-device-finder
```

Alternatively, to install the `Local Device Finder` package using Unity Package Manager, follow these steps:

1. In UnityEditor, go to Window -> Package Manager.
2. Click the + button in the top left.
3. Select "Add package from git URL..."
4. Paste the following URL and press Enter.

```
https://github.com/afjk/LocalDeviceFinder.git?path=/Packages/com.afjk.local-device-finder#v0.0.1
```

## Samples
Import the Local Device Finder samples from the Package Manager.

![image](https://github.com/user-attachments/assets/23d8f72b-4bbe-434e-ad5f-e55686d13c76)

### Editor Sample
![image](https://github.com/user-attachments/assets/0db6f3ee-b92e-42d8-807d-8fc645109bda)

LocalDeviceFinderEditor and LocalDeviceResponderEditor are samples for testing device detection and response within the Unity editor.

To open these editor windows, in UnityEditor, click `Tools > Local Device Finder > Finder` or `Tools > Local Device Finder > Responder`.

### Runtime Sample
<img src="https://github.com/user-attachments/assets/0d1a92e9-802c-4720-be93-2dd930b2b4fa" width="50%">

`LocalDeviceFinderClient` and `LocalDeviceResponderClient` are samples for detecting devices and responding at runtime.

These classes can be attached to Unity game objects for use.

The sample scene `Assets/Samples/Local Device Finder/0.0.1/Sample/Demo/Demo.unity` includes an example using these classes.
