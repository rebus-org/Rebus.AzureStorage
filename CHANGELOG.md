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

## 5.0.0

* Update WindowsAzure.Storage dependency to 9.3.3 and Rebus dependency to 5.0.0

---

[mattwhetton]: https://github.com/mattwhetton
[micdah]: https://github.com/micdah