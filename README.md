# HealthNet — Frontend

Angular 20 client for the HealthNet public-health & disease-surveillance backend (.NET Web API).
This README is written as an interview cheat-sheet: each section answers the kind of
question an interviewer is likely to ask about the codebase.

---

## TL;DR (30-second pitch)

> HealthNet is a multi-role Angular SPA built on standalone components.
> Citizens submit symptom reports, doctors triage them into cases & medical records,
> lab techs run tests and upload reports, public-health officers monitor outbreaks
> and run epidemiology metrics, compliance officers audit it all, and admins manage users.
> JWT auth in `sessionStorage`, role-based route guards, an HTTP interceptor that injects
> the token, a shared lookup service that turns every ID input into a searchable dropdown,
> and a reusable `<app-id-picker>` form control via `ControlValueAccessor`.

---

## Tech stack

| Concern              | Choice                                               |
|----------------------|------------------------------------------------------|
| Framework            | Angular 20 (standalone components, no NgModules)    |
| Forms                | Reactive forms + template-driven (mixed by component) |
| HTTP                 | `HttpClient` + functional interceptors               |
| State                | Local component state + RxJS observables (no NgRx)  |
| Auth                 | JWT in `sessionStorage`, manual base64 decode        |
| Routing              | Standalone `Routes`, `authGuard` + `roleGuard`       |
| UI library           | Custom design system + Tabler Icons + a little Bootstrap |
| Styling              | Global `styles.css` for shared tokens; component CSS only for overrides |
| Backend              | .NET Web API at `http://localhost:5171/api/v1`       |

---

## Folder structure

```
src/
├── environments/
│   └── environment.ts                ← apiUrl
├── styles.css                        ← shared design system (tokens, .page, .card, .btn-*…)
├── app/
│   ├── app.routes.ts                 ← all routes + role guards
│   ├── app.config.ts                 ← bootstrap config + interceptors
│   ├── app.ts / app.html             ← root <router-outlet> shell
│   ├── core/
│   │   ├── guards/
│   │   │   ├── auth-guard.ts         ← redirects to /login if no token
│   │   │   └── role-guard.ts         ← checks data.roles against decoded JWT role
│   │   ├── interceptors/
│   │   │   └── auth-interceptor.ts   ← attaches Bearer token + logs API errors
│   │   ├── models/                   ← typed DTOs that mirror backend
│   │   └── services/                 ← one service per backend resource + helpers
│   ├── shared/components/
│   │   ├── main-layout/              ← sidebar + header + <router-outlet>
│   │   ├── sidebar-component/        ← role-filtered menu
│   │   ├── header-component/         ← top bar with user menu & logout
│   │   ├── id-picker/                ← reusable searchable dropdown (CVA)
│   │   ├── alert/                    ← shared alert banner
│   │   └── loading-spinner/          ← reusable spinner
│   └── components/                   ← page-level feature components
```

---

## Authentication flow

1. User submits `/login` form → `AuthService.login()` → `POST /api/v1/User`
2. Response `{ token }` is stored by `TokenService.setToken()` in `sessionStorage` under
   key `healthnet_token`.
3. `TokenService.decodeToken()` does a manual base64 decode of the JWT payload and
   handles many claim-key shapes (Microsoft schema, plain `role`, `Role`, `RoleName`).
4. `AuthStateService` exposes an `isLoggedIn$` BehaviorSubject so the header/sidebar
   react to login/logout without re-reading session storage.
5. Login redirects per role:
   - `Citizen` → `/citizen-home`
   - `Admin` → `/home/admin`
   - `Compliance Officer` → `/compliance`
   - Everyone else (Doctor / Lab Tech / PHO / Researcher) → `/home`
6. Every authenticated request is intercepted by `auth-interceptor.ts` which:
   - reads the token from `sessionStorage`,
   - attaches it as `Authorization: Bearer …`,
   - catches errors and logs them so the rest of the app sees a clean stream.

**Forgot password**: public `/forgot-password` route → `PUT /api/v1/User/forgotpassword`.

