# Unit Conversion API

A production-ready, clean-architecture ASP.NET Core 8 Web API for performing physical unit conversions across Length, Temperature, and Weight.

---

## 🚀 Project Overview

The **Unit Conversion API** provides a robust, single endpoint to convert physical quantities between different units. The API is structured using **Clean Architecture** patterns to ensure that business logic remains decoupled, highly testable, and easy to maintain.

### Key Features
- **Clean Architecture & Separation of Concerns**: Logic is divided into Controllers (Presentation), Services (Application/Business Logic), Models (Data Transfer Objects), and Middleware (Cross-cutting concerns).
- **Dependency Injection**: Services are registered and managed using ASP.NET Core's built-in dependency injection container.
- **Robust Exception Handling**: Global middleware intercepts validation and runtime errors, returning standard, structured RFC 7807 `ProblemDetails` responses.
- **Auto-generated Swagger Documentation**: Full API specifications with interactive sandbox capability and XML documentation comments.
- **Case-Insensitive Unit Matching**: Client requests are sanitized and matched regardless of unit name capitalization (e.g., `Meter`, `METER`, `meter` are all valid).

---

## 🛠️ Folder Structure

```
UnitConversionApi/
├── Controllers/
│   └── ConversionController.cs      # presentation layer endpoint mappings
├── Services/
│   ├── IConversionService.cs        # business logic interface contract
│   └── ConversionService.cs         # conversion formulas and unit validation
├── Models/
│   └── ConversionResponse.cs        # API response payload format
├── Exceptions/
│   └── InvalidConversionException.cs # custom exception for business validation errors
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs# global error interceptor and translator
├── Properties/
│   └── launchSettings.json          # development server profiles
├── Program.cs                       # application bootstrap, DI registration, and middleware pipeline
├── UnitConversionApi.csproj         # project build metadata and dependencies
└── UnitConversionApi.http          # raw HTTP scratchpad for testing endpoints
```

---

## 📐 Supported Conversions

The API supports the following conversions:

| Category | Units Supported | Conversion Directives | Conversion Factor / Formula |
| :--- | :--- | :--- | :--- |
| **Length** | `meter` (m), `feet` (ft) | meter ↔ feet | `feet = meter * 3.28084` |
| **Length** | `kilometer` (km), `mile` (mi) | kilometer ↔ mile | `mile = kilometer * 0.621371` |
| **Temperature** | `celsius` (°C), `fahrenheit` (°F) | celsius ↔ fahrenheit | `°F = (°C * 9/5) + 32`<br>`°C = (°F - 32) * 5/9` |
| **Weight** | `kilogram` (kg), `pound` (lb) | kilogram ↔ pound | `pound = kilogram * 2.20462` |

*Note: Self-conversions (e.g., converting Celsius to Celsius) are also natively supported and return the input value immediately.*

---

## 🏃 How to Run the Project

### Prerequisites
- [.NET 8 SDK / .NET 10 SDK](https://dotnet.microsoft.com/download) installed on your system.

### Running Locally
1. Open a terminal/command prompt in the project's root folder (`c:\Users\sumit\Documents\UnitConversionApi`).
2. Run the application:
   ```bash
   dotnet run
   ```
3. The server will start and listen on standard HTTP/HTTPS ports, for example:
   - HTTPS: `https://localhost:7199`
   - HTTP: `http://localhost:5257`

### Interacting with Swagger UI
Once running, open your web browser and navigate to:
- [http://localhost:5257/swagger](http://localhost:5257/swagger)
- [https://localhost:7199/swagger](https://localhost:7199/swagger)

Here you will find full OpenAPI documentation containing details on models, query parameters, XML documentation comments, and an interactive "Try it out" feature.

---

## 🔌 API Endpoints & Usage

### GET `/api/conversion`

Converts a numeric value from one physical unit to another.

#### Query Parameters
- `value` (double, Required): The numeric value to be converted.
- `from` (string, Required): The starting unit.
- `to` (string, Required): The target unit.

#### Example Request
```http
GET http://localhost:5257/api/conversion?value=100&from=meter&to=feet
```

#### Example 200 OK Response
```json
{
  "originalValue": 100,
  "from": "meter",
  "to": "feet",
  "convertedValue": 328.084
}
```

#### Example 400 Bad Request Response (Validation / Type Mismatch)
If you attempt to convert between incompatible categories (e.g. meter to fahrenheit):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Invalid Conversion",
  "status": 400,
  "detail": "Cannot convert 'meter' (Length) to 'fahrenheit' (Temperature). Units must belong to the same category.",
  "instance": "/api/conversion"
}
```

---

## 🎨 Design Decisions

1. **Precision & Rounding**: Double-precision floats are susceptible to small rounding anomalies in base binary calculations (e.g. `0.30480000000000004`). To address this, the service rounds calculated values to **6 decimal places**. This preserves maximum precision for high-precision scientific or engineering applications while avoiding representation artifacts.
2. **Global Exception Middleware**: Rather than cluttering controller actions with nested try-catch blocks and manual `BadRequest()` responses, error handling is abstracted. The middleware intercepts exceptions, mapping custom business exceptions (`InvalidConversionException`) to `400 Bad Request` and general unhandled runtime failures to secure, sanitized `500 Internal Server Error` responses.
3. **Casing & Whitespace Resilience**: Input units are processed using `.Trim().ToLowerInvariant()`, meaning `Meter`, ` meter `, and `METER` are all accepted natively.
4. **Self-Conversion Resolution**: To prevent unnecessary calculations and support logical self-conversions, if `from == to` and the unit is supported, the API immediately short-circuits and returns the original value.

---

## 📈 Future Scalability Considerations

1. **Database-Driven Rates**: Currently, conversion rates are hardcoded. As the platform grows, we can introduce a Database/Cache repository layer to fetch dynamic conversion factors (or exchange rates for currencies).
2. **Composite Conversions (Dynamic Base Units)**: Instead of mapping exact pairwise conversions (e.g. `meter->feet`), we can define a single "Base Unit" for each category (e.g. `meter` for Length). Any input unit would convert to the base unit first, then convert from the base unit to the target unit. This changes the complexity of adding $N$ units from $O(N^2)$ conversion formulas to $O(N)$, dramatically scaling capabilities.
3. **API Versioning**: Implementing URL or Header-based API versioning (e.g., `/api/v2/conversion`) will prevent breaking existing consumer client integrations as new features or algorithms are released.
4. **Dynamic Custom Units**: Allowing users/organizations to define custom units dynamically via database records.
5. **Localization**: Translating unit names (e.g. "mètre" instead of "meter") and formatting response values based on regional settings.
