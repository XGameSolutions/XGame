set git_path=%1
set local_dir=%2

echo ---------------------------------------------------------------------
echo [git]:%git_path%
echo [dir]:%local_dir%
if exist "%local_dir%"(
    echo pull...
    git -C %local_dir% pull
)else(
    echo clone...
    git clone %git_path% %local_dir%
)
echo.