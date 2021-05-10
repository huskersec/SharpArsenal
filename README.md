# SharpArsenal
Repository of Windows offensive techniques implemented in C#



### GetSystem/GetSystemDLL
Implements three techniques: 

1. Service/pipe creation and execution of cmd.exe /c echo getsysdata > \\.\pipe\getsys
2. Service/pipe creation and execution of DLL written to disk that executes rundll32.exe GetSystemDLL.dll,ConnectPipe
3. Steals a token from the winlogon process and execution with the token
