::@echo off  
rem 正在搜索...  
rem 删除文件  
for /f "delims=" %%i in ('dir /b /a-d /s "*.tga"') do del %%i  
for /f "delims=" %%i in ('dir /b /a-d /s "*.sfp"') do del %%i  
rem 删除完毕  
pause  