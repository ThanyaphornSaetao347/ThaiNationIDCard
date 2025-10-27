using System.Text;
using WebAgent.Services;

// ✅ เพิ่มตรงนี้ก่อนสร้าง WebApplication
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Register Controller
builder.Services.AddControllers();

// ✅ 2. Register Service สำหรับอ่านบัตร
builder.Services.AddScoped<QuickThaiIdService>();

// ✅ 3. ตั้งค่า CORS ให้ WebDemo (port 18000) เรียกได้
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebDemo",
        policy => policy
            .WithOrigins("https://127.0.0.1:18000") // ✅ WebDemo ที่รันอยู่บนพอร์ต 18000
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// ✅ 4. สร้างแอป
var app = builder.Build();

// ✅ 5. Middleware พื้นฐาน
app.UseHttpsRedirection();

// ✅ 6. เปิดใช้ CORS
app.UseCors("AllowWebDemo");

// ✅ 7. Map Controllers
app.MapControllers();

// ✅ 8. Run Application
app.Run();
