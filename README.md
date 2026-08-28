# ProductosHex - API REST con Arquitectura Hexagonal

Proyecto de API REST desarrollado en C# y ASP.NET Web API aplicando los principios de la Arquitectura Hexagonal (Ports and Adapters), garantizando un alto desacoplamiento entre las reglas de negocio y los componentes de infraestructura tecnológica.

---

## 📐 Estructura de la Arquitectura

El sistema se organiza en 4 capas principales con inversión de dependencias:

       ┌──────────────────────────────────────────┐
       │      ProductosHex.Controllers            │ <-- Adaptador de Entrada (API REST)
       └────────────────────┬─────────────────────┘
                            │
                            v
       ┌──────────────────────────────────────────┐
       │      ProductosHex.Application            │ <-- Casos de Uso y DTOs
       └────────────────────┬─────────────────────┘
                            │
                            v
       ┌──────────────────────────────────────────┐
       │      ProductosHex.Domain                 │ <-- Core: Entidades y Puertos
       └────────────────────▲─────────────────────┘
                            │
       ┌────────────────────┴─────────────────────┐
       │      ProductosHex.Infrastructure         │ <-- Adaptador de Salida (EF + SQLite)
       └──────────────────────────────────────────┘

### Descripción de las Capas

* ProductosHex.Domain (Core / Núcleo):
  - Contiene las entidades principales de negocio (Producto, Categoria, Cliente, FacturaEncabezado, FacturaDetalle).
  - Define los Puertos (Interfaces) como ICategoriaRepository, IFacturaRepository, ILoggerService, manteniendo cero dependencias externas.

* ProductosHex.Application (Casos de Uso):
  - Contiene la lógica del negocio a través de servicios (FacturaService, CategoriaService, etc.).
  - Maneja los DTOs (Data Transfer Objects) para la entrada y salida de información.

* ProductosHex.Infrastructure (Adaptadores de Salida):
  - Implementa la persistencia de datos mediante Entity Framework y SQLite (ProductosDbContext, CategoriaRepositoryEF, FacturaRepositoryEF).
  - Incluye adaptadores de soporte como el sistema de registro de logs en archivos locales (FileLoggerService).

* ProductosHex.Controllers (Adaptador de Entrada):
  - Expone los endpoints HTTP (GET, POST, PUT, DELETE).
  - Gestiona las excepciones globales mediante LoggingExceptionFilterAttribute.
  - Configura la composición del contenedor de dependencias (DI) y la documentación con Swagger.

---

## 🛠️ Tecnologías Utilizadas

- Lenguaje: C# (.NET Framework)
- Entorno de Desarrollo: Visual Studio 2019
- Framework Web: ASP.NET Web API
- Documentación API: Swagger / Swashbuckle
- ORM: Entity Framework
- Base de Datos: SQLite (productos.db)
- Logging: Servicio de almacenamiento en texto plano diario (log_yyyyMMdd.txt)

---

## 🚀 Requisitos e Instalación

### Prerrequisitos
- Visual Studio 2019
- Carga de trabajo "Desarrollo de ASP.NET y web" instalada en VS 2019
- Motor o visor de SQLite (opcional para inspeccionar la BD)

### Pasos de Configuración

1. Clonar el repositorio:
   git clone https://github.com/dbetancourth-cloud/Hexagonal.git
   cd ProductosHex

2. Abrir la solución en Visual Studio 2019:
   Abre el archivo ProductosHex.sln en VS 2019.

3. Restaurar paquetes NuGet:
   En VS 2019, haz clic derecho sobre la Solución -> "Restaurar paquetes NuGet".

4. Aplicar Migraciones de Base de Datos:
   Abre la Consola del Administrador de Paquetes (Tools -> NuGet Package Manager -> Package Manager Console) y ejecuta:
   Update-Database

5. Ejecutar el proyecto:
   Selecciona ProductosHex.Controllers como proyecto de inicio y presiona F5 o Ctrl + F5.

---

## 🧪 Pruebas Interactivas con Swagger UI

Para probar y explorar los endpoints de la API de forma gráfica e interactiva:

1. Inicia la aplicación desde Visual Studio 2019 (F5 o Ctrl + F5).
2. Abre tu navegador e ingresa a la siguiente URL:
   http://localhost:<puerto>/swagger
3. Desde la interfaz de Swagger podrás visualizar todos los controladores, probar las peticiones (`GET`, `POST`, `DELETE`) y enviar los cuerpos de solicitud JSON directamente.

---

## 📜 Endpoints Disponibles

### Categorías
- GET /api/categoria -> Obtiene el listado completo de categorías.
- GET /api/categoria?pagina={p}&tamanoPagina={t} -> Consulta categorías con paginación y conteo total.
- GET /api/categoria/{id} -> Obtiene una categoría específica por ID.
- POST /api/categoria -> Registra una nueva categoría.
- DELETE /api/categoria/{id} -> Elimina una categoría por ID.

### Facturación y Productos
- POST /api/factura -> Procesa y registra una nueva factura con sus detalles.
- GET /api/factura/{id} -> Recupera la información detallada de una factura.

---

## 🛡️ Manejo de Excepciones y Logging

El sistema cuenta con un filtro global de excepciones (LoggingExceptionFilterAttribute) que intercepta cualquier error no controlado en la capa Web API, registrando el mensaje y la traza técnica (Stack Trace) en los archivos diarios de log dentro del servidor y retornando una respuesta estructurada al cliente sin exponer datos sensibles.