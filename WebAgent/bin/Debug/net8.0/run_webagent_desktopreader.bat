@echo off
REM เปลี่ยน working dir เป็นโฟลเดอร์ที่ไฟล์ .bat อยู่
pushd "%~dp0"

REM เปิด WebAgent.exe (ไม่รอให้ปิด)
start "" "WebAgent.exe"

REM รอ 2 วินาที (เงียบ)
timeout /t 2 /nobreak >nul

REM เปิด DesktopReader.exe
start "" "DesktopReader.exe"

REM กลับไปที่โฟลเดอร์เดิม (ถ้าต้องการ)
popd
