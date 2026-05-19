# HealthNet — Backend

The HealthNet backend is an **ASP.NET Core Web API** (.NET) built on a classic **layered architecture**: `Controller → Service → Repository → Database`. The layers are wired together through ASP.NET Core's built-in **dependency injection** container (configured in [HealthNet/Program.cs](HealthNet/Program.cs)), and persistence is handled with **Entity Framework Core** against a SQL Server database. Authentication uses **JWT bearer tokens** with role-based authorization, and the API surface is documented via **Swagger / OpenAPI**.

This README focuses on the four core code layers: **Controllers**, **DTOs**, **Repository**, and **Services**, plus a quick look at the supporting **Utility** layer.

---

## 1. High-Level Architecture

```
                 ┌────────────────────────────────────────────┐
   HTTP request  │              Controllers (API)             │   Receives HTTP, returns IActionResult
        ───────► │   [ApiController] [Authorize] [Route(...)] │   Validates input, reads JWT claims
                 └────────────────────────┬───────────────────┘
                                          │  DTOs (request / response models)
                                          ▼
                 ┌────────────────────────────────────────────┐
                 │                  Services                  │   Business rules, orchestration,
                 │            (IXxxService → XxxService)      │   role/ownership checks, mapping
                 └────────────────────────┬───────────────────┘
                                          │  DTOs / domain entities
                                          ▼
                 ┌────────────────────────────────────────────┐
                 │                Repositories                │   EF Core queries, persistence,
                 │       (IXxxRepository → XxxRepository)     │   no business logic
                 └────────────────────────┬───────────────────┘
                                          │
                                          ▼
                 ┌────────────────────────────────────────────┐
                 │   HealthNetContext (EF Core) → SQL Server  │
                 └────────────────────────────────────────────┘
```

Each module (Cases, Patients, Lab Reports, Outbreak Monitoring, etc.) is split vertically across these four layers, and each layer is registered with DI as `Scoped` in [Program.cs](HealthNet/Program.cs):

```csharp
builder.Services.AddScoped<ICasesService, CasesService>();
builder.Services.AddScoped<ICasesRepository, CasesRepository>();
```

This pattern (interface → implementation) is repeated for every module, which makes the layers swappable and unit-testable through their interfaces.

---

## 2. Controllers — `HealthNet/Controllers/`

Controllers form the **HTTP boundary** of the API. They expose RESTful endpoints under the route convention:

```
/api/v1/[controller]
```

### Responsibilities

A controller in HealthNet does **only** the following:

1. Accepts the incoming HTTP request and binds it to a DTO (`[FromBody]`, `[FromQuery]`, `[FromRoute]`).
2. Performs lightweight input checks (null body, ID > 0, etc.).
3. Reads identity information from the JWT (`User.FindFirst(ClaimTypes.NameIdentifier)`).
4. Delegates the actual work to the appropriate **Service**.
5. Translates the result (or exception) into an `IActionResult` with the correct HTTP status code.

Controllers **must not** touch the `DbContext`, run queries, or contain business rules — that logic belongs in the service layer.

### Attributes Used

| Attribute | Purpose |
|-----------|---------|
| `[ApiController]` | Enables automatic model validation and standard error responses. |
| `[Route("api/v1/[controller]")]` | Declares the route prefix. |
| `[Authorize(Roles = "...")]` | Restricts the endpoint to one or more roles defined in [Utility/Roles.cs](HealthNet/Utility/Roles.cs). |
| `[ProducesResponseType(...)]` | Documents possible status codes for Swagger. |
| `[HttpGet] [HttpPost] [HttpPut] [HttpDelete]` | Standard verb routing. |

### Example — `CasesController`

```csharp
[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = $"{Roles.Doctor}, {Roles.PublicHealthOfficer}, {Roles.Admin}")]
public class CasesController : ControllerBase
{
    private readonly ICasesService _casesService;

    public CasesController(ICasesService casesService) => _casesService = casesService;

    [HttpPost]
    public async Task<IActionResult> CreateCaseAsync([FromBody] CreateCaseDto request)
    {
        var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result   = await _casesService.CreateCaseAsync(request, doctorId);
        return Ok(result);
    }
}
```

