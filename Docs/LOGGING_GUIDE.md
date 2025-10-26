# hongWenAPP Logging Guide

## 🎯 Quick Answer: Where Are My Logs?

Your logs are being created! They are located in:

```
hongWen_APP/bin/Debug/net9.0/Logs/
```

**Important:** The `Logs` folder is in your **build output directory**, not your source code folder!

## 📂 Log Files Created

Your application creates **3 types of log files**:

### 1. Main Application Log
```
hongWenAPP-20250125.log  (daily rotation)
```
- **Contains:** All Information, Warning, and Error logs
- **Retention:** 30 days
- **Max Size:** 50MB per file
- **Example:**
  ```
  2025-01-25 10:30:15.123 +07:00 [INF] Loading add payment form <s:hongWenAPP.Controllers.PaymentController> <c:12345-abc-67890>
  2025-01-25 10:30:16.456 +07:00 [ERR] Failed to load add payment form <s:hongWenAPP.Controllers.PaymentController>
  System.Exception: Student not found
     at hongWenAPP.Controllers.PaymentController.AddPayment()
  ```

### 2. Error-Only Log
```
hongWenAPP-errors-20250125.log  (daily rotation)
```
- **Contains:** Only Error and Fatal logs
- **Retention:** 90 days (kept longer for troubleshooting)
- **Max Size:** 50MB per file
- **Use Case:** Quick review of errors without noise

### 3. Performance Log
```
hongWenAPP-performance-20250125.log  (daily rotation)
```
- **Contains:** Operations taking > 1 second
- **Retention:** 7 days
- **Use Case:** Identify slow database queries, API calls, etc.

## 🖥️ Console Output

Your application also logs to the **console** (Visual Studio Output window or terminal):

**Development Mode:**
```
[10:30:15 INF] Loading add payment form <s:PaymentController> <c:12345> <m:DESKTOP-ABC> <t:8>
```

**Production Mode:**
```
[10:30:15 INF] Loading add payment form <c:12345>
```

## 📊 Log Levels

Your application uses these log levels:

| Level | Usage | Example |
|-------|-------|---------|
| **Information** | Normal operations | `_logger.LogInformation("Loading add payment form")` |
| **Warning** | Issues that don't break functionality | `_logger.LogWarning("Student not found")` |
| **Error** | Exceptions and errors | `_logger.LogError(ex, "Failed to load payment")` |
| **Fatal** | Application-breaking issues | `Log.Fatal(ex, "Application crashed")` |

## 🔍 How to View Logs

### Option 1: Visual Studio Output Window
1. Run your application in Debug mode
2. Go to **View** → **Output**
3. Select **Debug** from dropdown
4. See real-time console logs

### Option 2: Open Log Files Directly
1. Navigate to: `hongWen_APP\bin\Debug\net9.0\Logs\`
2. Open today's log file: `hongWenAPP-YYYYMMDD.log`
3. Use a text editor or log viewer

### Option 3: Use Visual Studio Code
1. Install **Log Viewer** extension
2. Open the `Logs` folder in VS Code
3. Get syntax highlighting and filtering

### Option 4: PowerShell (Real-time Monitoring)
```powershell
# Navigate to project directory
cd D:\ALL2025\Sovantha_Project\HongWen_APP\hongWen_APP

# Watch logs in real-time
Get-Content .\bin\Debug\net9.0\Logs\hongWenAPP-*.log -Wait -Tail 50
```

### Option 5: Windows PowerShell (Filter Errors Only)
```powershell
# Show only errors from today's log
Get-Content .\bin\Debug\net9.0\Logs\hongWenAPP-*.log | Select-String "ERR"
```

## 📝 Log Format Explained

Your logs use this rich format:
```
{Timestamp} [{Level}] {Message} <s:{SourceContext}> <c:{CorrelationId}> <u:{UserId}> <m:{MachineName}> <t:{ThreadId}> <ip:{IpAddress}> <ua:{UserAgent}> <path:{RequestPath}> <method:{RequestMethod}> <env:{Environment}> <svc:{ServiceName}>
{Exception}
```

**Example:**
```
2025-01-25 10:30:15.123 +07:00 [ERR] Failed to record payment <s:hongWenAPP.Controllers.PaymentController> <c:abc-123-def> <u:admin@system.com> <m:DESKTOP-PC> <t:12> <ip:127.0.0.1> <ua:Mozilla/5.0> <path:/Payment/AddPayment> <method:POST> <env:Development> <svc:hongWenAPP>
System.Exception: Student not found
   at hongWenAPP.Controllers.PaymentController.AddPayment(CreatePaymentDTO paymentDto)
