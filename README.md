# Nostify Example

## About This Example

This is the example repository for the [Nostify framework](https://github.com/yanbu0/nostify). It demonstrates a microservices architecture using Nostify, featuring:

- **Account Service**: Complete CQRS implementation with account management functionality
  - Account aggregates with command and event handlers
  - Account status management
  - Full account projections
  - Comprehensive test coverage

- **Employee Service**: Employee management microservice
  - Employee aggregates and domain logic
  - Event-driven architecture
  - Projection support

Both services follow Domain-Driven Design (DDD) principles with:
- Event sourcing patterns
- Command and query separation (CQRS)
- Aggregate root implementations
- Value objects and domain events
- Azure Functions hosting

## Version 4.8.x Update

This example has been updated to the latest available 4.8.x release. The major updates include:

- **Updated to nostify 4.8.0**: All projects now reference the latest published nostify package
- **gRPC Event Request Gateway**: The `GrpcEventRequestServer` project provides a centralized gRPC event request service that routes by `service_name`
- **Projection gRPC Integration**: `FullAccount` now uses `ExternalDataEventFactory` gRPC requestors when a gateway address is configured
- **Fallback Compatibility**: Projection initialization still supports HTTP `EventRequest` fallback when gRPC settings are not provided
- **Current Standards Alignment**: Service projects now rely on nostify's built-in gRPC contracts instead of maintaining duplicate generated contracts

### gRPC Projection Configuration

Set the following values in `/Microservices/Account/local.settings.json` to enable gRPC event requests from projections:

- `GrpcEventRequestAddress` - gRPC gateway endpoint (example: `http://localhost:5050`)
- `GrpcEventRequestAuthToken` - optional API key/token for secured gateways
- `GrpcEmployeeServiceName` - target service route name (default: `Employee`)

When `GrpcEventRequestAddress` is provided, `FullAccount` uses `WithGrpcEventRequestor(...)`. If it is omitted, the projection falls back to HTTP `EventRequest`.

Please refer to the [main nostify repository README](https://github.com/yanbu0/nostify).