---

## Routing & guards

`app.routes.ts` registers every route. Guards combine:

```ts
{ path: 'patients',
  component: PatientsComponent,
  canActivate: [authGuard, roleGuard],
  data: { roles: ['Admin', 'Doctor', 'Public Health Officer', 'Lab Technician'] } }
```

- `authGuard`: returns true if token exists and isn't expired; otherwise `router.navigate(['/login'])`.
- `roleGuard`: reads `route.data['roles']` and checks the decoded JWT role; if no match,
  redirects to `/unauthorized`.

Public routes: `/login`, `/register`, `/forgot-password`, `/about`, `/compliance`, `/unauthorized`.

---

## Shared services (`core/services/`)

| Service                  | Responsibility                                                                              |
|--------------------------|---------------------------------------------------------------------------------------------|
| `AuthService`            | login(), logout(), `isLoggedIn$`, `hasRole()`, `hasAnyRole()`. Single source of truth.       |
| `TokenService`           | get/set/remove JWT in `sessionStorage`, decode payload, expose `getUserId/Role/Email`.       |
| `AuthStateService`       | RxJS BehaviorSubject mirroring auth state for templates.                                    |
| `UserService`            | `/User` — register, getById, update, soft-delete, getAll, forgotPassword.                   |
| `PatientService`         | `/Patient` — search, getById, register, update, deactivate.                                 |
| `CaseService`            | `/Cases` — CRUD; returns plain text on PUT/DELETE so we use `responseType: 'text'`.         |
| `MedicalRecordService`   | `/MedicalRecord` — add, getRecords (by patientId), update, deactivate (PATCH /close).        |
| `LabTestService`         | `/LaboratoryTesting` + `/LabReport` — list, create, update, download, **multipart upload**. |
| `OutbreakService`        | `/OutbreakMonitoring` + `/ReportingAndAnalytics/outbreaks` — outbreak + Epidemiology CRUD.  |
| `SymptomReportService`   | `/CitizenSymptomReporting` — submit, my-reports, all (staff), update status, delete.        |
| `ComplianceService`      | `/ComplianceRecord` — create, get, update, delete; result strings normalized to lowercase.  |
| `AuditService`           | `/Audit` — full CRUD + close.                                                                |
| `AnalyticsService`       | `/ReportingAndAnalytics/*` — cases, patients, outbreaks, epidemiology, compliance metrics.   |
| `LookupService`          | **Single cache** for dropdown options (patients, technicians, cases, lab tests, outbreaks, symptom reports). Uses `shareReplay(1)` so multiple components share one HTTP call per session. |

### Interview talking-point: `LookupService`

> "When users complained that they had to memorise database IDs, I built a single
> `LookupService` that caches lookup lists with `shareReplay(1)`. It exposes
> `Observable<LookupOption[]>` per kind (patients, technicians, outbreaks…) so any
> form can drop in a dropdown by passing the observable to `<app-id-picker>`.
> Caching means switching between pages doesn't re-fetch."

---

## Reusable form control — `<app-id-picker>`

Implements `ControlValueAccessor`, so it plugs into both `formControlName` and
`[(ngModel)]`. Behaviors:

- Click → searchable list of options (filter by label, sub-text, or numeric ID)
- Click an option → emits the numeric `value` to the parent form control
- `[allowManualEntry]="true"` → if no list is loaded (e.g. `/User/GetAll` returns 403
  for non-admins), the user can type a positive integer and click **Use ID #N**.
- Listens for `document:click` to close on outside click.

Used in: cases (symptom-report picker), compliance (entity picker driven by type),
lab test create/edit (patient + technician), lab report upload (test picker),
medical records (patient picker), epidemiology (outbreak picker).

---

## Component-by-component tour

### Auth / public

