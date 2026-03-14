# Document Validation & Customer Verification

This guide explains the rules for document categorization, file upload security, and automatic customer verification status.

## Document Categorization
Documents are categorized using the `DocumentType` enum:
- `Identity`: Identification document (RG, CNH).
- `AddressProof`: Proof of address (Utility bill).
- `SignedContract`: General signed contracts.
- `SocialContract`: Mandatory for Legal Entities (PJ).
- `Other`: General attachments.

## Customer Verification Rules
The system automatically evaluates a customer's `Status` based on uploaded documents:

### Individual (PF)
- **Verified**: Must have at least one `Identity` document AND one `AddressProof` document. No document should be expired.
- **Attention**: If ANY document is expired.
- **Active**: If documents are present but don't fulfill the "Verified" requirements.

### Legal Entity (PJ)
- **Verified**: Must have at least one `Identity` document AND one `AddressProof` document AND one `SocialContract` document. No document should be expired.
- **Attention**: If ANY document is expired.
- **Active**: If documents are present but don't fulfill the "Verified" requirements.

## File Upload Policy
All files attached via `DocumentApplication` undergo validation:
- **Allowed Extensions**: `.pdf`, `.png`, `.jpg`, `.jpeg`.
- **Max File Size**: 5MB.
- **Validation Interface**: `IFileValidator` is used to enforce these rules.

## Expiry Dates
Documents can have an optional `ExpiryDate`. If the current date is past the `ExpiryDate`, the document is considered expired, which triggers an `Attention` status for the customer during status evaluation.

## Manual Trigger
The endpoint `POST /api/Customer/customers/{id}/verify` can be used to manually re-evaluate the verification status.

