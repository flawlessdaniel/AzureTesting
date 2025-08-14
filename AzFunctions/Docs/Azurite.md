1️⃣ ¿Qué es Azurite?

Azurite es un emulador local de Azure Storage. Permite trabajar con Blob Storage, Queue Storage y Table Storage sin necesidad de una cuenta real en Azure. Es ideal para desarrollo y pruebas.

2️⃣ Instalación
Opción A – Con Node.js (recomendada)

Instala Node.js LTS (mínimo v14) desde nodejs.org.

Instala Azurite globalmente:

npm install -g azurite

Opción B – Con Docker

Tener Docker instalado.

Ejecutar:

docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite

3️⃣ Puertos y Servicios

Por defecto Azurite usa:

Blob Storage: http://127.0.0.1:10000/devstoreaccount1

Queue Storage: http://127.0.0.1:10001/devstoreaccount1

Table Storage: http://127.0.0.1:10002/devstoreaccount1

4️⃣ Credenciales predeterminadas

Azurite siempre arranca con:

AccountName: devstoreaccount1

AccountKey:

Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==


ConnectionString:

DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;

5️⃣ Ejecución local

Si instalaste con npm:

azurite --location ./data --silent --debug ./debug.log


--location → carpeta donde se guardarán los datos.

--silent → suprime logs de consola.

--debug → guarda logs detallados.

6️⃣ Uso con Azure Functions

En el archivo local.settings.json de tu Function App:

{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  }
}


La clave "UseDevelopmentStorage=true" hace que tu Function use Azurite en lugar de Azure real.

7️⃣ Herramientas gráficas para explorar datos

Puedes administrar Azurite con:

Azure Storage Explorer (descargar)

Visual Studio Code con la extensión Azure Storage.

8️⃣ Ejemplo rápido con Blob Storage en .NET
using Azure.Storage.Blobs;

string connectionString = "UseDevelopmentStorage=true";
string containerName = "testcontainer";

// Crear cliente y contenedor
var blobServiceClient = new BlobServiceClient(connectionString);
var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
await containerClient.CreateIfNotExistsAsync();

// Subir archivo
var blobClient = containerClient.GetBlobClient("archivo.txt");
await blobClient.UploadAsync("archivo.txt", overwrite: true);