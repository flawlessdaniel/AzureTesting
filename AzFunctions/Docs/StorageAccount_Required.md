## 📊 ¿Storage Account requerido según plan y tipo de trigger?

| **Plan de hosting**             | **Tipo de funciones**               | **¿Storage Account requerido?** | **Notas** |
|---------------------------------|-------------------------------------|---------------------------------|-----------|
| **Consumption**                 | Cualquier trigger                   | ✅ **Sí, siempre**              | El runtime depende de Storage para escalar y coordinar instancias. |
| **Premium**                     | Solo **HTTP triggers**              | ❌ **No**                       | Puedes marcar *Run without storage account*. Sin colas, timers ni blobs. |
| **Premium**                     | HTTP + otros triggers (Timer, Queue, Blob, Event Hub, etc.) | ✅ **Sí**                       | Necesario para checkpoints, colas y escalado. |
| **Dedicated (App Service Plan)**| Solo **HTTP triggers**              | ❌ **No**                       | Igual que en Premium, puede correr sin Storage. |
| **Dedicated (App Service Plan)**| HTTP + otros triggers               | ✅ **Sí**                       | Requerido para todos los bindings que usan estado. |
| **Local development**           | Cualquier trigger                   | ✅ **Sí (pero con Azurite)**    | Se usa `"UseDevelopmentStorage=true"` o cadena a Azurite. |
