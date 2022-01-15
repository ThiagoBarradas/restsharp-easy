[![Build Status](https://barradas.visualstudio.com/Contributions/_apis/build/status/NugetPackage/RestSharp%20Easy?branchName=develop)](https://barradas.visualstudio.com/Contributions/_build/latest?definitionId=15&branchName=master)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RestSharp.Easy.svg)](https://www.nuget.org/packages/RestSharp.Easy/)
[![NuGet Version](https://img.shields.io/nuget/v/RestSharp.Easy.svg)](https://www.nuget.org/packages/RestSharp.Easy/)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ThiagoBarradas_restsharp-easy&metric=alert_status)](https://sonarcloud.io/dashboard?id=ThiagoBarradas_restsharp-easy)
<!-- [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=ThiagoBarradas_restsharp-easy&metric=coverage)](https://sonarcloud.io/dashboard?id=ThiagoBarradas_restsharp-easy) -->
# RestSharp Easy

Complement for RestSharp to create fast and easily API Clients (SDK) with Newtonsoft serialization and Serilog Log

## Install via NuGet

````command
PM> Install-Package RestSharp.Easy
````

## Sample

> Use it as Scoped or Transient because this client keeps context to use some features like RequestKey, AdditionalLog items, etc.

````csharp

var config = new EasyRestClientConfiguration
{
	BaseUrl = "https://server.com/api/v1",
	TimeoutInMs = 60000,
	SerializeStrategy = SerializeStrategyEnum.SnakeCase,
	RequestKey = "123456"
};

IEasyRestClient client = new EasyRestClient(config);

var response = client.SendRequestAsync<User, ErrorModel>(Method.GET, "users");

````

## How can I contribute?
Please, refer to [CONTRIBUTING](.github/CONTRIBUTING.md)

## Found something strange or need a new feature?
Open a new Issue following our issue template [ISSUE TEMPLATE](.github/ISSUE_TEMPLATE.md)

## Changelog
See in [nuget version history](https://www.nuget.org/packages/RestSharp.Easy)

## Did you like it? Please, make a donate :)

if you liked this project, please make a contribution and help to keep this and other initiatives, send me some Satochis.

BTC Wallet: `1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX`

![1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX](https://i.imgur.com/mN7ueoE.png)
