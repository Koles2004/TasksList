# taskslist-api-provider

A TypeScript client library for the TasksList REST API. Provides a typed `TaskListProvider` class for all task-list operations, a factory function for creating a pre-configured Axios instance, and a typed `ApiError` class for structured error handling.

---

## Installation

```bash
npm install
```

---

## Build

```bash
npm run build        # compile once → dist/
npm run dev          # watch mode
```

---

## Quick start

```ts
import { createApiClient, TaskListProvider, ApiError } from './src/index';

const client = createApiClient({ baseUrl: 'http://localhost:5077' });
const provider = new TaskListProvider(client);
```

Run the bundled example against a locally running API:

```bash
npm run example
```

---

## API reference

### `createApiClient(config)`

Creates and returns a configured Axios instance with JSON headers, a response interceptor that converts HTTP errors into `ApiError`, and an optional timeout.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `baseUrl` | `string` | — | Base URL of the TasksList API |
| `timeoutMs` | `number` | `10000` | Request timeout in milliseconds |

---

### `TaskListProvider`

All methods are `async` and throw `ApiError` on non-2xx responses.

#### `create(request)`

```ts
create(request: CreateTaskListRequest): Promise<TaskList>
```

Creates a new task list. `ownerId` is required.

#### `getById(id, userId)`

```ts
getById(id: string, userId: string): Promise<TaskList>
```

Returns a single task list by its ID. The caller must be the owner or a shared user.

#### `getPaged(params)`

```ts
getPaged(params: GetPagedParams): Promise<PagedResponse<TaskListSummary>>
```

Returns a paginated list of task-list summaries visible to `userId`.

| Param | Type | Default |
|---|---|---|
| `userId` | `string` | — |
| `page` | `number` | `1` |
| `pageSize` | `number` | `20` |

#### `update(id, userId, request)`

```ts
update(id: string, userId: string, request: UpdateTaskListRequest): Promise<TaskList>
```

Updates the name of an existing task list. Only the owner can update.

#### `delete(id, userId)`

```ts
delete(id: string, userId: string): Promise<void>
```

Deletes a task list. Only the owner can delete.

#### `addSharedUser(id, userId, request)`

```ts
addSharedUser(id: string, userId: string, request: AddSharedUserRequest): Promise<void>
```

Grants another user read access to the task list.

#### `getSharedUsers(id, userId)`

```ts
getSharedUsers(id: string, userId: string): Promise<string[]>
```

Returns the list of user IDs that have been granted access.

#### `removeSharedUser(id, sharedUserId, userId)`

```ts
removeSharedUser(id: string, sharedUserId: string, userId: string): Promise<void>
```

Revokes access for a previously shared user.

---

## Error handling

All HTTP errors are thrown as `ApiError`:

```ts
try {
  await provider.getById('unknown-id', 'user1');
} catch (error) {
  if (error instanceof ApiError) {
    console.error(error.statusCode); // e.g. 404
    console.error(error.isNotFound);    // true
    console.error(error.isForbidden);   // false
    console.error(error.isServerError); // false
  }
}
```

| Property | Type | Description |
|---|---|---|
| `statusCode` | `number` | HTTP status code |
| `isNotFound` | `boolean` | `statusCode === 404` |
| `isForbidden` | `boolean` | `statusCode === 403` |
| `isServerError` | `boolean` | `statusCode >= 500` |

---

## Type reference

```ts
interface TaskList {
  id: string;
  name: string;
  ownerId: string;
  createdAt: string;
  sharedUserIds: string[];
}

interface TaskListSummary {
  id: string;
  name: string;
}

interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

interface CreateTaskListRequest { name: string; ownerId: string; }
interface UpdateTaskListRequest { name: string; }
interface AddSharedUserRequest  { userId: string; }
```

---

## Project structure

```
src/
  index.ts                 # public re-exports
  config/
    apiClient.ts           # Axios factory + error interceptor
  errors/
    ApiError.ts            # typed HTTP error class
  provider/
    TaskListProvider.ts    # CRUD + sharing methods
  types/
    index.ts               # shared request/response types
example.ts                 # runnable usage demo
```

---

## Requirements

- Node.js 18+
- TypeScript 5.4+
- A running instance of the TasksList API (default: `http://localhost:5077`)
