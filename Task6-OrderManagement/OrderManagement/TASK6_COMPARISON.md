# Task 6 - Direct REST vs Kafka

## Direct REST

Order creation:

Client
-> OrderService
-> CustomerService through HTTP
-> InventoryService through HTTP
-> Confirmed or Rejected

The calling service waits for its dependencies.

If a required service is unavailable, the request can fail.


## Kafka

Customer changes:

KafkaCustomerService
-> customer-events
-> KafkaOrderService
-> LocalCustomers


Order creation:

Client
-> KafkaOrderService
-> local customer validation
-> save order as Created
-> order-created
-> Kafka


Inventory:

Kafka
-> KafkaInventoryService
-> check stock
-> inventory-reserved OR inventory-rejected
-> Kafka


Order completion:

Kafka
-> KafkaOrderService
-> Created becomes Confirmed or Rejected
-> order-confirmed OR order-rejected


## Main Difference

REST is synchronous.

Kafka is asynchronous.


## Service Dependency

REST services depend on other services being available
during the request.

Kafka services communicate through durable events and can
process events later.


## Eventual Consistency

In Kafka an order is first:

Created

and later becomes:

Confirmed

or:

Rejected


## Idempotency

KafkaInventoryService stores processed EventIds.

If the same order-created event is delivered twice,
Inventory ignores the duplicate and does not reduce stock twice.


## Consumer Lag

Lag is the number of Kafka records a consumer has not
processed yet.

When a consumer is offline, lag increases.

When it returns, it processes the backlog and lag decreases.


## Analytics

Direct AnalyticsService calls CustomerService,
OrderService and InventoryService through HTTP.

KafkaAnalyticsService consumes customer-events,
order-events and inventory-events and builds local state.


## REST Is Better When

- immediate response is required
- workflow is simple
- synchronous consistency is important


## Kafka Is Better When

- services should be loosely coupled
- consumers may temporarily be offline
- events need to survive downtime
- multiple services need the same events
- high throughput is important
- asynchronous processing is acceptable


## Conclusion

Neither approach is always better.

REST is simpler for synchronous request-response operations.

Kafka is better suited to asynchronous distributed workflows
where durability, recovery, scalability and service independence
are important.

Real systems commonly use both.
