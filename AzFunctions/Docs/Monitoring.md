# Guía de Monitoreo para Azure Functions en .NET

Esta guía describe las herramientas, configuraciones y mejores prácticas para implementar un monitoreo efectivo de **Azure Functions** desarrolladas en **.NET**.

---

## 1. Herramientas principales para monitoreo

- **Application Insights**  
  Servicio de telemetría que registra métricas, logs y diagnósticos.
- **Azure Monitor**  
  Plataforma centralizada para métricas y alertas en todos los recursos de Azure.
- **Log Analytics Workspace**  
  Almacena y permite consultas avanzadas sobre datos de telemetría.
- **Metrics Explorer**  
  Interfaz para visualizar métricas históricas y en tiempo real.

---

## 2. Habilitar Application Insights

### Desde Azure Portal:
1. Ir a la **Function App**.
2. En el panel izquierdo: **Application Insights → Activar**.
3. Crear o vincular un recurso existente de Application Insights.
4. Confirmar que la opción **Log Level** esté configurada.

### Desde .NET:
```csharp
using Microsoft.Extensions.Logging;

public class MyFunction
{
    private readonly ILogger<MyFunction> _logger;

    public MyFunction(ILogger<MyFunction> logger)
    {
        _logger = logger;
    }

    [FunctionName("ExampleFunction")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("Function ejecutada a: {time}", DateTime.UtcNow);
    }
}
```

---

## 3. Tipos de datos que se pueden monitorear

- **Métricas**:
  - Tiempo de ejecución.
  - Cantidad de ejecuciones.
  - Ejecuciones con éxito vs fallidas.
  - Consumo de memoria y CPU.
- **Logs**:
  - Mensajes de información (`LogInformation`).
  - Advertencias (`LogWarning`).
  - Errores (`LogError`).
- **Dependencias**:
  - Llamadas a otros servicios como Azure Storage, Cosmos DB, APIs externas.
- **Custom Events**:
  - Eventos definidos por el desarrollador para análisis personalizado.

---

## 4. Consultas con Kusto Query Language (KQL)

Ejemplo de consulta para errores:
```kql
traces
| where severityLevel == 3
| order by timestamp desc
```

Ejemplo para contar ejecuciones por función:
```kql
requests
| summarize count() by operation_Name
```

---

## 5. Configuración de niveles de logging

En `host.json`:
```json
{
  "logging": {
    "applicationInsights": {
      "samplingSettings": {
        "isEnabled": true,
        "maxTelemetryItemsPerSecond" : 5
      }
    },
    "logLevel": {
      "Function": "Information",
      "Host": "Error",
      "default": "Warning"
    }
  }
}
```

---

## 6. Alertas

1. Ir a **Azure Monitor → Alerts**.
2. Crear una regla de alerta:
   - **Scope**: Seleccionar la Function App.
   - **Condition**: Definir métrica o consulta KQL.
   - **Action Group**: Correo, SMS, Teams, Webhook, etc.
3. Guardar y activar la alerta.

Ejemplos de alertas útiles:
- Latencia alta (>5 segundos).
- Más de X errores en un periodo de tiempo.
- Sin ejecuciones en el último intervalo esperado.

---

## 7. Monitoreo de dependencias

Application Insights puede registrar automáticamente:
- Azure Storage (Queue, Blob, Table).
- Azure SQL Database.
- Cosmos DB.
- HTTP requests a APIs externas.

En .NET:
```csharp
var telemetryClient = new TelemetryClient();
telemetryClient.TrackDependency("HTTP", "ExternalAPI", "GET /data", DateTimeOffset.Now, TimeSpan.FromMilliseconds(120), true);
```

---

## 8. Dashboard personalizado

- Usar **Azure Dashboard** para centralizar métricas clave.
- Combinar gráficos de Application Insights y Azure Monitor.
- Guardar y compartir el dashboard con el equipo.

---

## 9. Buenas prácticas de monitoreo

- **Activar sampling** para reducir costos y volumen de datos.
- Definir **nombres claros de funciones y operaciones**.
- Usar **Custom Properties** en logs para facilitar filtrado.
- Revisar periódicamente métricas de consumo y performance.
- Probar las alertas en un entorno de staging antes de producción.

---

## 10. Ejemplo de flujo de monitoreo recomendado

1. **Captura de logs** → `ILogger` en el código.
2. **Envío a Application Insights** → Telemetría automática.
3. **Análisis en Log Analytics** → Consultas KQL.
4. **Alertas en Azure Monitor** → Notificaciones al equipo.
5. **Dashboard en Azure** → Visualización en tiempo real.
6. **Optimización** → Ajustar código y configuración según métricas.

---
