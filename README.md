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

## Version 3.8.0 Update

This example has been updated to version 3.8.0. The updates include:

- **EventFactory Pattern**: All direct Event instantiation has been replaced with the new EventFactory pattern for better abstraction and validation
- **IEvent Interface**: Apply methods now use IEvent interface instead of concrete Event class
- **Enhanced Validation**: EventFactory provides built-in payload validation with RequiredFor attributes
- **Null Payload Events**: Delete operations now use CreateNullPayloadEvent() method for cleaner implementation

Stay tuned for additional features and improvements.