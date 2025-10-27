using System;
using ThaiNationalIDCard;

namespace WebAgent.Services
{
    public class QuickThaiIdService
    {
        private readonly ThaiIDCard idcard;

        public QuickThaiIdService()
        {
            idcard = new ThaiIDCard();
        }

        public Personal ReadAll()
        {
            try
            {
                var personal = idcard.readAllPhoto();
                if (personal == null)
                    throw new Exception("ไม่สามารถอ่านข้อมูลจากบัตรได้");

                return personal;
            }
            catch (Exception ex)
            {
                throw new Exception($"เกิดข้อผิดพลาดในการอ่านบัตร: {ex.Message}");
            }
        }
    }
}
