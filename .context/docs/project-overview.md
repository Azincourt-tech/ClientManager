# Project Overview

## Purpose and Goals
ClientManager is an application built with .NET Core and RavenDB, exploring Domain-Driven Design (DDD) patterns for an e-commerce or shop-based platform.

## Key Technologies & Architectural Features
- .NET (C#)
- RavenDB (NoSQL & Attachments)
- **Unit of Work** (Scoped RavenDB Sessions)
- **Traceability** (Native Revisions for Auditing)
- **Optimistic Concurrency Control**
- Manual Mapping (Extension Methods)
- FluentValidation
- ProblemDetails (Global Exception Handling)
- xUnit & Moq

## Getting Started
Load `ClientManager.sln` in VS, ensure a local or cloud RavenDB instance is available, and set the connection string in the Api project's `appsettings.json`.

