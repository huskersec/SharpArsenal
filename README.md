# SharpArsenal
Repository of Windows offensive techniques implemented in C#



### GetSystem/GetSystemDLL
Implements three techniques: 

1. Service/pipe creation and execution of cmd.exe /c echo getsysdata > \\\\.\pipe\getsys
2. Service/pipe creation and execution of DLL written to disk that executes rundll32.exe GetSystemDLL.dll,ConnectPipe
3. Steals a token from the winlogon process and executes a process with the token


### StealToken
Steal a token from a given PID or process name and create a new process with the token.


### MakeToken
Given a username and password, utilize the LogonUserA() function to interact with network resources as the provided user.