```

**Fields:**
- `<s:>` = Source Context (Which controller/class)
- `<c:>` = Correlation ID (Track request across logs)
- `<u:>` = User ID
- `<m:>` = Machine Name
- `<t:>` = Thread ID
- `<ip:>` = IP Address
- `<ua:>` = User Agent
- `<path:>` = Request Path
- `<method:>` = HTTP Method
- `<env:>` = Environment (Development/Production)
- `<svc:>` = Service Name (hongWenAPP)

## 🚀 What Changed in Your Program.cs

### Before (Conflicting Configuration):
```csharp
// ❌ Problem: Tried to read Serilog config from appsettings.json (but it doesn't exist)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // No Serilog section!
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

builder.Services.AddSharedServices(builder.Configuration, "hongWenAPP");

// ❌ Tried to configure Serilog AGAIN
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)  // Still no Serilog section!
    .ReadFrom.Services(services)
    ...
);
```

### After (Clean Configuration):
```csharp
// ✅ Let SharedServices handle ALL Serilog configuration
builder.Services.AddSharedServices(builder.Configuration, "hongWenAPP");

// ✅ Just tell ASP.NET Core to use the pre-configured Serilog
builder.Host.UseSerilog();

// ✅ Added request logging
app.UseSerilogRequestLogging();

// ✅ Added proper shutdown handling
try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();  // Ensure all logs are written before exit
}
```

## 🔧 Troubleshooting

### Problem: Still No Logs?

1. **Check if Logs folder exists:**
   ```
   hongWen_APP\bin\Debug\net9.0\Logs\
   ```

2. **Check folder permissions:**
   - Make sure your user has write access

3. **Check if application is running:**
   - Console should show: `"Logs location: D:\...\Logs"`

4. **Check for old log files:**
   ```powershell
   Get-ChildItem .\bin\Debug\net9.0\Logs\ | Sort-Object LastWriteTime -Descending
   ```

### Problem: Logs Are Too Verbose

Edit `SharedHongWenApp\DependencyInjection\SharedServiceContainer.cs`:
```csharp
// Change from Information to Warning
.MinimumLevel.Warning()
```

### Problem: Want JSON Format Logs

Install: `Serilog.Formatting.Compact`

Edit `SharedServiceContainer.cs`:
```csharp
using Serilog.Formatting.Compact;

.WriteTo.File(
    new CompactJsonFormatter(),
    Path.Combine(logDirectory, $"{serviceName}-.json"),
    ...
)
```

## 📚 Using Logs in Your Controllers

Your controllers already use logging correctly:

```csharp
public class PaymentController : Controller
{
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ILogger<PaymentController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> AddPayment()
    {
        // ✅ Information - normal operation
        _logger.LogInformation("Loading add payment form");

        try
        {
            // Your code...
        }
        catch (Exception ex)
        {
            // ✅ Error - with exception details
            _logger.LogError(ex, "Failed to load add payment form");
            return PartialView("_Error");
        }
    }
}
```

## 🎉 Summary

✅ **Logs ARE being created** - in `bin\Debug\net9.0\Logs\`  
✅ **Console output works** - check Visual Studio Output window  
✅ **3 log files** - main, errors, performance  
✅ **Rich context** - correlation IDs, user info, request details  
✅ **Automatic rotation** - daily files, automatic cleanup  
✅ **Production-ready** - proper error handling and shutdown  

**Next time you see an error:**
1. Check Visual Studio Output window for immediate feedback
2. Open `Logs\hongWenAPP-errors-YYYYMMDD.log` for detailed stack traces
3. Use correlation ID to track requests across multiple log entries

Happy debugging! 🐛🔍

