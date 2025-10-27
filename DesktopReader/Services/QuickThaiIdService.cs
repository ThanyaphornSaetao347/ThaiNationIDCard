using System;
using System.Drawing;
using System.IO;
using ThaiNationalIDCard;

namespace DesktopReader.Services
{
	public class QuickThaiIdService
	{
		public ThaiIdDto ReadAll()
		{
			Console.WriteLine("🟢 เริ่มต้นอ่านบัตรประชาชน...");
			ThaiIDCard idcard = new ThaiIDCard();

			try
			{
				Console.WriteLine("🟡 เรียก idcard.readAll() ...");
				Personal? p = idcard.readAll();

				if (p is null)
				{
					Console.WriteLine("🔴 ไม่พบข้อมูลจากบัตร หรือยังไม่เสียบบัตร");
					throw new Exception("❌ ไม่สามารถอ่านข้อมูลจากบัตรได้");
				}

				Console.WriteLine("🟢 อ่านข้อมูลสำเร็จ");
				Console.WriteLine($"เลขบัตร: {p.Citizenid}");
				Console.WriteLine($"ชื่อ-นามสกุล: {p.Th_Firstname} {p.Th_Lastname}");
				Console.WriteLine($"วันเกิด: {p.Birthday}");
				Console.WriteLine($"เพศ: {p.Sex}");
				Console.WriteLine($"ผู้ออกบัตร: {p.Issuer}");

				byte[]? photoBytes = null;

				// ✅ รองรับทั้ง PhotoBitmap และ PhotoRaw
				if (p.PhotoBitmap != null)
				{
					using (var ms = new MemoryStream())
					{
						p.PhotoBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
						photoBytes = ms.ToArray();
					}
				}
				else if (p.PhotoRaw != null)
				{
					photoBytes = p.PhotoRaw;
				}
				else
				{
					Console.WriteLine("⚠️ ไม่มีรูปภาพในข้อมูลบัตร");
				}

				return new ThaiIdDto
				{
					CitizenId = p.Citizenid,
					ThFullName = $"{p.Th_Prefix}{p.Th_Firstname} {p.Th_Lastname}",
					EnFullName = $"{p.En_Prefix}{p.En_Firstname} {p.En_Lastname}",
					BirthDate = p.Birthday,
					Gender = p.Sex,
					IssueDate = p.Issue,
					ExpireDate = p.Expire,
					IssuePlace = p.Issuer,
					Address = p.Address,
					PhotoJpeg = photoBytes
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine("❌ Exception: " + ex.Message);
				throw;
			}
		}

	}

	public class ThaiIdDto
	{
		public string CitizenId { get; set; } = string.Empty;
		public string ThFullName { get; set; } = string.Empty;
		public string EnFullName { get; set; } = string.Empty;
		public string Gender { get; set; } = string.Empty;
		public DateTime BirthDate { get; set; }
		public DateTime IssueDate { get; set; }
		public DateTime ExpireDate { get; set; }
		public string IssuePlace { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public byte[]? PhotoJpeg { get; set; }
	}
}
