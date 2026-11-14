# KASHOP Postman collection structure

Create a collection named **KASHOP API — Complete Role Tests**. Add these collection variables:

| Variable | Initial value |
|---|---|
| `baseUrl` | `https://localhost:7158` |
| `adminToken` | empty |
| `superAdminToken` | empty |
| `userToken` | empty |
| `categoryId` | empty |
| `productId` | empty |
| `orderId` | empty |
| `userId` | empty |
| `stripeSessionId` | empty |

Use `Authorization: Bearer {{adminToken}}`, `Bearer {{superAdminToken}}`, or `Bearer {{userToken}}` only on the request that needs it. Public catalogue requests deliberately have no Authorization header.

```text
KASHOP API — Complete Role Tests
│
├── 00. Setup and Authentication
│   ├── Register a User                         POST /api/Account/register
│   ├── Confirm email externally                Click the URL received by email
│   ├── Login — User                            POST /api/Account/login
│   ├── Login — Admin                           POST /api/Account/login
│   └── Login — Super Admin                     POST /api/Account/login
│
├── 01. Catalogue — Public / User / Admin / Super Admin
│   ├── Public
│   │   ├── List categories: page, search, sort GET /api/Categories
│   │   ├── Category by ID                      GET /api/Categories/{{categoryId}}
│   │   ├── Category by name                    GET /api/Categories/name/{name}
│   │   ├── List products: every filter/sort    GET /api/Products
│   │   ├── Product by ID                       GET /api/Products/{{productId}}
│   │   └── Product by name                     GET /api/Products/name/{name}
│   ├── User — repeat public reads with user token (expect 200)
│   ├── Admin
│   │   ├── Create category                     POST /api/Categories
│   │   ├── Create product (form-data)          POST /api/Products
│   │   ├── Delete product                      DELETE /api/Products/{{productId}}
│   │   └── Delete category                     DELETE /api/Categories/{{categoryId}}
│   └── Super Admin — repeat the four Admin write tests (expect 200)
│
├── 02. Cart — User / Admin / Super Admin
│   ├── User
│   │   ├── Add product                          POST   /api/Carts
│   │   ├── List cart: page, search, sort        GET    /api/Carts
│   │   ├── Cart item by product ID              GET    /api/Carts/{{productId}}
│   │   ├── Update quantity                      PUT    /api/Carts/{{productId}}
│   │   ├── Remove product                       DELETE /api/Carts/{{productId}}
│   │   └── Clear cart                           DELETE /api/Carts/clear
│   ├── Admin — repeat with admin token (tests the Admin's own cart)
│   └── Super Admin — repeat with Super Admin token (tests that user's own cart)
│
├── 03. Orders — User / Admin / Super Admin
│   ├── User
│   │   ├── Checkout cash                        POST /api/Orders/checkout
│   │   ├── Checkout Visa                        POST /api/Orders/checkout
│   │   ├── My orders: filters and sort          GET  /api/Orders
│   │   └── My order by ID                       GET  /api/Orders/{{orderId}}
│   ├── Public Stripe callbacks
│   │   ├── Payment success                      GET /api/Orders/payment/success
│   │   └── Payment cancel                       GET /api/Orders/payment/cancel
│   ├── Admin
│   │   ├── All orders: every filter/sort        GET   /api/Orders/admin
│   │   ├── Pending-orders shortcut              GET   /api/Orders/admin/status
│   │   ├── Any order by ID                      GET   /api/Orders/admin/{{orderId}}
│   │   └── Change order status                  PATCH /api/Orders/admin/{{orderId}}/status
│   └── Super Admin — repeat Admin order requests (expect 200)
│
├── 04. Users — User / Admin / Super Admin
│   ├── User: list users                         GET /api/Account/users (expect 403)
│   ├── Admin: list/filter/sort users            GET /api/Account/users
│   ├── Admin: user by ID                        GET /api/Account/users/{{userId}}
│   ├── Super Admin: list/filter/sort users      GET /api/Account/users
│   └── Super Admin: user by ID                  GET /api/Account/users/{{userId}}
│
└── 05. Validation and Authorization
    ├── Invalid paging (0 / 101)                 expect 400
    ├── Invalid price / count / missing image    expect 400
    ├── MinPrice greater than MaxPrice           expect 400
    ├── FromDate later than ToDate               expect 400
    ├── Invalid enum sort/status                 expect 400
    ├── No token on protected route              expect 401
    └── User token on Admin route                expect 403
```

## Exact conditional-query requests

Use a separate request for every row below. Change only one field at a time first, then run the combined request.

| Area | Request URL suffix | What it proves |
|---|---|---|
| Categories | `?pageNumber=1&pageSize=5&search=home&sortBy=Name&sortDescending=false` | name search + ascending name sort |
| Products – full | `?pageNumber=1&pageSize=10&search=chair&categoryId={{categoryId}}&minPrice=10&maxPrice=500&inStock=true&sortBy=Price&sortDescending=true` | search, category/price/stock filters, descending price |
| Products – every sort | `?sortBy=Id`, `Name`, `Price`, `Discount`, `Quantity`, `Rate`, `CreatedOn` | all product sort options |
| Cart | `?pageNumber=1&pageSize=10&search=chair&sortBy=Total&sortDescending=true` | product-name search and calculated-total sort |
| Orders | `?status=Pending&paymentStatus=Pending&paymentMethod=Cash&fromDate=2026-01-01&toDate=2026-12-31&search=Hebron&sortBy=TotalAmount&sortDescending=true` | enum/date/text filters and sort |
| Users | `?pageNumber=1&pageSize=10&search=admin&emailConfirmed=true&sortBy=Email&sortDescending=false` | user search, confirmation filter, sort |

Accepted sort values are enforced by the API: categories `Id, Name, CreatedOn`; products `Id, Name, Price, Discount, Quantity, Rate, CreatedOn`; carts `ProductName, Price, Count, Total`; orders `Id, OrderDate, TotalAmount, Status, PaymentStatus`; users `Id, UserName, Email, FullName`.

## Bodies to paste in Postman

Create category:

```json
{
  "translations": [{ "name": "Office", "language": "en-US" }]
}
```

Add to cart:

```json
{ "productId": {{productId}}, "count": 2 }
```

Update cart quantity:

```json
{ "count": 1 }
```

Cash checkout:

```json
{
  "paymentMethod": "Cash",
  "city": "Hebron",
  "street": "Main Street",
  "phoneNumber": "1234567890"
}
```

Update order status:

```json
{ "newStatus": "Confirmed" }
```

For product creation, use **Body → form-data**: `price`, `quantity`, `discount`, `categoryId`, `translations[0].name`, `translations[0].description`, `translations[0].language`, and a file-valued `mainImage`.

## Postman test scripts

Put this in each login request's **Tests** tab, changing `userToken` to the appropriate token variable:

```javascript
pm.test("Login returned a token", () => pm.response.to.have.status(200));
pm.environment.set("userToken", pm.response.json().token);
```

On Create Category, Create Product, Checkout, and List Users, save values with:

```javascript
const data = pm.response.json().data;
pm.environment.set("categoryId", data.id); // create category
pm.environment.set("productId", data.id);  // create product
pm.environment.set("orderId", data.id);    // checkout
pm.environment.set("userId", data.data[0].id); // list users: outer data then page data
```
