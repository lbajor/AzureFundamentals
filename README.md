# Azure Fundamentals

A set of .NET examples demonstrating Azure Blob Storage and Azure Functions. The solution contains two ASP.NET Core MVC applications and one .NET isolated-worker Azure Functions app.

## Projects

| Project | Description |
| --- | --- |
| `AzureBlobProject` | MVC interface for listing and managing Blob Storage containers and blobs. It supports uploading files with title/comment metadata, viewing and deleting blobs, and creating/deleting containers. Containers created by the app use container-level public blob access; use non-sensitive sample data only. |
| `AzureFunctionTangyWeb` | MVC application with a sales submission form and a grocery-list interface. Sales requests are submitted to an HTTP-triggered function, with optional headshots uploaded to the `salesrep` blob container. Grocery-list operations call the `TangyAzureFunc` API. |
| `TangyAzureFunc` | .NET isolated-worker Azure Functions app. It provides grocery-list CRUD endpoints, queues HTTP-submitted sales requests, saves queue messages to SQL Server, and resizes images uploaded to `salesrep` into 100 × 100 JPEGs in `salesrep-resized`. |

## Grocery API

The Functions app exposes a grocery-list API at `http://localhost:7141/api/GroceryList` when run with the included launch profile. All endpoints use `AuthorizationLevel.Function` and require a Function key. The MVC grocery client currently does not send a Function key, so configure key forwarding in the client before using the integrated grocery UI.

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/GroceryList` | Get all grocery items. |
| `GET` | `/api/GroceryList/{id}` | Get an item by ID. |
| `POST` | `/api/GroceryList` | Create an item. Send a JSON body such as `{"name":"Milk"}`. |
| `PUT` | `/api/GroceryList/{id}` | Update an item's name. Send a JSON body such as `{"name":"Oat milk"}`. |
| `DELETE` | `/api/GroceryList/{id}` | Delete an item. |

## Prerequisites

- .NET 9 SDK for the MVC projects and .NET 10 SDK for `TangyAzureFunc`.
- Azure Functions Core Tools v4 to run the Functions app locally.
- An Azure Storage account, or a local Storage emulator such as Azurite, with Blob and Queue services available.
- A SQL Server database for sales-request persistence.

## Configuration

Do not commit credentials or connection strings. Supply the following settings using local user secrets, environment variables, or local-only configuration files:

| Project | Setting | Purpose |
| --- | --- | --- |
| `AzureBlobProject` | `BlobConnection` | Connection string for the Blob Storage account used by the blob-management app. |
| `AzureFunctionTangyWeb` | `StorageConnection` | Connection string for the account containing the `salesrep` container. |
| `TangyAzureFunc` | `AzureWebJobsStorage` | Storage connection used by the Functions host and Blob/Queue triggers. Use `UseDevelopmentStorage=true` when using a compatible local emulator. |
| `TangyAzureFunc` | `AzureSqlDatabase` | SQL Server connection string used by Entity Framework Core. |

For example, the Blob management project has a `UserSecretsId` configured, so its connection string can be stored outside the repository with:

```powershell
dotnet user-secrets --project .\AzureBlobProject\AzureBlobProject.csproj set "BlobConnection" "<storage-connection-string>"
```

For the Functions app, create `TangyAzureFunc\local.settings.json` locally (this file is ignored by Git) and add the settings under `Values`:

```json
{
  "IsEncrypted": false,
  "Values": {
	"AzureWebJobsStorage": "UseDevelopmentStorage=true",
	"FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
	"AzureSqlDatabase": "<sql-server-connection-string>"
  }
}
```

Ensure the configured database has the `SalesRequests` and `GroceryItems` tables expected by the app before processing queue messages or using the grocery API. This repository does not include database migrations or a schema-creation script.

`AzureFunctionTangyWeb` currently posts to `http://localhost:7141/api/OnSalesUploadWriteToQueue`. The Functions app's launch profile uses port `7141`; keep the function endpoint and web-app configuration in sync if you change the port or deploy the apps.

## Run locally

Restore the solution packages:

```powershell
dotnet restore .\AzureFundamentals.sln
```

Start the Blob management app in one terminal:

```powershell
dotnet run --project .\AzureBlobProject\AzureBlobProject.csproj
```

Start the Functions app in another terminal, with its local settings configured:

```powershell
cd .\TangyAzureFunc
func start
```

Start the sales web app in a third terminal:

```powershell
dotnet run --project .\AzureFunctionTangyWeb\AzureFunctionTangyWeb.csproj
```

Open the URL printed by the web app. Its launch profile uses `http://localhost:5026` and `https://localhost:7186`.

## Build

Build the complete solution with:

```powershell
dotnet build .\AzureFundamentals.sln
```
