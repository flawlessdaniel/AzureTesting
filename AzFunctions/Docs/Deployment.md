# Guía de Despliegue de Azure Functions en .NET

Esta guía detalla los métodos, configuraciones y buenas prácticas para desplegar **Azure Functions** desarrolladas en **.NET**.

---

## 1. Requisitos previos

- **.NET SDK** instalado (versión compatible con Azure Functions, por ejemplo, .NET 6 o .NET 8).
- **Azure Functions Core Tools** para ejecución y publicación local.
- **Cuenta de Azure** con permisos de contribuidor.
- **Azure CLI** o **Azure PowerShell** instalado.
- **Visual Studio 2022** o **VS Code** (opcional pero recomendado).

---

## 2. Métodos de despliegue soportados

### 2.1 Desde Visual Studio
1. Abrir el proyecto de Azure Functions.
2. Clic derecho en el proyecto → **Publish**.
3. Seleccionar **Azure** → **Azure Function App (Windows/Linux)**.
4. Crear o seleccionar una Function App existente.
5. Publicar directamente.

Ventajas:
- Integración sencilla.
- Despliegue rápido.

Desventajas:
- Menos control sobre el pipeline.

---

### 2.2 Con Azure CLI
```bash
# Autenticarse
az login

# Crear grupo de recursos
az group create --name myResourceGroup --location eastus

# Crear plan de consumo (opcional si no existe)
az functionapp plan create --name myPlan --resource-group myResourceGroup --location eastus --number-of-workers 1 --sku Y1 --is-linux

# Crear storage account
az storage account create --name mystorageacct --location eastus --resource-group myResourceGroup --sku Standard_LRS

# Crear la Function App
az functionapp create --resource-group myResourceGroup --consumption-plan-location eastus --runtime dotnet --functions-version 4 --name myFunctionApp --storage-account mystorageacct

# Publicar
func azure functionapp publish myFunctionApp
```

---

### 2.3 Con Azure PowerShell
```powershell
# Autenticarse
Connect-AzAccount

# Crear grupo de recursos
New-AzResourceGroup -Name myResourceGroup -Location eastus

# Crear cuenta de almacenamiento
New-AzStorageAccount -ResourceGroupName myResourceGroup -Name mystorageacct -Location eastus -SkuName Standard_LRS

# Crear Function App
New-AzFunctionApp -ResourceGroupName myResourceGroup -Name myFunctionApp -StorageAccountName mystorageacct -Location eastus -Runtime dotnet -FunctionsVersion 4 -ConsumptionPlanLocation eastus

# Publicar desde carpeta
func azure functionapp publish myFunctionApp
```

---

### 2.4 Despliegue continuo (CI/CD) con GitHub Actions
Archivo `.github/workflows/azure-function.yml`:
```yaml
name: Deploy Azure Function

on:
  push:
    branches:
      - main

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v2
        with:
          dotnet-version: '8.0.x'
      - name: Publish to Azure
        uses: Azure/functions-action@v1
        with:
          app-name: myFunctionApp
          package: '.'
          publish-profile: ${{ secrets.AZURE_FUNCTIONAPP_PUBLISH_PROFILE }}
```

---

## 3. Configuración de variables de entorno

En Azure Portal:
1. Ir a la Function App.
2. **Configuration → Application settings**.
3. Agregar las variables necesarias.
4. Guardar y reiniciar.

En `local.settings.json` (solo local):
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}
```

---

## 4. Slots de despliegue

- **Slot de producción**: Tráfico real.
- **Slots de staging**: Pruebas previas antes de producción.
- Permiten **swap** para cambios sin downtime.

Creación por CLI:
```bash
az functionapp deployment slot create --name myFunctionApp --resource-group myResourceGroup --slot staging
```

---

## 5. Estrategias de despliegue recomendadas

- **Blue-Green Deployment**: Dos entornos, uno activo y otro preparado para el cambio.
- **Canary Release**: Publicar para un pequeño porcentaje de usuarios antes del despliegue total.
- **Rollback rápido**: Mantener versión anterior lista para restaurar.

---

## 6. Buenas prácticas

- Probar localmente con **Azurite** y `func start` antes de publicar.
- Usar **slots** para reducir riesgos.
- Versionar el código y usar CI/CD.
- Mantener el **`host.json`** y **`local.settings.json`** limpios y organizados.
- Documentar variables de entorno y secretos.

---

## 7. Ejemplo de flujo de despliegue rápido con .NET y CLI

```bash
# Compilar proyecto
dotnet build

# Ejecutar pruebas
dotnet test

# Publicar a Azure
func azure functionapp publish myFunctionApp
```

---

## 8. Referencias útiles

- [Azure Functions Deployment Methods](https://learn.microsoft.com/azure/azure-functions/functions-deployment-technologies)
- [Azure CLI Documentation](https://learn.microsoft.com/cli/azure/)
- [Azure PowerShell Documentation](https://learn.microsoft.com/powershell/azure/)
- [GitHub Actions for Azure Functions](https://learn.microsoft.com/azure/azure-functions/functions-how-to-github-actions)
