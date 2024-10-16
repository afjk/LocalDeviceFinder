# Local Device Finder

Local Device Finder is a C# library that allows you to discover devices on your local network. It uses UDP broadcast and multicast to send discovery messages and listens for responses from devices.

## Features

- Send UDP broadcast messages.
- Send UDP multicast messages.
- Start and stop receiving messages.
- Deserialize received data.

## Usage

First, create an instance of `DeviceSearcher`:

```csharp
IReceiveDataFactory receiveDataFactory = new MyReceiveDataFactory();
int receivePort = 12345;
string multicastIP = "239.255.255.250";
DeviceSearcher deviceSearcher = new DeviceSearcher(receiveDataFactory, receivePort, multicastIP);
```

To send a broadcast message:

```csharp
int port = 12345;
deviceSearcher.SendBroadcast(port);
```

To send a multicast message:

```csharp
int port = 12345;
deviceSearcher.SendMulticast(port);
```

To start receiving messages:

```csharp
deviceSearcher.StartReceiving((data, ipAddress) =>
{
    // Handle received data here.
});
```

To stop receiving messages:
```csharp
deviceSearcher.StopReceiving();
```

## LocalDeviceFinderEditor Usage

LocalDeviceFinderEditor is a Unity Editor Window that provides a GUI for using the DeviceSearcher class.  To open the LocalDeviceFinderEditor window, go to the Unity Editor and click on Tools > Local Device Finder.  In the LocalDeviceFinderEditor window, you can:  

* Set the send and receive ports.
* Choose whether to use multicast or not.
* If using multicast, set the multicast IP.
* Start finding devices by clicking the "Start Finding" button. This will send a broadcast or multicast message and start receiving responses.
* Start a receiver by clicking the "Start Receiver" button. This will start listening for incoming messages.
* Stop the receiver by clicking the "Stop Receiver" button.
The discovered devices will be displayed in the window.
