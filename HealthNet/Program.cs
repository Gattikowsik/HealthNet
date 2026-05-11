using System.Text;
using HealthNet.Repository;
using HealthNet.Repository.LabTestRepo;
using HealthNet.Repository.ComplianceRecord;
using HealthNet.Repository.User;
using HealthNet.Services;
using HealthNet.Services.LabTestServices;
using HealthNet.Services.PaginationService;
using HealthNet.Services.ComplianceRecordServices;
using HealthNet.Services.UserServices;
using HealthNetDb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using HealthNet.Repository.PatientRepository;
using HealthNet.Services.PatientServices;
using HealthNet.Services.OutbreakMonitoringServices;
using HealthNet.Repository.OutbreakMonitoringRepository;
using HealthNetDb.Entities;
using HealthNet.Utility;
using HealthNet.Repository.MedicalRepository;
using HealthNet.Services.MedicalServices;
using HealthNet.Repository.LabReportRepo;
using HealthNet.Services.LabReportServices;
using HealthNet.Services.AuditService;
using HealthNet.Repository.AuditRepository;
using HealthNet.Repository.ReportingAndAnalytics;
using HealthNet.Services.ReportingAndAnalyticsServices;
using HealthNet.Services.AutoTriage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISubmitSymptomReportRepository, SubmitSymptomReportRepository>();
builder.Services.AddScoped<ISubmitSymptomReportService, SubmitSymptomReportService>();
builder.Services.AddScoped<ILaboratoryTestingRepository, LaboratoryTestingRepository>();
builder.Services.AddScoped<ILaboratoryTestingService, LaboratoryTestingService>();
builder.Services.AddScoped<IPaginationService, PaginationService>();
builder.Services.AddScoped<IComplianceRecordService, ComplianceRecordService>();
builder.Services.AddScoped<IComplianceRepository, ComplianceRepository>();
builder.Services.AddScoped<IPatientManagementRepository, PatientManagementRepository>();
builder.Services.AddScoped<IPatientManagementService, PatientManagementService>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IOutbreakMonitoringServices, OutbreakMonitoringServices>();
builder.Services.AddScoped<IOutBreakMonitoringRepository, OutbreakMonitoringRepository>();
builder.Services.AddScoped<ISymptomRiskEvaluator, SymptomRiskEvaluator>();
builder.Services.AddHostedService<AutoTriageBackgroundService>();
builder.Services.AddScoped<IPatientManagementRepository,PatientManagementRepository>();
builder.Services.AddScoped<IPatientManagementService,PatientManagementService>();
builder.Services.AddScoped<IOutbreakMonitoringServices,OutbreakMonitoringServices>();
builder.Services.AddScoped<IOutBreakMonitoringRepository,OutbreakMonitoringRepository>();
builder.Services.AddScoped<ILabReportRepository, LabReportRepository>();
builder.Services.AddScoped<ILabReportService, LabReportService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddHttpClient<LocationHelper>();
builder.Services.AddScoped<IReportingAndAnalyticsRepository,ReportingAndAnalyticsRepository>();
builder.Services.AddScoped<IReportingAndAnalyticsService,ReportingAndAnalyticsService>();
builder.Services.AddDbContext<HealthNetContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authentication using Bearer scheme"
    });
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() }
    });
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular", policy =>
	{
		policy.WithOrigins("http://localhost:4200")
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();
app.Run();