| Component | Route | What to say in an interview |
|---|---|---|
| `LoginComponent` | `/login` | Two-pane shell (gradient brand panel + form card). Locks to `100vh` with `overflow: hidden` so it never page-scrolls. JWT-based login that redirects per role. |
| `RegisterComponent` | `/register` | Multi-field form with strong-password regex (8+ chars, upper, lower, digit, special) and 10-digit phone validation. |
| `ForgotPasswordComponent` | `/forgot-password` | Reset-password page calling `PUT /User/forgotpassword`. Cross-field validator ensures new + confirm match. |
| `Unauthorized` | `/unauthorized` | Static 403 fallback used by `roleGuard`. |
| `AboutComponent` | `/about` | Public marketing-style page: gradient hero with floating stat cards, stat strip, pillar cards, alternating image+copy module rows (`image1`…`image5`), closing CTA. |
| `ComplianceComponent` | `/compliance` | Public posture page: hero, 3-step flow (Audit → Record → Review), 4 result-type chips, role permissions matrix, closing CTA. |

### Layout (shared)

| Component | Why it matters |
|---|---|
| `MainLayoutComponent` | Renders the sidebar + header + `<router-outlet>` once logged in. |
| `SidebarComponent` | Renders a role-filtered menu. Has a hard-coded `allNavItems` list each item declaring `roles: string[]`. Filtered by `this.role = TokenService.getUserRole()` on init. Role-coloured badge per user role. |
| `HeaderComponent` | Top bar with the HealthNet wordmark, user initial badge, dropdown with profile / logout. |

### Citizen workflow

| Component | What it does |
|---|---|
| `CitizenhomeComponent` | Landing page for Citizen role. |
| `SymptomReportComponent` | Form with 12 common symptoms + 7 vitals (temp & oxygen required). Payload is **compacted before submit** — only `true` symptoms and non-null vitals — because the backend `SymptomsJson` column is small and truncates verbose payloads. |
| `SymptomHistoryComponent` | Paginated list of a citizen's own past reports via `GET /CitizenSymptomReporting/mine`. |

### Clinical workflow

| Component | What it does |
|---|---|
| `PatientsComponent` | Patient registry. Search by name + status filter + pagination, register new patient (10-digit phone validation), view detail modal (`GET /{id}`), update, deactivate (`PATCH /{id}/deactivate`). |
| `CasesComponent` | Cases CRUD. Symptom-report picker **filters out reports whose citizen already has a case** (backend rejects duplicates). Diagnosis validator blocks the literal string `"string"` and all-digit inputs (mirrors backend rules). |
| `MedicalRecordsComponent` | Add / view / update / deactivate medical records. Patient picker; because backend's `GetRecords` doesn't return `recordId` we expose a "Manage by Record ID" panel for update/deactivate. |
| `AllSymptomReportsComponent` | Doctor / PHO / Admin view of all citizen reports. Status PATCH sends the **numeric enum value** (`Submitted=1, UnderReview=2, Reviewed=3, Closed=4`) under both `Status` (Pascal) and `status` (camel) for cross-config compatibility, and uses `responseType: 'text'` because the API returns plain text. |

### Lab workflow

| Component | What it does |
|---|---|
| `LabTestComponent` | Lists lab tests with filters (type, status, date). |
| `LabTestCreateComponent` | Create test with patient + technician dropdowns. `allowManualEntry` enabled on the technician picker because `/User/GetAll` is Admin-only — a Doctor can't fetch tech IDs and may need to type one. |
| `LabTestDetailComponent` / `LabTestEditComponent` | View test detail + reports; update type / re-assign technician. |
| `LabReportUploadComponent` | **Multipart upload** to `POST /LabReport` (FormData with `TestId` + `File`). Lets HttpClient set the multipart boundary — do **not** manually set `Content-Type`. Shows the tech's own pending tests as picker options. |
| `LabTechAssignmentsComponent` | "My Assignments" view filtered to `technicianId === currentUserId`. |

### Outbreak & Epidemiology

