# BxHelpers

BxHelpers is a library designed to simplify event and export handling in FiveM resources.
It allows you to send more advanced types of data with events and exports, for example Player, Vehicle, and other custom classes.

## **Installation**

1. Change the `LangVersion` to the latest in your FiveM resource's `.csproj` file:
   ```xml
   <LangVersion>latest</LangVersion>
   ```

2. Include the helpers by adding this line to your `ItemGroup` in the `.csproj` file (or your own directory):
   ```xml
   <Compile Include="../../Helpers/**/*.cs" />
   ```

***Important***: When using exports make sure both resources include this library.

## **Events**

### Creating an Event Class

To create a new event, you need to define a class that extends `BaseEvent`. Here's an example:

```cs
public class TestEvent : BaseEvent
{
    public string Test { get; set; }
    public Vehicle Vehicle { get; set; }

    public TestEvent(string test, Vehicle vehicle)
    {
        Test = test;
        Vehicle = vehicle;
    }
}
```

### Event handlers

For server events, the method that handles the event needs to accept a `Player` parameter along with your event data. The reason the `Player` parameter is included is because the server knows which player triggered the event. This makes it impossible for cheaters to fake the sender of the event.

For client side or same-side events, the method only needs to accept the event data.

***Important***: The method that handles the event must be public and static.

### Server
```cs
[BxEvent(typeof(TestEvent))]
public static void TestEvent(Player client, TestEvent data)
{
    Debug.WriteLine($"Client {client.Name} sent event with test: {data.Test} and vehicle: {data.Vehicle.Model}");
}
```

- **Player client**: A player who triggered the event on the server. Since the server knows who the player is, it’s not possible for a cheater to fake the sender of the event.

#### Triggering the event from client-side
```cs
BxEvents.SendServerEvent(new TestEvent("test", GetClosestVehicle()));
```

### Client
```cs
[BxEvent(typeof(TestEvent))]
public static void TestEvent(TestEvent data)
{
    Debug.WriteLine($"Server sent event with test: {data.Test} and vehicle: {data.Vehicle.Model}");
}
```

#### Triggering the event from server-side
```cs
BxEvents.SendClientEvent(client, new TestEvent("test", GetClosestVehicle()));
```

### Same Side
#### Triggering the event
```cs
BxEvents.SendEvent(new TestEvent("test", GetClosestVehicle()));
```

## **Exports**

### Creating an Export Class

To use exports, create a class that extends `BaseExport`:

```cs
public class TestExport : BaseExport
{
    public string Test { get; set; }

    public TestExport(string test)
    {
        Test = test;
    }
}
```

### Registering Exports

Use the `BxExport` attribute to register an export method:

```cs
[BxExport(typeof(TestExport))]
public static string TestExport(TestExport data)
{
    Debug.WriteLine($"Export called with test: {data.Test}");
    return "Response";
}
```

- **Return value**: The return value of the export method will be sent back to the caller, it can be any type.

### Calling Exports

To call an export, use the `BxExports.CallExport` method and pass the return type as a generic parameter:

```cs
var response = BxExports.CallExport<string>(new TestExport("test"));
Debug.WriteLine($"Export response: {response}");
```

# **Contributing**
Pull requests are welcome.
