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

## Version 4.1.0 Update

This example has been updated to version 4.1.0. The major updates include:

- **Upgraded to .NET 10.0**: All projects now target .NET 10 for improved performance and access to the latest framework features
- **Updated Dependencies**: All package dependencies have been updated to be compatible with nostify 4.1.0
- **ExternalDataEventFactory Pattern**: The FullAccount projection now uses the recommended ExternalDataEventFactory fluent builder pattern for gathering external data events, providing cleaner and more maintainable code
- **Modern Patterns**: All code follows the latest recommended patterns from nostify 4.1.0

For detailed information about nostify 4.1.0 features, including:
- Sequential Number Generation
- Enhanced Testability with IQueryExecutor
- Default Command and Event Handlers
- Event Hubs Support
- And more

Please refer to the [main nostify repository README](https://github.com/yanbu0/nostify).
