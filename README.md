# Azure Fundamentals

A set of .NET examples demonstrating Azure Blob Storage and Azure Functions. The solution contains two ASP.NET Core MVC applications and one .NET isolated-worker Azure Functions app.

## Projects

| Project | Description |
| --- | --- |
| `AzureBlobProject` | MVC interface for listing and managing Blob Storage containers and blobs. It supports uploading files with title/comment metadata, viewing and deleting blobs, and creating/deleting containers. Containers created by the app use container-level public blob access; use non-sensitive sample data only. |
| `AzureFunctionTangyWeb` | MVC sales application form. It submits sales-request details to an HTTP-triggered function and uploads an optional headshot to the `salesrep` blob container. |
| `TangyAzureFunc` | .NET isolated-worker Azure Functions app. It queues HTTP-submitted sales requests, saves queue messages to SQL Server, and resizes images uploaded to `salesrep` into 100 × 100 JPEGs in `salesrep-resized`. |

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

Ensure the configured database has the `SalesRequests` table expected by the app before processing queue messages. This repository does not include database migrations or a schema-creation script.

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
