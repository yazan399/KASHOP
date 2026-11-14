# KASHOP API Postman test guide

Import `postman/KASHOP.postman_collection.json` into Postman, select the **KASHOP local** environment, and start the API. The collection uses `https://localhost:7158` by default; change `baseUrl` if your launch profile uses another URL.

## Roles and tokens

Run these requests first, in this order. Their test scripts save the returned JWT into the matching environment variable.

| Request | Seeded account | Permission to verify |
|---|---|---|
| `Authentication/Login - Admin` | `admin1@example.com` / `Password123!` | catalogue writes, all-orders, users, status update |
| `Authentication/Login - User` | `user1@example.com` / `Password123!` | own cart, checkout, own orders |
| `Authentication/Login - Super Admin` | `superadmin1@example.com` / `Password123!` | same elevated endpoints as Admin |

The seeded accounts are email-confirmed. For a newly registered account, execute **Register**, open the confirmation link received by email (outside Postman), then use **Login - User** with its credentials. Registration assigns the `User` role automatically.

All protected requests use a Bearer token from the environment. Re-run the appropriate login whenever a request returns `401`. Use the User token against an admin endpoint to confirm it returns `403`.

## Collection execution order

1. Run public category/product list and details requests. They intentionally work without a token.
2. Login as Admin, then create a category and product. The tests save `categoryId` and `productId`.
3. Login as User, add that product to the cart, inspect/update the cart, then checkout with **Cash**. This saves `orderId`.
4. Login as Admin and run the all-orders requests; update the saved order from `Pending` to `Confirmed`.
5. Run delete requests last, because they remove the product and category created by this run.

For Visa, checkout returns `paymentUrl`; complete it in Stripe and then call `GET /api/Orders/payment/success?sessionId=...` with Stripe's session id. The cancel endpoint is a display-only callback and can be called with any `sessionId` for route testing.

## Query parameters and filters

Every collection response has `data.data`, `data.totalCount`, `data.pageNumber`, `data.pageSize`, `data.totalPages`, `data.hasPreviousPage`, and `data.hasNextPage` for a collection endpoint. `pageNumber` starts at 1 and `pageSize` is 1–100; invalid values return HTTP 400.

| Endpoint | Common fields | Filters | `sortBy` values |
|---|---|---|---|
| `GET /api/Products` | `pageNumber`, `pageSize`, `search`, `sortDescending` | `categoryId`, `minPrice`, `maxPrice`, `inStock` | `Id`, `Name`, `Price`, `Discount`, `Quantity`, `Rate`, `CreatedOn` |
| `GET /api/Categories` | common | `search` | `Id`, `Name`, `CreatedOn` |
| `GET /api/Carts` | common | `search` product name | `ProductName`, `Price`, `Count`, `Total` |
| `GET /api/Orders` and `/api/Orders/admin` | common | `status`, `paymentStatus`, `paymentMethod`, `fromDate`, `toDate`, `search` (order id, phone, city) | `Id`, `OrderDate`, `TotalAmount`, `Status`, `PaymentStatus` |
| `GET /api/Account/users` | common | `emailConfirmed`, `search` | `Id`, `UserName`, `Email`, `FullName` |

Enum examples: `status=Pending`, `paymentMethod=Cash`, and `paymentStatus=Paid`. Example product URL: `{{baseUrl}}/api/Products?pageNumber=1&pageSize=10&search=chair&categoryId=1&minPrice=20&maxPrice=500&inStock=true&sortBy=Price&sortDescending=true`.

## Validation checks

The **Validation** folder contains intentionally invalid requests. Expected behavior is HTTP 400 with an `errors` object for request-model validation. Business validation (missing category, duplicate translation language, stock too low, an invalid status transition) returns a descriptive API error. Required fields, email, phone, range, maximum length, page limits, dates, and duplicate translation languages are guarded.
