using System.Text;
using WebAgent.Services;
using ThaiNationalIDCard;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// ✅ Register Controller + Service
builder.Services.AddControllers();
builder.Services.AddScoped<QuickThaiIdService>();

// ✅ โหลด Allowed Origins จากไฟล์ .txt
var exeDir = AppContext.BaseDirectory;
var originFilePath = Path.Combine(exeDir, "allowed-origins.txt");

string[] allowedOrigins;

// ถ้าไฟล์มีอยู่ → โหลดทุกบรรทัดที่ไม่ว่าง
if (File.Exists(originFilePath))
{
    allowedOrigins = File.ReadAllLines(originFilePath)
                         .Where(line => !string.IsNullOrWhiteSpace(line))
                         .Select(line => line.Trim())
                         .ToArray();
}
else
{
    // ถ้าไม่มีไฟล์ ให้ใช้ default ค่าเริ่มต้น
    allowedOrigins = new[] { "https://127.0.0.1:18000" };
    Console.WriteLine($"⚠️ ไม่พบไฟล์ allowed-origins.txt — ใช้ค่า default: {string.Join(", ", allowedOrigins)}");
}

// ✅ อนุญาตให้ frontend (WebDemo) เรียก API ได้

    // ✅ ตั้งค่า CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowWebDemo", policy =>
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials());
    });

// ✅ เพิ่ม Controller และ Service
builder.Services.AddControllers();
builder.Services.AddSingleton<ThaiIDCard>();

var app = builder.Build();

// ✅ ใช้ CORS สำหรับทุก request
app.UseCors("AllowWebDemo");

// ✅ Redirect HTTPS (เพื่อให้มั่นใจว่าเรียกได้ทั้ง http/https)
app.UseHttpsRedirection();


app.UseHttpsRedirection();

// =====================================================
// ✅ GLOBAL STATE
// =====================================================
bool isCardInserted = false;
ThaiIDCard idcard = new ThaiIDCard();
string? lastReader = null;

// =====================================================
// ✅ START MONITOR FUNCTION
// =====================================================
void StartMonitor()
{
    try
    {
        var readers = idcard.GetReaders();
        if (readers == null || readers.Length == 0)
        {
            Console.WriteLine("⚠️ ไม่พบเครื่องอ่านบัตร");
            return;
        }

        lastReader = readers[0];
        Console.WriteLine($"🎯 Start monitoring: {lastReader}");

        idcard.eventCardInserted += (readerName) =>
        {
            Console.WriteLine($"✅ Card Inserted on {readerName}");
            isCardInserted = true;
        };

        idcard.eventCardRemoved += () =>
        {
            Console.WriteLine("🟥 Card Removed");
            isCardInserted = false;
        };

        idcard.MonitorStart(lastReader);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Monitor error: {ex.Message}");
    }
}

// เริ่มต้น Monitor ตอนเปิดโปรแกรม
StartMonitor();

// =====================================================
// ✅ STATUS ENDPOINT
// =====================================================
app.MapGet("/idcard/status", () =>
{
    try
    {
        return Results.Json(new
        {
            success = true,
            present = isCardInserted,
            message = isCardInserted ? "CARD_PRESENT" : "NO_CARD"
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            success = false,
            present = false,
            message = $"ERROR: {ex.Message}"
        });
    }
});

// =====================================================
// ✅ READ ENDPOINT (อ่านเฉพาะตอนเสียบบัตร)
// =====================================================
app.MapPost("/idcard/read", () =>
{
    try
    {
        if (!isCardInserted)
            return Results.Json(new { success = false, message = "NO_CARD" });

        var personal = idcard.readAllPhoto();
        if (personal == null)
            return Results.Json(new { success = false, message = "ไม่สามารถอ่านข้อมูลจากบัตรได้" });

        string? photoBase64 = null;
        if (personal.PhotoRaw != null)
            photoBase64 = Convert.ToBase64String(personal.PhotoRaw);

        return Results.Json(new
        {
            success = true,
            citizenId = personal.Citizenid,
            thFullName = $"{personal.Th_Prefix}{personal.Th_Firstname} {personal.Th_Lastname}",
            enFullName = $"{personal.En_Prefix}{personal.En_Firstname} {personal.En_Lastname}",
            gender = personal.Sex,
            birthDate = personal.Birthday,
            issueDate = personal.Issue,
            expireDate = personal.Expire,
            issuer = personal.Issuer,
            address = personal.Address,
            photoBase64
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new { success = false, message = $"ERROR: {ex.Message}" });
    }
});

app.Run();
