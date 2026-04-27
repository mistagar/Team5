# Team5 Hackathon API

## Overview
This repository contains the backend foundation for Team 5’s hackathon solution, built with Clean Architecture principles across:
- `Team5Hackathon.API`
- `Team5Hackathon.Application`
- `Team5Hackathon.Domain`
- `Team5Hackathon.Infrastructure`
- `Team5Hackathon.Tests`

---

## Security Foundation Implemented

### 1. Audit Logging
A reusable audit logging flow has been implemented.

#### Components
- `AuditLog` domain entity
- `IAuditLogRepository` contract
- `AuditService` (`IAuditService`) in Application layer
- EF Core persistence through `AppDbContext` and `AuditLogRepository`

#### Current Auto-Audited Events
- Unauthorized API access attempts (invalid/missing API key)
- Unhandled API exceptions

---

### 2. API Security Middleware Pipeline
A middleware-first API protection pipeline has been added.

#### Middleware
1. Correlation ID middleware (`X-Correlation-Id`)
2. Security headers middleware
3. Global exception middleware
4. API key authentication middleware

#### Behavior
- Public paths such as `/health` and swagger are allowed.
- Protected endpoints require API key header validation.
- Unauthorized requests return `401` and are audited.
- Unhandled exceptions return `500` and are audited.

---

## Configuration

Set these values in configuration (or environment variables / user secrets):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  },
  "ApiSecurity": {
    "HeaderName": "X-Api-Key",
    "ApiKey": "YOUR_SECURE_API_KEY"
  }
}
