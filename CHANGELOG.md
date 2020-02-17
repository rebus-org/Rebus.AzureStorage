# Changelog

## 2.0.0-a1
* Test release

## 2.0.0-b01
* Test release

## 2.0.0-b02
* Updated to Rebus 2.0.1

## 3.0.0-b01
* Update to Rebus 3

## 3.0.0-b02
* Fix configuration API that accidentally threw an `ArgumentNullException` when it shouldn't have

## 4.0.0
* Update to Rebus 4
* Add .NET Core support

## 4.0.1
* Validate that queue names passed to the configuration can be used because the error returned from Azure is not that great - thanks [mattwhetton]

## 4.1.0
* Add .NET Standard 2.0 as a target

## 4.2.0
* Ability to configure whether to use the native message deferral mechanism, making it possibe to register a custom timeout manager (e.g. in SQL Server)
* Prefetch option - can be configured to prefetch a number of messages (up to 32, because that's what Azure Storage Queues can do), which may improve performance in some scenarios

## 4.2.1
* Fix bug that would leak memory due to static `OperationContext` instances accumulating state for each call

## 4.3.0
* Make message visibility timeout configurable - thanks [micdah]

## 4.3.1
* Set access condition when updating last read time of data bus attachments to avoid race condition

## ~~5.0.0~~
Sorry, but version 5.0.0 was unlisted from NuGet.org, when it turned out that Microsoft's new storage client libs for blobs, queues, AND tables (the new CosmosDB driver) could not live in the same application.

Since the original WindowsAzure.Storage library is deprecated, and clients will continue their life in individually versioned packages, so will Rebus' implementations.

Therefore: Go check out [`Rebus.AzureBlobs`](https://github.com/rebus-org/Rebus.AzureBlobs) for blob-based implementations of databus storage and saga snapshots.


---

[mattwhetton]: https://github.com/mattwhetton
[micdah]: https://github.com/micdah