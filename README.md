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


## Audio Streaming MVP

### Endpoint
`POST /api/stream/audio-chunk`

### Request Contract
```json
{
  "callId": "11111111-1111-1111-1111-111111111111",
  "sequence": 1,
  "chunkBase64": "AQIDBA==",
  "sentAtUtc": "2026-04-28T12:00:00Z"
}


{
  "success": true,
  "data": {
    "accepted": true,
    "callId": "11111111-1111-1111-1111-111111111111",
    "sequence": 1,
    "correlationId": "31826d3eed6c43ecacc0530e85038a0c"
  },
  "message": null,
  "errors": null
}


## New Update: Dynamic Audio Input for Transcription

### What Was Newly Implemented
We extended audio ingestion to support **dynamic input sources**:

1. `chunkBase64` input (existing flow)
2. `audioUrl` input (new flow)

The backend now normalizes both into audio bytes before transcription.

### New Behavior
- If `chunkBase64` is provided, it is used directly.
- If `chunkBase64` is empty and `audioUrl` is provided, backend downloads the audio and processes it.
- Supported URL audio file types now include:
  - `mp3`, `wav`, `webm`, `ogg`, `oga`, `m4a`, `mp4`, `flac`, `mpeg`, `mpga`

### Endpoint (same endpoint, enhanced behavior)
`POST /api/stream/audio-chunk`

### New Request Example (audioUrl mode)
```json
{
  "callId": "11111111-1111-1111-1111-111111111111",
  "sequence": 1,
  "chunkBase64": "",
  "audioUrl": "https://raw.githubusercontent.com/vicradon/semantic-kernel-demos/refs/heads/main/assets/sounds/70-years-old-man.mp3",
  "sentAtUtc": "2026-04-29T09:00:00Z"
}

{
  "callId": "11111111-1111-1111-1111-111111111111",
  "sequence": 2,
  "chunkBase64": "<base64-audio-content>",
  "audioUrl": "",
  "sentAtUtc": "2026-04-29T09:01:00Z"
}


Processing Enhancement
Worker now sends inferred fileName and contentType to Whisper.
This improves compatibility across different audio formats.
Result
The pipeline now supports both:

real-time chunk-based ingestion
URL-based audio ingestion for quick demos/tests