### Controllers in This Project

Located in [HealthNet/Controllers/](HealthNet/Controllers/):

- `UserController.cs` — registration, login, JWT issuing, password reset flows.
- `PatientController.cs` — patient registration and management.
- `CitizenSymptomReportingController.cs` — citizen-submitted symptom reports.
- `CaseController.cs` — doctor-created clinical cases linked to symptom reports.
- `MedicalRecordController.cs` — patient medical records / prescriptions.
- `LaboratoryTestingController.cs` — lab test orders and execution.
- `LabReportController.cs` — diagnostic reports uploaded by lab technicians.
- `OutbreakMonitoringController.cs` — outbreak detection and alerts.
- `ComplianceRecordController.cs` — compliance tracking and policy adherence.
- `ReportingAndAnalyticsController.cs` — aggregated dashboards and KPIs.
- `AuditController.cs` — audit trail queries for compliance officers.

---

## 3. DTOs — `HealthNet/DTOs/`

**DTO** = *Data Transfer Object*. DTOs are plain C# classes that describe **the shape of data crossing the API boundary** — both inbound (from the client) and outbound (back to the client). They keep EF Core entities out of the public contract, which prevents over-posting and leaking internal columns.

### DTO Conventions

DTOs are grouped per module into subfolders under [HealthNet/DTOs/](HealthNet/DTOs/). Each module typically contains three kinds of DTOs:

| Pattern | Used for |
|---------|----------|
| `Create<Entity>Dto` | Payload to **create** a new record (POST). |
| `Update<Entity>Dto` | Payload to **update** an existing record (PUT / PATCH). |
| `<Entity>ResponseDto` / `<Entity>ListDto` | Shape of data **returned** to the client (single record vs. list). |

### Example — Cases module

[HealthNet/DTOs/CaseDto/](HealthNet/DTOs/CaseDto/) contains:

- `CreateCaseDto.cs` — fields a doctor sends when opening a new case (`ReportId`, `Diagnosis`, `Status`).
- `UpdateCaseDiagnosisDto.cs` — fields used when revising a diagnosis.
- `CaseResponseDto.cs` — shape returned right after creation.
- `CaseListDto.cs` — shape returned when listing or retrieving cases.

```csharp
public class CreateCaseDto
{
    public int ReportId { get; set; }       // SymptomReport ID (CitizenId derived from it)
    public string Diagnosis { get; set; } = null!;
    public bool? Status { get; set; }       // true = Recovered, false = Under Treatment
}
```

### DTO Modules in This Project

[HealthNet/DTOs/](HealthNet/DTOs/) — one folder per domain:
`AuditDTO`, `CaseDto`, `CitizenSymptomReportingDTO`, `ComplianceRecordDto`, `LabReportDTO`, `LabTestDTO`, `MedicalRecordDto`, `OutbreakMonitoringDTO`, `Pages` (pagination wrappers), `PatientDto`, `ReportingAndAnalyticsDTO`, `UserDTO`.

### Why DTOs (and not entities) on the API?

- **Security** — internal fields (`PasswordHash`, audit columns, FKs) never leak to clients.
- **Stability** — DB schema can evolve without breaking the public contract.
- **Validation** — DTOs can carry their own `[Required]`, `[MaxLength]`, `[Range]` attributes without polluting domain entities.
- **Clarity** — a controller signature like `CreateCaseAsync(CreateCaseDto dto)` documents exactly what the endpoint accepts.

---

## 4. Repository Layer — `HealthNet/Repository/`

The repository layer **owns all data access**. It is the *only* layer in the project that talks to `HealthNetContext` (the EF Core `DbContext`). Each module has:

