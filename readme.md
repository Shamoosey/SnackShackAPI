API for SnackShack Web Site



dotnet ef migrations add
dotnet ef database update

## How to run the project
create appsettings.json file in the root directory of the project and add the following content:
```json
{
  "Application": {
    "Name": "SnackShackAPI",
    "Port": 50501
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AngularAppUrl": "http://localhost:4200",
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Discord": {
    "ClientId": "",
    "ClientSecret": "",
    "RedirectUri": "",
    "RequiredServerId": ""
  },
  "Data": {
    "DefaultAccounts": [
      {
        "StartingAmount": 10,
        "CurrencyCode": "MIL",
        "Name": "Millions"
      },
      {
        "StartingAmount": 3,
        "CurrencyCode": "BWL",
        "Name": "Bowls"
      },
      {
        "StartingAmount": 0,
        "CurrencyCode": "WOD",
        "Name": "Da wood"
      }
    ]
  },
  "Jwt": {
    "Secret": "",
    "Issuer": "",
    "Audience": ""
  }
}
```