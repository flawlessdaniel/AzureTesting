# Seguridad y Autenticación en Azure Functions con .NET

Esta guía cubre los aspectos clave para proteger y autenticar Azure Functions cuando se desarrollan en **.NET**, con un enfoque en las mejores prácticas y la integración con servicios de Azure.

---

## 1. Tipos de Autenticación y Autorización en Azure Functions

Azure Functions soporta varios niveles de acceso configurables desde el **Function App Settings**:

- **Anonymous**  
  Permite invocar la función sin autenticación. **No recomendado** para funciones expuestas públicamente sin medidas adicionales.

- **Function**  
  Requiere la **Function Key** para invocar la función. La clave puede ser por función o a nivel de aplicación.

- **Admin**  
  Requiere la **Host Key**. Otorga permisos completos a todas las funciones.

---

## 2. Configuración de Niveles de Autorización

En el archivo `function.json`:
```json
{
  "bindings": [
    {
      "authLevel": "function",
      "type": "httpTrigger",
      "direction": "in",
      "name": "req"
    }
  ]
}
```
Valores posibles para `authLevel`: `anonymous`, `function`, `admin`.

---

## 3. Uso de Keys (Function y Host)

- **Function Keys**: Se utilizan para proteger funciones individuales.
- **Host Keys**: Tienen alcance global para todas las funciones de la app.

Se pueden administrar en **Azure Portal** → Function App → "Function Keys".

---

## 4. Integración con Azure AD (Authentication / Authorization)

### Pasos principales:
1. En el **Azure Portal**, habilitar **Authentication** en tu Function App.
2. Configurar un proveedor de identidad (Azure Active Directory).
3. Registrar la aplicación en **Azure AD** y obtener:
   - `Application (client) ID`
   - `Directory (tenant) ID`
   - **Client Secret** (si es necesario).
4. Configurar en el portal el **Redirect URL**:
   ```
   https://<tu-funcion>.azurewebsites.net/.auth/login/aad/callback
   ```

Ejemplo de validación de token JWT en .NET:
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{tenantId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidAudience = clientId,
            ValidateLifetime = true
        };
    });
```

---

## 5. Azure Managed Identities

Las **Managed Identities** permiten que la Function acceda a otros recursos de Azure sin almacenar credenciales.

### Tipos:
- **System-assigned**: Identidad ligada al ciclo de vida de la Function App.
- **User-assigned**: Identidad independiente, reutilizable por varias aplicaciones.

**Ejemplo de uso con Azure Key Vault:**
```csharp
var credential = new DefaultAzureCredential();
var client = new SecretClient(new Uri(keyVaultUrl), credential);
var secret = await client.GetSecretAsync("MiSecreto");
```

---

## 6. Almacenamiento seguro de secretos

- Usar **Azure Key Vault** para credenciales, cadenas de conexión y claves API.
- Evitar colocar secretos en `local.settings.json` en entornos productivos.
- Usar **Application Settings** del portal de Azure (que se almacenan cifrados).

---

## 7. HTTPS obligatorio

Habilitar solo conexiones seguras:
- En el portal: **TLS/SSL settings → HTTPS Only → ON**
- Forzar HTTPS a nivel de código si es necesario.

---

## 8. Roles y Claims en .NET

Una vez autenticado con Azure AD, puedes validar roles:
```csharp
[FunctionName("SecureFunction")]
public IActionResult Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
    ClaimsPrincipal user,
    ILogger log)
{
    if (user.IsInRole("Admin"))
    {
        return new OkObjectResult("Bienvenido, admin.");
    }
    return new UnauthorizedResult();
}
```

---

## 9. Monitorización y Alertas de Seguridad

- Usar **Application Insights** para registrar accesos y errores.
- Configurar alertas sobre patrones sospechosos.
- Revisar periódicamente logs de autenticación.

---

## 10. Buenas Prácticas

- Deshabilitar funciones no usadas.
- Rotar periódicamente las Function Keys.
- Minimizar el nivel de privilegio al conectar con otros servicios.
- Usar **Azure Policy** para exigir HTTPS y Managed Identities.
- Aplicar **Network Restrictions** para limitar el acceso a IPs específicas.

---
