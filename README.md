# DynaMod

Lab project for testing out a Unit Of Work/Session Manager for DynamoDb. 

The main goal is to play around with things like:
- Fluent Mapping for the models DB representation
- AWS Requests from scrath: Serialization and AWS Signature Version 4
- Source Generators, mostly for creating API Request/Response serializers
- Optimistic Locks using Condition Expressions
- Constraints using Condition Expressions
- Relationships. Although not very performatic for complex graphs of relationships, it is possible to model relational entities in dynamo.
- Sessions, implementing the Unit of Work pattern using Dynamo Transactions Api.
