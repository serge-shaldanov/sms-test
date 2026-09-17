# Решение тестового задания для компании «ООО СМАРТ МИЛ СЕРВИС»

## SmsTest Orders App

Консольное приложение, реализующее функционал из первого задания.

Представлено проектами `SmsTest.OrdersApi.*` и `SmsTest.Mocks.OrdersApi.*`:

* `SmsTest.OrdersApi.Console` - точка входа, консольное приложение.
* `SmsTest.OrdersApi.Contracts` - общие контракты для всех типов API.
* `SmsTest.OrdersApi.Rest.Client` - клиент для REST API.
* `SmsTest.OrdersApi.Grpc.Client` - клиент для gRPC API.
* `SmsTest.OrdersApi.Grpc.Contracts` - общие контракты gRPC API (proto-файлы и определения).

Вспомогательные проекты:

* `SmsTest.Mocks.OrdersApi.Rest` - мокс-сервер для REST API.
* `SmsTest.Mocks.OrdersApi.Grpc` - мокс-сервер для gRPC API.

Более детальное описание некоторых проектов можно посмотреть в их собственных `README.md` файлах.

## SmsTest Variables App

Консольное приложение, реализующее функционал из второго задания.

Представлено единственным проектом `SmsTest.VariablesApp`.

Более детальное описание проекта можно посмотреть в его собственном `README.md` файле.