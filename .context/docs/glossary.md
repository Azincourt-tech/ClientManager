# Glossary & Domain Concepts

## Core Terms
- **Document**: The base entity representing a record stored in RavenDB.
- **Shop**: Core aggregate root representing a store in the domain.
- **RavenDB**: A NoSQL document database used by the application, optimized for .NET.
- **Customer**: Divided into **Individual (PF)** and **Legal Entity (PJ)**.
- **Customer Status**: Defines the verification state (`Active`, `Verified`, `Attention`, `Blocked`, `Inactive`).
- **Document Type**: Categorizes documents (e.g., `Identity`, `AddressProof`, `SocialContract`) for business rule enforcement.

## Personas / Actors
- **End User / Customer**: Individuals (PF) or Companies (PJ) navigating shops and submitting documents.
- **Admin**: Staff managing domain configurations and verifying data.

