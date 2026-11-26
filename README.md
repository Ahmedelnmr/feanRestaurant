# Fean Restaurant

An ASP.NET Core MVC restaurant web app: customers browse a menu, order and reserve tables, and an admin manages the menu and orders.

## Features

- Accounts: register and log in (ASP.NET Core Identity, session support).
- Menu: categories and menu items, with an admin screen to create items.
- Cart and orders: cart items, orders and order items.
- Table reservations.
- Daily menu item statistics for the admin.
- Separate Admin and User controllers.

## Tech stack

C#, .NET 8, ASP.NET Core MVC, Entity Framework Core (SQL Server), Identity, Razor views.

## Getting started

Prerequisites: .NET 8 SDK and SQL Server.

```bash
git clone https://github.com/Ahmedelnmr/feanRestaurant.git
cd feanRestaurant
# set ConnectionStrings:DefaultConnection in projectMvc/appsettings.json
dotnet ef database update --project projectMvc
dotnet run --project projectMvc
```

The app opens on the login page.
