
Remove-Item ./MultiplayerChessGame.Server.linux-arm.zip
Set-Location .\src\MultiplayerChessGame.Server
dotnet publish -r linux-arm --no-self-contained
Set-Location ..

# zip
7z.exe a -tzip MultiplayerChessGame.Server.linux-arm.zip ./MultiplayerChessGame.Server/bin/Debug/netcoreapp3.1/linux-arm/publish/*
Move-Item ./MultiplayerChessGame.Server.linux-arm.zip ..
Set-Location ..
