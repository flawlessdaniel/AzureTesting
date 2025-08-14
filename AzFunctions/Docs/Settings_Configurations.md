# Guía de Configuración y Settings en Azure Functions (.NET)

Esta guía explica todo lo necesario para manejar **configuraciones** y **settings** en Azure Functions desarrolladas en **.NET**, tanto en entornos locales como en producción.

---

## 1. Conceptos clave

- **Application Settings**: Variables de configuración almacenadas en Azure Portal, accesibles como variables de entorno.
- **Connection Strings**: Cadenas de conexión para bases de datos, colas, etc.
- **local.settings.json**: Archivo local para desarrollo, **no** se sube a producción.
- **Azure App Configuration**: Servicio para centralizar configuraciones y gestionarlas de forma dinámica.
- **Key Vault**: Servicio para almacenar secretos de forma segura.

---

## 2. Configuración local (`local.settings.json`)

Usado para ejecutar la Function localmente.

Ejemplo:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "MyCustomSetting": "LocalValue",
    "ConnectionStrings:SqlDb": "Server=localhost;Database=TestDb;User Id=sa;Password=Pass123;"
  },
  "Host": {
    "CORS": "*",
    "LocalHttpPort": 7071
  }
}
```

⚠ **Importante**:  
- Este archivo **no** debe incluirse en control de versiones.
- Agregar a `.gitignore`.

---

## 3. Configuración en Azure Portal

En la Function App:
1. **Configuration → Application settings**
2. **Agregar** variables y valores.
3. Se accede con `Environment.GetEnvironmentVariable("NombreVariable")`.

Ejemplo:
```csharp
string value = Environment.GetEnvironmentVariable("MyCustomSetting");
```

---

## 4. Variables de entorno y jerarquía

Orden de prioridad (de menor a mayor):
1. **`host.json`** y configuraciones por defecto.
2. Variables definidas en `local.settings.json`.
3. Variables de entorno del sistema.
4. Variables en Azure Portal.
5. Variables inyectadas por Azure App Configuration o Key Vault.

---

## 5. `host.json` – Configuración global

Archivo para definir opciones de ejecución y bindings.

Ejemplo:
```json
{
  "version": "2.0",
  "logging": {
    "applicationInsights": {
      "samplingSettings": {
        "isEnabled": true,
        "maxTelemetryItemsPerSecond": 5
      }
    },
    "logLevel": {
      "Function": "Information"
    }
  },
  "extensions": {
    "http": {
      "routePrefix": "api"
    },
    "queues": {
      "maxDequeueCount": 5,
      "visibilityTimeout": "00:00:30"
    }
  }
}
```

---

## 6. `function.json` – Configuración por función

Generado automáticamente según los atributos en el código.  
Ejemplo para una función HTTP:
```json
{
  "bindings": [
    {
      "authLevel": "function",
      "type": "httpTrigger",
      "direction": "in",
      "name": "req",
      "methods": [ "get", "post" ]
    },
    {
      "type": "http",
      "direction": "out",
      "name": "$return"
    }
  ]
}
```

---

## 7. Cadenas de conexión

En Azure Portal → **Configuration → Connection strings**.

En .NET:
```csharp
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:SqlDb");
```

---

## 8. Uso de Azure App Configuration

Para centralizar configuraciones:
```bash
az appconfig create --name MyAppConfig --resource-group MyResourceGroup --location eastus
```

En código:
```csharp
var builder = new ConfigurationBuilder()
    .AddAzureAppConfiguration(Environment.GetEnvironmentVariable("AppConfigConnectionString"));
```

---

## 9. Uso de Azure Key Vault para secretos

Asignar permisos a la Function App:
```bash
az keyvault set-policy --name MyKeyVault --object-id <FunctionApp-Identity-ID> --secret-permissions get list
```

En código:
```csharp
var secretClient = new SecretClient(
    new Uri($"https://{Environment.GetEnvironmentVariable("KeyVaultName")}.vault.azure.net"),
    new DefaultAzureCredential()
);

KeyVaultSecret secret = await secretClient.GetSecretAsync("MySecret");
string secretValue = secret.Value;
```

---

## 10. Configuración por entornos (Development, Staging, Production)

- Usar **slots de despliegue** con configuraciones diferentes.
- Variables con el mismo nombre pero diferente valor en cada slot.
- En local, usar `DOTNET_ENVIRONMENT=Development` para activar settings específicos.

---

## 11. Buenas prácticas

- **No almacenar secretos** en `local.settings.json` o en el código.
- Usar **Azure Key Vault** para credenciales.
- Mantener configuraciones separadas por entorno.
- Documentar todas las variables necesarias.
- Usar `IConfiguration` para inyección de settings.

Ejemplo con inyección:
```csharp
public class MyFunction
{
    private readonly string _mySetting;

    public MyFunction(IConfiguration config)
    {
        _mySetting = config["MyCustomSetting"];
    }

    [FunctionName("TestFunc")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        return new OkObjectResult($"Setting value: {_mySetting}");
    }
}
```

---

## 12. Recursos recomendados

- [Azure Functions configuration and settings](https://learn.microsoft.com/azure/azure-functions/functions-app-settings)
- [Secure configuration in Azure](https://learn.microsoft.com/azure/security/fundamentals/management)
- [Azure Key Vault integration](https://learn.microsoft.com/azure/key-vault/general/overview)
