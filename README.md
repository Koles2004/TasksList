# TasksList API

A RESTful Web API for managing task lists built with **.NET 10**, **ASP.NET Core**, and **MongoDB**.  
It supports creating, reading, updating, and deleting task lists, as well as sharing lists between users.

---

## Table of Contents

- [Architecture](#architecture)
- [Projects](#projects)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [API Reference](#api-reference)
- [Error Handling](#error-handling)
- [Running Tests](#running-tests)

---

## Architecture

The solution follows a **Clean Architecture** pattern and is divided into four layers:

```
┌─────────────────────────────────────┐
│           TasksList.API             │  HTTP layer – controllers, DTOs, middleware
├─────────────────────────────────────┤
│       TasksList.Application         │  Business logic – services, interfaces, exceptions
├─────────────────────────────────────┤
│       TasksList.Infrastructure      │  Data access – MongoDB repository
├─────────────────────────────────────┤
│         TasksList.Domain            │  Core entities
└─────────────────────────────────────┘
```

Dependencies flow inward: `API → Application ← Infrastructure`, `Domain` has no dependencies.

---

## Projects

| Project | Description |
|---|---|
| `TasksList.Domain` | `TaskList` entity |
| `TasksList.Application` | `ITaskListService`, `TaskListService`, `ITaskListRepository`, `PagedResult<T>`, exceptions |
| `TasksList.Infrastructure` | `MongoTaskListRepository`, `TaskListDocument`, `MongoDbSettings` |
| `TasksList.API` | ASP.NET Core controllers, DTOs, `ExceptionHandlingMiddleware`, Swagger |
| `TasksList.Tests` | xUnit unit tests for `TaskListService` using Moq |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A running [MongoDB](https://www.mongodb.com/try/download/community) instance (local or remote)

---

## Configuration

The API is configured via `appsettings.json`. Set your MongoDB connection details under the `MongoDb` section:

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "taskslist",
    "TaskListsCollectionName": "task_lists"
  }
}
```

You can override these values with environment variables using the standard ASP.NET Core convention:

```
MongoDb__ConnectionString=mongodb://localhost:27017
MongoDb__DatabaseName=taskslist
MongoDb__TaskListsCollectionName=task_lists
```

---

## Running the Application

```powershell
cd TasksList.API
dotnet run
```

Swagger UI is available at:

```
http://localhost:<port>/swagger
```

---

## API Reference

All routes are prefixed with `/task-lists`.  
`userId` identifies the requesting user and is required on every endpoint (query string or body).

### Task Lists

| Method | Route | Description |
|---|---|---|
| `POST` | `/task-lists` | Create a new task list |
| `GET` | `/task-lists?userId=&page=&pageSize=` | Get all task lists for a user (paginated) |
| `GET` | `/task-lists/{id}?userId=` | Get a task list by ID |
| `PUT` | `/task-lists/{id}?userId=` | Update a task list name |
| `DELETE` | `/task-lists/{id}?userId=` | Delete a task list (owner only) |

### Sharing

| Method | Route | Description |
|---|---|---|
| `POST` | `/task-lists/{id}/shares?userId=` | Share a task list with another user |
| `GET` | `/task-lists/{id}/shares?userId=` | Get all shared user IDs |
| `DELETE` | `/task-lists/{id}/shares/{sharedUserId}?userId=` | Remove a shared user |

### Request / Response examples

**Create task list** `POST /task-lists`
```json
{
  "name": "Shopping List",
  "ownerId": "user-123"
}
```

**Response `201 Created`**
```json
{
  "id": "664f1a2b3c4d5e6f7a8b9c0d",
  "name": "Shopping List",
  "ownerId": "user-123",
  "createdAt": "2025-01-01T12:00:00Z",
  "sharedUserIds": []
}
```

**Get paginated task lists** `GET /task-lists?userId=user-123&page=1&pageSize=20`
```json
{
  "items": [
    { "id": "664f1a2b3c4d5e6f7a8b9c0d", "name": "Shopping List" }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 20
}
```

**Add shared user** `POST /task-lists/{id}/shares?userId=user-123`
```json
{
  "userId": "user-456"
}
```

---

## Error Handling

All errors are returned as JSON by the global `ExceptionHandlingMiddleware`:

```json
{
  "error": "Task list 'xyz' not found."
}
```

| HTTP Status | Cause |
|---|---|
| `404 Not Found` | Task list does not exist |
| `403 Forbidden` | User is not the owner or a shared member |
| `500 Internal Server Error` | Unexpected server error |

---

## Running Tests

```powershell
dotnet test
```

The test project (`TasksList.Tests`) contains **18 unit tests** covering all `TaskListService` methods:

- Happy-path scenarios (owner and shared-user access)
- `NotFoundException` when a task list does not exist
- `ForbiddenException` when the requesting user lacks permission
- Repository call verification

Tests use **xUnit** and **Moq** — no running MongoDB instance is required.