- An **interface** (`IXxxRepository.cs`) that declares the data operations.
- An **implementation** (`XxxRepository.cs`) that uses EF Core (`_context.Cases.Where(...)`, `SaveChangesAsync()`, etc.) to fulfill them.

### Responsibilities

- CRUD operations against EF Core entities (`DbSet<>`).
- Query composition — `Include`, `Where`, `OrderBy`, projection, pagination.
- Persisting changes (`SaveChangesAsync`).
- Returning either entities or **DTOs already projected** from queries (the project does both, depending on the module).

### What Repositories Do *Not* Do

- No business rules (e.g. "a doctor can only close their own case").
- No authentication / authorization decisions.
- No HTTP concerns.
- No cross-module orchestration (the service layer handles that).

### Example — `ICasesRepository`

```csharp
public interface ICasesRepository
{
    Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto request, int doctorId, int citizenId);
    Task<IEnumerable<CaseListDto>> GetAllCasesAsync();
    Task<CaseListDto?> GetCaseByIdAsync(int caseId);
    Task UpdateCaseDiagnosisAsync(int caseId, string diagnosis);
    Task DeleteCaseAsync(int caseId);
}
```

Notice that the service passes both `doctorId` *and* `citizenId` — the citizen is resolved from the symptom report **before** the call reaches the repository. The repository simply persists what it's given.

### Repository Modules in This Project

[HealthNet/Repository/](HealthNet/Repository/) — one folder per domain, each with an `I*.cs` interface and its implementation:
`AuditRepository`, `CaseRepository`, `CitizenSymptomReportingRepository`, `ComplianceRecord`, `LabReportRepo`, `LabTestRepo`, `MedicalRepository`, `OutbreakMonitoringRepository`, `PatientRepository`, `ReportingAndAnalytics`, `User`.

---

## 5. Service Layer — `HealthNet/Services/`

The service layer is where **business logic lives**. Services sit between controllers (which know about HTTP) and repositories (which know about EF Core), so they remain agnostic of both — pure C# methods enforcing the domain rules of HealthNet.

### Responsibilities

- Enforce business and workflow rules (e.g. "a case can only be created from an existing, unprocessed symptom report").
- Perform ownership / authorization checks beyond simple role gating (e.g. "this doctor is the one assigned to this case").
- Orchestrate multiple repository calls inside one logical operation (e.g. create a case **and** mark the symptom report as triaged **and** write an audit entry).
- Translate between DTOs and entities where needed.
- Throw meaningful exceptions (`ArgumentException`, `KeyNotFoundException`, `UnauthorizedAccessException`) that controllers map to HTTP status codes.

### Example — `ICasesService`

```csharp
public interface ICasesService
{
    Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto request, int doctorId);
    Task<IEnumerable<CaseListDto>> GetAllCasesAsync();
    Task<CaseListDto> GetCaseByIdAsync(int caseId);
    Task UpdateCaseDiagnosisAsync(int caseId, UpdateCaseDiagnosisDto request, int doctorId);
    Task DeleteCaseAsync(int caseId, int doctorId);
}
```

The service signature exposes the *intent* (`CreateCaseAsync(dto, doctorId)`) — the controller does not need to know that the citizen ID has to be resolved from the symptom report, that an audit row must be written, or which tables are involved. That is the service's job.

### Service Modules in This Project

[HealthNet/Services/](HealthNet/Services/) — one folder per domain (interface + implementation):
`AuditService`, `CaseService`, `CitizenSymptomReportingServices`, `ComplianceRecordServices`, `LabReportServices`, `LabTestServices`, `MedicalServices`, `OutbreakMonitoringServices`, `PaginationService`, `PatientServices`, `ReportingAndAnalyticsServices`, `UserServices`.

Plus one specialized service:

- **`AutoTriage/`** — registered as a **hosted background service** (`AddHostedService<AutoTriageBackgroundService>()` in [Program.cs](HealthNet/Program.cs)). It uses `ISymptomRiskEvaluator` to automatically assess incoming symptom reports without blocking an HTTP request.