| Component | What it does |
|---|---|
| `OutbreakDashboardComponent` / `OutbreakListComponent` / `OutbreakDetailComponent` / `OutbreakUpdateComponent` / `OutbreakFormComponent` | Outbreak CRUD. Location filter runs **client-side** on `o.location.toLowerCase().includes(...)` because `GET /outbreakmonitoring/active` has no server-side filter and `region` on the analytics endpoint doesn't match free-text reliably. |
| `EpidemiologyComponent` | Full CRUD for epidemiology records mounted under an outbreak. Structured metrics form (infected / recovered / deaths / hospitalised / vaccinated / notes) serializes to the `MetricsJSON` string the backend expects. Outbreak detail has a "Create Epidemiology" button that deep-links here with `?outbreakId=…`. |

### Compliance & Audit

| Component | What it does |
|---|---|
| `ComplianceRecordComponent` | Compliance CRUD. Result is a 4-pill radio with backend's lowercase wire values (`compliant`, `non compliant`, `partially compliant`, `pending review`). Entity picker is **type-aware** — picking "Lab Test" loads tests, "Outbreak" loads outbreaks, etc. via `LookupService`. |
| `AuditComponent` | Audit CRUD: list with multi-field filter form, lookup by ID, create form, inline update, close, soft-delete. |

### Analytics

| Component | What it does |
|---|---|
| `AnalyticsComponent` | Dashboard with tabs for cases / patients / outbreaks / epidemiology / compliance. KPI cards + custom SVG bar charts and a donut chart for compliance distribution. Open to every role (Citizen sees only outbreaks + epidemiology). |

### Admin

| Component | What it does |
|---|---|
| `AdminDashboardComponent` | Admin-only landing. |
| `UsersComponent` | Admin user management: filterable Active Users table, view modal, edit inline, soft-delete. |
| `UpdateUserComponent` / `DeleteUserComponent` | Legacy single-user update / delete forms; kept for routing back-compat. |
| `ProfileComponent` | Logged-in user's profile. Role-coloured gradient banner with initials avatar, key-value About card, Quick Actions card (Update / Deactivate for Admin, Reset Password, Analytics), Security & Access card. |

### Misc

| Component | What it does |
|---|---|
| `DashboardHomeComponent` | Staff dashboard. Greeting + role-specific action cards. Admin auto-redirects to `/home/admin`. |
| `HomeComponent` | Public landing page. |
| `AboutComponent` / `ComplianceComponent` | Public marketing pages (see above). |

---

## Models (`core/models/`)

Mirror backend DTOs. Notable ones:

- `User.ts`, `UpdateUser.ts`, `register.ts` — auth + admin payloads.
- `patient.model.ts` — `RegisterPatientRequest` etc. ContactInfo is phone-only.
- `case.model.ts` — `CreateCaseRequest { reportId, diagnosis, status }`.
- `medical-record.model.ts` — note `MedicalRecordGetDto` has no `recordId` field
  (matches backend quirk).
- `lab-test.model.ts` — test + report shapes + upload result.
- `Outbreak.ts` + `epidemiology.model.ts` — outbreak + epi metrics.
- `compliance.model.ts` — request, list-dto, filter, update.
- `audit.model.ts` — `AuditFilterDto`, `UpdateAuditRequest`, list dto.
- `analytics.model.ts` — typed response shapes for each analytics endpoint.
- `symptom-report.ts` — `SymptomsJsonPayload` for citizen reports.

---

## Styling architecture

- **Single source of truth**: `src/styles.css` defines CSS custom properties
  (`--hn-primary`, `--hn-bg-page`, `--hn-radius`…) and global classes:
  `.page`, `.card`, `.form-control`, `.btn-primary`, `.badge-*`, `.modal-*`,
  `.kpi-*`, `.search-row`, `.filter-row`, alerts, table styles, animations,
  responsive defaults.
- **Component CSS** only contains palette overrides (e.g. lab-tech pages use an
  amber palette) and layout deltas specific to that page (column hides at small
  breakpoints, custom widgets like the donut chart).
- **Why**: removed ~120KB of duplicate CSS across components and locks the design
  language so a token change in one place propagates everywhere.

---

## Cross-cutting interview talking points

1. **Standalone components everywhere.** No NgModules. Each component declares its
   own `imports` array. Easier to tree-shake and reason about.

