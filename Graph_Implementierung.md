# Microsoft Graph Implementierung

## Ziel
Umstellung der Anwendung von lokalen JSON-Daten auf Live-Daten aus **Azure Active Directory (Microsoft 365)** mithilfe der **Microsoft Graph API**.

Dank Repository Pattern sind **keine Änderungen am Frontend oder an den Controllern notwendig**.

---

## Schritt 1: Azure App Registrierung

Voraussetzung: Zugriff auf das Azure Portal (Microsoft Entra ID).

1. App Registration erstellen
2. Client Secret erzeugen
3. API-Berechtigung hinzufügen:
   - `User.Read.All` (Application)
4. **Admin Consent** erteilen
5. Notieren:
   - TenantId
   - ClientId
   - ClientSecret

---

## Schritt 2: Backend Konfiguration

Eintrag in `WebAPI/appsettings.json`:

```json
{
  "AzureAd": {
    "TenantId": "IHRE-GUID-HIER",
    "ClientId": "IHRE-GUID-HIER",
    "ClientSecret": "IHR-SECRET-HIER"
  }
}
```

---

## Schritt 3: NuGet Pakete installieren

Im Projekt **WebAPI**:

- `Microsoft.Graph`
- `Azure.Identity`

---

## Schritt 4: GraphUserService implementieren

Datei: `WebAPI/Services/GraphUserService.cs`

```csharp
using Application;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace WebAPI.Services;

public class GraphUserService : IUserService
{
    private readonly GraphServiceClient _graphClient;

    public GraphUserService(GraphServiceClient graphClient)
    {
        _graphClient = graphClient;
    }

    public async Task<List<Application.User>> SearchAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return new();

        var result = await _graphClient.Users.GetAsync(config =>
        {
            config.QueryParameters.Filter =
                $"startswith(displayName, '{term}') or startswith(mail, '{term}')";
            config.QueryParameters.Select = new[]
            {
                "id", "displayName", "mail", "mobilePhone",
                "jobTitle", "department", "officeLocation", "employeeId"
            };
            config.QueryParameters.Top = 10;
        });

        return result?.Value?.Select(MapToAppUser).ToList() ?? new();
    }

    public async Task<Application.User?> GetByIdAsync(string id)
    {
        var user = await _graphClient.Users[id].GetAsync(config =>
        {
            config.QueryParameters.Select = new[]
            {
                "id", "displayName", "mail", "mobilePhone",
                "jobTitle", "department", "officeLocation", "employeeId"
            };
        });

        return user != null ? MapToAppUser(user) : null;
    }

    private Application.User MapToAppUser(Microsoft.Graph.Models.User graphUser)
    {
        return new Application.User
        {
            Id = graphUser.Id ?? string.Empty,
            Name = graphUser.DisplayName ?? "Unbekannt",
            Email = graphUser.Mail ?? string.Empty,
            Phone = graphUser.MobilePhone ?? string.Empty,
            Role = graphUser.JobTitle ?? "N/A",
            Department = graphUser.Department ?? string.Empty,
            Faculty = graphUser.OfficeLocation ?? string.Empty,
            IdentificationNumber = graphUser.EmployeeId ?? "N/A"
        };
    }
}
```

---

## Schritt 5: Aktivierung (Service-Switch)

### Program.cs anpassen

**Using-Statements:**
```csharp
using Microsoft.Graph;
using Azure.Identity;
```

**Graph Client registrieren (vor `AddControllers`)**:
```csharp
builder.Services.AddScoped<GraphServiceClient>(sp =>
{
    var settings = builder.Configuration.GetSection("AzureAd");
    var credential = new ClientSecretCredential(
        settings["TenantId"],
        settings["ClientId"],
        settings["ClientSecret"]);

    return new GraphServiceClient(credential);
});
```

**Service austauschen:**

```csharp
// Alt
builder.Services.AddScoped<IUserService, FileUserService>();

// Neu
builder.Services.AddScoped<IUserService, GraphUserService>();
```

---

## Ergebnis
Nach dem Neustart der WebAPI werden die Benutzerdaten direkt aus **Azure Active Directory** geladen.

✔ Frontend unverändert
✔ Controller unverändert
✔ Datenquelle flexibel austauschbar
