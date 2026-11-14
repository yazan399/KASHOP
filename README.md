# KASHOP

KASHOP is a modular e-commerce REST API built with ASP.NET Core and .NET 9. It provides account management, localized categories and products, image uploads, shopping carts, checkout, Stripe payments, order tracking, and role-based administration.

## Features

- JWT authentication and authorization
- Registration, login, email confirmation, and seeded users/roles
- Product and category management with Arabic/English translations
- Product image uploads through Cloudinary
- Pagination, searching, filtering, and sorting
- Shopping cart management
- Cash and Stripe card checkout
- Stripe payment success and cancellation callbacks
- User order history and order details
- Admin order filtering and status transitions
- Audit fields for created/updated timestamps and users
- Global exception handling and localized responses
- Postman collection for API testing

## Technology stack

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9
- SQL Server
- ASP.NET Core Identity
- JWT Bearer authentication
- Mapster
- Stripe.net
- CloudinaryDotNet
- MailKit

## Architecture

The solution is separated into three projects:

```text
KASHOP.PL      Presentation layer: controllers, middleware, configuration, seeding
KASHOP.BLL     Business layer: services, validation, mapping, checkout logic
KASHOP.DAL     Data layer: entities, DTOs, EF Core DbContext, repositories, migrations
```

The application uses a repository/unit-of-work approach. Controllers call business services, services use repositories, and Entity Framework Core persists data to SQL Server.

## Requirements

- .NET 9 SDK
- SQL Server or SQL Server Express
- A configured SMTP account for email confirmation
- Cloudinary account for product images
- Stripe account and test API keys for card checkout
- Postman, optional but recommended for testing

## Getting started

```bash
git clone https://github.com/yazan399/KASHOP.git
cd KASHOP
dotnet restore
dotnet build
```

Update the SQL Server connection string in `KASHOP.PL/appsettings.json`, or use .NET User Secrets for local development. Do not commit passwords, API keys, SMTP credentials, or production connection strings.

Apply the existing migrations:

```bash
dotnet ef database update --project KASHOP.DAL --startup-project KASHOP.PL
```

Run the API:

```bash
dotnet run --project KASHOP.PL
```

The exact HTTP/HTTPS URLs are printed by ASP.NET Core when the application starts. Use the active URL as the Postman collection base URL.

## Configuration

Keep secrets in User Secrets or environment variables:

```bash
dotnet user-secrets --project KASHOP.PL set "ConnectionStrings:DefaultConnection" "Server=.;Database=KASHOP14;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet user-secrets --project KASHOP.PL set "ApiSettings:Issuer" "KASHOP"
dotnet user-secrets --project KASHOP.PL set "ApiSettings:Audience" "KASHOP_CLIENT"
dotnet user-secrets --project KASHOP.PL set "ApiSettings:SecretKey" "replace-with-a-long-random-secret"
dotnet user-secrets --project KASHOP.PL set "Stripe:SecretKey" "sk_test_replace-me"
dotnet user-secrets --project KASHOP.PL set "Stripe:PublishableKey" "pk_test_replace-me"
```

Configure SMTP and Cloudinary values using the same User Secrets mechanism. The required setting names can be found in `EmailSender` and `FileService`.

## Seeded development accounts

The development seeders create roles and sample users when the application starts. Change these credentials before using a shared or production environment.

| Role | Email | Password |
| --- | --- | --- |
| SuperAdmin | `superadmin1@example.com` | `Password123!` |
| Admin | `admin1@example.com` | `Password123!` |
| User | `user1@example.com` | `Password123!` |

## Main API routes

Protected routes require:

```http
Authorization: Bearer <jwt-token>
```

### Account

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/Account/register` | Register a user |
| POST | `/api/Account/login` | Login and receive a JWT |
| GET | `/api/Account/ConfirmEmail` | Confirm a user's email |
| GET | `/api/Account/users` | List users |

### Categories

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/Categories` | List categories |
| GET | `/api/Categories/{id}` | Get one category |
| POST | `/api/Categories` | Create a category |
| DELETE | `/api/Categories/{id}` | Delete a category |

### Products

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/Products` | List products with pagination/search |
| GET | `/api/Products/{id}` | Get one product |
| POST | `/api/Products` | Create a product with multipart form data |
| DELETE | `/api/Products/{id}` | Delete a product |

Product listing supports query parameters such as `pageNumber`, `pageSize`, search text, filters, and sorting options defined by the request DTOs.

### Cart

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/Carts` | Add a product to the current user's cart |
| GET | `/api/Carts` | Get the current user's cart |
| PUT | `/api/Carts/{productId}` | Update item quantity |
| DELETE | `/api/Carts/{productId}` | Remove a product |
| DELETE | `/api/Carts/clear` | Clear the cart |

### Orders and checkout

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/Orders/checkout` | Create a cash or Stripe checkout order |
| GET | `/api/Orders/payment/success` | Confirm a successful Stripe payment |
| GET | `/api/Orders/payment/cancel` | Handle a cancelled Stripe payment |
| GET | `/api/Orders` | List the current user's orders |
| GET | `/api/Orders/{orderId}` | Get order details |
| GET | `/api/Orders/admin` | Admin: list orders with an optional status filter |
| GET | `/api/Orders/admin/status` | Admin: list orders by status |
| PATCH | `/api/Orders/admin/{orderId}/status` | Admin: update order status |

Allowed order transitions are enforced by the business layer. Admin routes require the `Admin` or `SuperAdmin` role.

## Postman testing

Import [`postman/KASHOP.postman_collection.json`](postman/KASHOP.postman_collection.json) into Postman. Start the API first, set the collection base URL to the running API URL, then run the account flow to obtain a JWT before testing protected endpoints.

Additional testing guidance:

- [`POSTMAN_TESTING.md`](POSTMAN_TESTING.md)
- [`POSTMAN_COLLECTION_STRUCTURE.md`](POSTMAN_COLLECTION_STRUCTURE.md)

## Database migrations

```bash
dotnet ef migrations add MigrationName --project KASHOP.DAL --startup-project KASHOP.PL
dotnet ef database update --project KASHOP.DAL --startup-project KASHOP.PL
```

## Security notes

- Never commit Stripe keys, JWT signing keys, SMTP passwords, Cloudinary secrets, or production database credentials.
- Replace seeded development passwords before deployment.
- Use HTTPS in production.
- Use Stripe test keys only in development and configure verified webhook handling before production payments.
- Restrict administrative endpoints with the appropriate roles.

## License

No license has been specified yet. Add a license before distributing the project publicly.
