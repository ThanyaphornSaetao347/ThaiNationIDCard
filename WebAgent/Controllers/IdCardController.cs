using Microsoft.AspNetCore.Mvc;
using WebAgent.Services;
using System;

namespace WebAgent.Controllers
{
    [ApiController]
    [Route("idcard")]
    public class IdCardController : ControllerBase
    {
        private readonly QuickThaiIdService _thaiIdService;

        public IdCardController(QuickThaiIdService thaiIdService)
        {
            _thaiIdService = thaiIdService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { status = "ok", service = "Thai ID Reader Agent" });
        }

        [HttpPost("read")]
        public IActionResult Read()
        {
            try
            {
                var personal = _thaiIdService.ReadAll();

                return Ok(new
                {
                    success = true,
                    message = "อ่านข้อมูลสำเร็จ",
                    citizenId = personal.Citizenid,
                    thFullName = $"{personal.Th_Prefix}{personal.Th_Firstname} {personal.Th_Lastname}",
                    enFullName = $"{personal.En_Prefix}{personal.En_Firstname} {personal.En_Lastname}",
                    gender = personal.Sex,
                    birthDate = personal.Birthday,
                    issueDate = personal.Issue,
                    expireDate = personal.Expire,
                    issuer = personal.Issuer,
                    address = personal.Address,
                    // ✅ ใช้ PhotoRaw ที่เป็น byte[] แล้วแปลงเป็น base64 เอง
                    photoBase64 = personal.PhotoRaw != null ? Convert.ToBase64String(personal.PhotoRaw) : null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
