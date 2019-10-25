# SwitchingProject
IP Lookup Project for the Switching and Routing course coded in C#

# Contents
This project contains:
- IPLookup: a .NET standard library that implements the Binary, Compressed and Multibit tries
- IPLookupTool: a GUI tool made with WPF that offers performance analysis capabilities

# Library API
The library is fully documented using built-in XML comments, so a description for each method can easily be read using Visual Studio or any other editor that supports it.

First of all add the references:
```csharp
using IPLookup;
using IPLookup.Nodes;
```

Creating the tries:
```csharp
var binaryRoot = new BinaryNode() { NextHop = "Root" };
var compressedRoot = new CompressedNode() { NextHop = "Root" };
var multibitRoot = new MultibitNode() { Stride = 3 };
```

Generating IPs to be used to create the lookup table or to simulate incoming packets:
```csharp
var gen = new AddressGenerator().GenerateAddress();
var db = gen.Take(10).ToArray();
```
where you can choose how many addresses you want to generate by changing the argument of the Take function.

Adding all DB nodes to the tries:
```csharp
foreach (var ip in db)
{
    binaryRoot.AddChild(ip.MaskedIPv4, ip.BinaryString);
    compressedRoot.AddChild(ip.MaskedIPv4, ip.BinaryString);
    multibitRoot.AddChild(ip.MaskedIPv4, ip.BinaryString);
}
compressedRoot.Compress();
```

Performing a lookup:
```csharp
var tosearch = new Address("132.15.162.33/17");
Console.WriteLine($"Binary: {binaryRoot.Lookup(tosearch.BinaryString)}");
Console.WriteLine($"Compressed: {compressedRoot.Lookup(tosearch.BinaryString)}");
Console.WriteLine($"Multibit: {multibitRoot.Lookup(tosearch.BinaryString, "Root")}");
```
You can also call `LookupNonRecursive` instead of `Lookup` to get better performances.
