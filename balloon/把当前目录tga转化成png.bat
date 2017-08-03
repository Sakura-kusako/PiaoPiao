::@echo off  
rem 正在搜索...  
rem 转换文件  
for /f "delims=" %%i in ('dir /b /a-d /s "*.tga"') do change.exe %%i  
rem 转换完毕  
pause  