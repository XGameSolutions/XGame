set dir=%1
if exist "%dir%" (
    echo mkdir:%dir%
    md %dir%
)