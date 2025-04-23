# Ecommerce.CustomerApp

## Project Structure

```
CustomerApp/
│
├── Controllers/
│   └── CategoryController.cs
│
├── Services/
│   ├── ApiClients/
│   │   ├── Interfaces/
│   │   │   └── ICategoryApiClient.cs
│   │   └── CategoryApiClient.cs
│   │
│   ├── Interfaces/
│   │   └── ICartService.cs
│   │
│   ├── CartService.cs
│   └── SessionService.cs
│
├── ViewModels/   (Optional: If specific UI ViewModels are needed)
│   └── LocalCategoryViewModel.cs
│
├── Views/
│   └── Category/
│       ├── Index.cshtml
│       └── Details.cshtml
│
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
│
├── Helpers/
│   └── PriceFormatter.cs
│
├── wwwroot/
│
├── Program.cs
├── appsettings.json
└── Ecommerce.CustomerApp.csproj
```

## Layer Responsibilities

| Layer / Folder       | Responsibilities                                                                 |
|-----------------------|---------------------------------------------------------------------------------|
| **Controllers/**      | Handles Razor UI navigation, calls ApiClient, and passes data to the View.      |
| **Services/ApiClients** | Communicates with the API Layer using `HttpClient` to call endpoints.          |
| **Services/**         | Provides UI-related logic: cart, session, cookies, and other UI utilities.      |
| **ViewModels/**       | (Optional) Specific ViewModels for Razor if different from Shared ViewModels.   |
| **Views/**            | Razor Views for displaying data.                                                |
| **Middleware/**       | Handles HTTP pipeline and global exception UI (optional).                       |
| **Helpers/**          | Utility functions: formatting prices, dates, and simple UI logic.               |
| **wwwroot/**          | Static files: CSS, JS, images, etc.                                             |

## Shared Library: `Ecommerce.Shared`

- **ViewModels Shared**: Shared between API and CustomerApp.
- **PagedResult**: Used for paginated data transfer via JSON.
- Contains no business logic – only models for data communication.

## Dependency Flow

### CustomerApp:
- References only `Ecommerce.Shared`.
- Does not depend on `Ecommerce.Application` or its business logic.

### API Layer:
- Calls services → returns Shared ViewModels → JSON response.
- Does not expose DTOs externally.

### Clean Flow Summary:
1. `HttpClient` → Calls API Layer → Receives Shared ViewModel JSON.
2. Controller → Processes data → Passes it to the View.
3. No business logic in `CustomerApp`, only UI logic.
4. `PagedResult` + Shared ViewModels = Standard communication between API ↔ CustomerApp.
