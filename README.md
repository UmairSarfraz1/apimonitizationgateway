# API Monetization Gateway

A .NET 8 based API monetization gateway that handles rate limiting, usage tracking, and billing.

## Features

- Dynamic tier configuration (Free, Pro)
- Rate limiting per second
- Monthly quota enforcement
- API usage tracking
- Monthly billing summaries
- Docker support

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Docker
  
### Running with Docker

1. Clone the repository
2. Run to build: `docker build -f ApiMonetizationGateway.API/Dockerfile -t monetization-api .`
3. Run to a Docker container: `docker run -p 8080:8080 monetization-api`
4. The API will be available at `http://localhost:8080`

### API Usage
APIs
- /api/Test
- /api/GetUserMonthlyUsageReport
- /api/GetUserMonthlyQuota

### Users
- Free Tier - Api Key - asftdtyfqwy2332jb423ui4b3u2b324
- Pro Tier - Api Key - qwftwefqwy2332jb423ui4b3u2b334

### Database setup
-- Run CMD from Infrastructure project - `update-database` 

```bash
# Make requests with API key
curl -H "X-API-Key: your-api-key" http://localhost:8080/api/Test
```


