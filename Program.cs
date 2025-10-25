using hongWenAPP.Helpers;
using hongWenAPP.Middleware;
using hongWenAPP.Services;
using Serilog;
using SharedHongWenApp.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog (SharedServices handles the full configuration)
builder.Services.AddSharedServices(builder.Configuration, "hongWenAPP");

// Use Serilog as the logging provider (Log.Logger already configured by SharedServices)
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register HttpClient and HttpContextAccessor
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "AspNetCore.Session";
    options.Cookie.Path = "/";
    options.Cookie.SameSite = SameSiteMode.Lax; // Changed from default for better HTTP compatibility
    // Use conditional security policy - secure only for HTTPS
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Works for both HTTP and HTTPS
});

// Register authentication service
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<ITermService, TermService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ILevelService, LevelService>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IClassSectionService, ClassSectionService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IWaitlistService, WaitlistService>();
builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Register NEW Invoice-Based Payment System Services
builder.Services.AddScoped<IStudentCourseService, StudentCourseService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentNewService, PaymentNewService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<ISupplyService, SupplyService>();

// Register Report Export Service
builder.Services.AddScoped<ReportExportService>();

// Register AIA Statement Service
builder.Services.AddScoped<IAIAStatementService, AIAStatementService>();

// Register Account Statement Service
builder.Services.AddScoped<IAccountStatementService, AccountStatementService>();

builder.Services.AddScoped<ReturnHelper>();

// Register the new unified request handler (replaces old handlers)
builder.Services.AddTransient<UnifiedRequestHandler>();

// Configure HttpClient with the unified handler
builder.Services.AddHttpClient("AuthJwtClient")
    .AddHttpMessageHandler<UnifiedRequestHandler>();

// Configure API endpoints
builder.Services.AddOptions();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add Serilog request logging
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add session middleware
app.UseSession();

app.UseRouting();

// Use custom authentication middleware (handles all auth logic)
app.UseCustomAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Log application startup
Log.Information("hongWenAPP application started successfully");

app.Run();
