# ASP.Net Core Web API Demo

This is simple ASP.Net Core 3.1 Web API Application having some common features that required by allmost Web API app.

Feel free to Fork/PR.

### Features

---

-   [x] ASP.Net Core 3.1
    -   [x] Model Validations
    -   [x] Dynamic Service Register
    -   [x] Localization
-   [x] Authentication
    -   [x] Asp.Net Core Identity
    -   [x] JWT based Auth
    -   [x] Custom User/Roles
-   [x] Entity Framework Core 3.1
    -   [x] Code First Approach
    -   [x] Unit of Work with Repository
    -   [x] Automatic Seeding
-   [x] Third-party
    -   [x] Swagger
    -   [x] Auto Mapper
    -   [x] Logs with NLog
    -   [x] Mail send with SendGrid
-   [x] Deploy
    -   [x] CI GitHub Actions
    -   [x] Deploy on Heroku
    -   [x] Use Docker

### Commands for SQL Server Migration

---

1. Open src folder
    ```
    cd src
    ```
2. Add Migration
    ```
    dotnet ef migrations add <Migration name> -o "Migrations" -p "DemoApp.EntityFramework" -s "DemoApp.API" -c "AppDBContext"
    ```
3. Update Database
    ```
    dotnet ef migrations update <Migration name> -o "Migrations" -p "DemoApp.EntityFramework" -s "DemoApp.API" -c "AppDBContext"
    ```
4. Remove Last Migration
    ```
    dotnet ef migrations remove -p "DemoApp.EntityFramework" -s "DemoApp.API" -c "AppDBContext"
    ```
5. Generate full Script for all Migrations
    ```
    dotnet ef migrations script -i -v -o script.sql -o "Migrations" -p "DemoApp.EntityFramework" -s "DemoApp.API" -c "AppDBContext"
    ```

### Project Dependency

---

                   API
                    ↑
                 Business
                    ↑
                DataAccess
                    ↑
                AutoMapper
                    ↑
                  Models
             🡕              🡔
    EntityFramework     Localization

### Setup

---

1. Download this Repo.
2. Update AppSettings.json file Under API project.
3. Create App on Heroku.
4. Add Secrets in GitHub.
    - HEROKU_API_KEY
    - HEROKU_APP_NAME
5. Create & Push repo on Github.
6. Open your Heroku App page.

That's it! 😎
