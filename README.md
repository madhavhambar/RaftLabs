﻿# RaftLabUsers.Core (.NET Core)

A reusable .NET class library that interacts with the public [https://reqres.in](https://reqres.in) API to fetch and manage user data. This simulates integration with an external provider.

---

## 📦 Features

- Fetch user details by ID
- Fetch all users across pages (pagination handled internally)
- Typed models (DTOs + domain models)
- HTTP client abstraction
- In-memory caching using IMemoryCache with expiration
- Error handling for 404, 5xx, timeouts, deserialization
- Retry policy using [Polly](https://github.com/App-vNext/Polly) for transient errors
- Configurable base URL via `appsettings.json`

---

## 🛠️ How to Build

> .NET core 8.0 is required.

```bash
# Clone repo
git clone https://github.com/madhavhambar/RaftLabs.git
cd RaftLabs

# Restore dependencies
dotnet restore

# Build all projects
dotnet build

cd RaftLabs.Console
dotnet run

```
> OR Open the solution RaftLabs.sln and run the project

## Video Walkthrogh
[![Video Title](https://img.youtube.com/vi/NTleHxdIpjs/0.jpg)](https://www.youtube.com/watch?v=NTleHxdIpjs)
