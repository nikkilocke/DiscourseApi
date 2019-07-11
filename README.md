# DiscourseApi

This is a C# wrapper to (the most important bits of) the [Discourse API](https://docs.discourse.org/).

It is provided with a Visual Studio 2019 build solution for .NET Standard, so can be used with any version of .NET.

There is a test project (for net core only) which also demonstrates usage.

## Setup before using the API

In order to use the Discourse API you need to obtain an ApiKey from the relevant Discourse server admin panel. You also need to supply an Api Username.

This information has to be provided in an object that implements the [ISettings](../master/DiscourseApi/Settings.cs) interface, which is then used to create a DiscourseApi instance. A Settings class which implements this interface is provided, to save you work. This provides a static Load method, reads the settings from *LocalApplicationData*/DiscourseApi/Settings.json. On a Windows 10 machine, *LocalApplicationData* is `C:\Users\<USER>\AppData\Local`, on Linux it is `~user/.local/share`.

## Testing

In order to run the Unit Tests provided, you must provide additional data in your ISettings object - see the Settings object in [UnitTest1.cs](../master/Tests/UnitTest1.cs).

## Hooks for more complex uses

You do not have to use the provided Settings class, provided you have a class that implements ISettings.

## License

This wrapper is licensed under creative commons share-alike, see [license.txt](../master/license.txt).

## Using the Api

The Unit Tests should give you sufficient examples on using the Api.

An Api instance is created by passing it an object that implements ISettings (a default class is provided which will read the settings from a json file). The Api instance is IDisposable, so should be Disposed when no longer needed (this is because it contains an HttpClient).

C# classes are provided for the objects you can send to or receive from the Discourse api. For instance the Group object represents groups. These main objects have methods which call the Discourse api - such as Group.Create to create a new group, Group.Get to get group details, etc.

Some Api calls return a list of items (such as Group.ListAll). These are returned as a subclass of ApiList<Group>. The Discourse api itself usually only returns the first few items in the list, and needs to be called again to return the next chunk of items. This is all done for you by ApiList - it has a method called All(Api) which will return an IEnumerable of the appropriate listed object. Enumerating the enumerable will return all the items in the first chunk, then call the Discourse api to get the next chunk, return them and so on. It hides all that work from the caller, while remaining as efficient as possible by only getting data when needed - for instance, using Linq calls like Any or First will stop getting data when the first item that matches the selection function is found.