2. **Functional HTTP interceptors.** `auth-interceptor.ts` is a pure function,
   registered via `provideHttpClient(withInterceptors([...]))`. Clean, testable.

3. **Plain-text responses.** Several backend endpoints return `"OK message"` as
   text, not JSON. Angular's `HttpClient` parses as JSON by default and throws.
   The fix is `responseType: 'text'`. We hit this on:
   - PATCH `/CitizenSymptomReporting/{id}` (status update)
   - PUT/DELETE `/Cases/{id}`
   - PUT/PATCH/DELETE `/Audit/{id}`
   - PUT/PATCH `/MedicalRecord/{id}*`

4. **Error message extraction.** Backend throws `ArgumentException` with specific
   strings (`"Duplicate case for this citizen."`, `"Diagnosis must not be a placeholder."`, etc.).
   We surface them via an `extractError(err, fallback)` helper that handles plain-text,
   `{message}`, ProblemDetails `{title, detail, errors}`.

5. **Role-aware UI.** Beyond route guards, every page asks `AuthService.getUserRole()`
   to decide which buttons to render. The sidebar filters menu items the same way.

6. **No NgRx.** State stays in components + a handful of `BehaviorSubject`s in
   `AuthStateService`. The trade-off is simpler code + faster onboarding;
   risk is no central undo history. For this app size that's the right call.

7. **Compact JSON for tiny columns.** `SymptomsJson` backend column was overflowing,
   so the citizen form filters to only `true` symptoms and non-null vitals before
   submit. A real backend fix would widen the column, but we couldn't touch backend.

8. **Picker as ControlValueAccessor.** One reusable component drives every ID input
   across the app. The trick: implement `writeValue / registerOnChange / registerOnTouched`
   and provide `NG_VALUE_ACCESSOR`. Then it works with both reactive and
   template-driven forms.

9. **Caching with `shareReplay(1)`.** `LookupService` returns a `shareReplay(1)`
   observable per kind. First subscriber triggers the HTTP call, every subsequent
   subscriber gets the cached value synchronously. Refreshable via `refresh(kind)`.

10. **CSS custom properties + a global stylesheet.** Cuts duplication and gives us
    a single place to change the design language.

---

## Running the app

```bash
npm install
npm start                 # serves on http://localhost:4200
```

Backend must be running on `http://localhost:5171` (see `src/environments/environment.ts`).

Build:
```bash
npx ng build              # production build
npx ng build --configuration=development
```

---

## File-finding cheat sheet

| Looking for…              | Path                                                            |
|---------------------------|-----------------------------------------------------------------|
| API base URL              | `src/environments/environment.ts`                              |
| Routes                    | `src/app/app.routes.ts`                                         |
| Auth guard                | `src/app/core/guards/auth-guard.ts`                             |
| Role guard                | `src/app/core/guards/role-guard.ts`                             |
| HTTP interceptor          | `src/app/core/interceptors/auth-interceptor.ts`                 |
| Lookup cache              | `src/app/core/services/lookup.service.ts`                       |
| Reusable picker           | `src/app/shared/components/id-picker/`                          |
| Sidebar menu config       | `src/app/shared/components/sidebar-component/sidebar-component.ts` |
| Global design tokens      | `src/styles.css`                                                |

---

## Known constraints / things to mention if asked

- `/User/GetAll` is Admin-only on the backend, so non-admin roles can't populate the
  technician picker. We mitigate with `allowManualEntry` on the picker.
- `MedicalRecordGetDto` doesn't return `recordId`, so update/deactivate are exposed
  via a dedicated "Manage by Record ID" form rather than inline row buttons.
- `SymptomsJson` column is small; payloads must be compacted before submit.
- Backend prevents a second case for the same citizen — the cases picker hides
  reports whose citizen already has a case so the user can't even try.
- Some `responseType: 'text'` decisions are workarounds for endpoints that should
  return JSON.

---

## Test files

`*.spec.ts` files live next to components but are mostly scaffolded defaults — the
focus of this code base is end-to-end flows against the live backend.
