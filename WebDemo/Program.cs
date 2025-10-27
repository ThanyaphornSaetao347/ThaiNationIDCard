using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// ✅ เปิดให้เสิร์ฟ static files (CSS, JS, IMG, HTML)
app.UseStaticFiles();

// ✅ ตั้งค่าให้ default file เป็น index.html
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});

// ✅ เมื่อเข้า root ให้โหลด index.html จาก wwwroot
app.MapGet("/", async ctx =>
{
    var filePath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "index.html");
    ctx.Response.ContentType = "text/html; charset=utf-8";
    await ctx.Response.SendFileAsync(filePath);
});

app.Run();