---

## 6. Utility Layer — `HealthNet/Utility/`

The Utility folder collects **cross-cutting helpers** that don't belong to a specific module. They are intentionally small and stateless. Notable files in [HealthNet/Utility/](HealthNet/Utility/):

| File | Purpose |
|------|---------|
| `Roles.cs` | Central string constants for role names (`Doctor`, `LabTechnician`, `Admin`, etc.) used in `[Authorize(Roles = ...)]`. |
| `PasswordHelper.cs`, `ForgotPasswordHelper.cs` | Password hashing / verification / reset token generation. |
| `EmailHelper.cs` | SMTP-based email dispatch (alerts, password reset, notifications). |
| `LocationHelper.cs` | Geolocation / address resolution via an HTTP client (registered with `AddHttpClient<LocationHelper>()`). |
| `OutbreakAlertUtility.cs` | Logic for raising outbreak alerts when thresholds are crossed. |
| `SymptomReportValidator.cs`, `SymptomReportHelper.cs` | Validation + helpers for citizen symptom reports. |
| `AuditHelper.cs`, `UpdateHelper.cs`, `StringHelper.cs` | Generic helpers for audit writing, partial updates, and string operations. |
| `CasesHelper.cs`, `LabReportHelper.cs`, `LabTestHelper.cs`, `ComplianceRecordHelper.cs`, etc. | Module-specific constants (error messages, mapping helpers) used by both controllers and services. |
| `CaseResponse.cs`, `EpidemiologyResponse.cs`, `ComplianceRecordResultHelper.cs` | Standardized response shapes / result wrappers. |

---

## 7. Cross-Cutting Concerns

Beyond the four layers, a few things are configured globally in [Program.cs](HealthNet/Program.cs):

- **Entity Framework Core** — `HealthNetContext` is registered with `UseSqlServer(...)` and reads the `DefaultConnection` connection string from `appsettings.json`.
- **JWT Authentication** — `AddAuthentication("Bearer").AddJwtBearer(...)` validates issuer, audience, lifetime, and signing key from the `JwtSettings` section.
- **Authorization** — role-based via `[Authorize(Roles = ...)]` on controllers; role constants live in [Utility/Roles.cs](HealthNet/Utility/Roles.cs).
- **CORS** — two policies are registered: `MyPolicy` (open) and `AllowAngular` (restricted to `http://localhost:4200` for the Angular dev server in [frontend/](../frontend/)).
- **Swagger / OpenAPI** — `AddSwaggerGen` is configured with a Bearer security scheme, so the Swagger UI lets you paste a JWT and call protected endpoints directly.

---

## 8. Adding a New Module — The Pattern

To add a new feature (say, **Vaccination**):

1. Create entities in the `HealthNetDb` project and add `DbSet<Vaccination>` to `HealthNetContext`.
2. Add DTOs under `HealthNet/DTOs/VaccinationDto/`:
   `CreateVaccinationDto.cs`, `VaccinationResponseDto.cs`, `VaccinationListDto.cs`.
3. Add the repository under `HealthNet/Repository/VaccinationRepository/`:
   `IVaccinationRepository.cs` + `VaccinationRepository.cs` (EF Core only).
4. Add the service under `HealthNet/Services/VaccinationServices/`:
   `IVaccinationService.cs` + `VaccinationService.cs` (business rules, calls repository).
5. Add the controller `Controllers/VaccinationController.cs` with `[Authorize(Roles = ...)]`.
6. Register the new interfaces in [Program.cs](HealthNet/Program.cs):
   ```csharp
   builder.Services.AddScoped<IVaccinationRepository, VaccinationRepository>();
   builder.Services.AddScoped<IVaccinationService,    VaccinationService>();
   ```
7. Add any helpers or role constants needed under `HealthNet/Utility/`.

Following this pattern keeps each layer single-purpose, the dependency direction one-way (`Controller → Service → Repository → DB`), and the codebase consistent across modules.
