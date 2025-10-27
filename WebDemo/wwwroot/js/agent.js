async function readThaiId() {
    const status = document.getElementById('status');
    const img = document.getElementById('photo');

    try {
        status.textContent = '🔄 กำลังตรวจสอบ WebAgent...';
        const ping = await fetch('https://127.0.0.1:17890/idcard/ping');
        if (!ping.ok) throw new Error('❌ Agent ไม่ตอบสนอง');

        status.textContent = '📖 กำลังอ่านข้อมูลจากบัตร...';
        const resp = await fetch('https://127.0.0.1:17890/idcard/read', { method: 'POST' });
        const data = await resp.json();

        if (!data.success) throw new Error(data.message);

        // ✅ แปลงเพศให้อ่านง่าย
        const genderLabel = data.gender === '1' || data.gender === 1 ? 'ชาย' :
            data.gender === '2' || data.gender === 2 ? 'หญิง' : '-';

        // ✅ แสดงข้อมูล
        document.getElementById('citizenId').textContent = data.citizenId;
        document.getElementById('thFullName').textContent = data.thFullName;
        document.getElementById('enFullName').textContent = data.enFullName;
        document.getElementById('gender').textContent = genderLabel;
        document.getElementById('birthDate').textContent = new Date(data.birthDate).toLocaleDateString('th-TH');
        document.getElementById('issueDate').textContent = new Date(data.issueDate).toLocaleDateString('th-TH');
        document.getElementById('expireDate').textContent = new Date(data.expireDate).toLocaleDateString('th-TH');
        document.getElementById('issuer').textContent = data.issuer;
        document.getElementById('address').textContent = data.address;

        // ✅ แสดงรูปภาพ
        if (data.photoBase64) {
            img.src = 'data:image/jpeg;base64,' + data.photoBase64;
        } else {
            img.src = '/img/default-photo.jpg';
        }

        status.textContent = '✅ อ่านข้อมูลสำเร็จ';
    } catch (err) {
        console.error(err);
        status.textContent = '❌ ล้มเหลว: ' + err.message;
    }
}

// ✅ Auto-read ครั้งแรกเมื่อหน้าโหลดเสร็จ
window.addEventListener('DOMContentLoaded', () => {
    console.log('✅ หน้าโหลดครบแล้ว: เริ่มอ่านบัตรอัตโนมัติ...');
    setTimeout(() => {
        readThaiId();
    }, 1000);

    // ✅ อ่านซ้ำทุก 30 วินาที (ปรับเวลาได้)
    setInterval(() => {
        readThaiId();
    }, 1000);
});
