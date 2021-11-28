# SharpArsenal
Repository of Windows offensive techniques implemented in C#. Meant to be a study of techniques implemented in Cobalt Strike's Beacon, Metasploit's Meterpreter, and some Mimikatz functionality as well. 



### GetSystem/GetSystemDLL
Implements three techniques: 

1. Service/pipe creation and execution of cmd.exe /c echo getsysdata > \\\\.\pipe\getsys
2. Service/pipe creation and execution of DLL written to disk that executes rundll32.exe GetSystemDLL.dll,ConnectPipe
3. Steals a token from the winlogon process and executes a process with the token


### StealToken
Steal a token from a given PID or process name and create a new process with the token.


### MakeToken
Given a username and password, utilize the LogonUserA() function to interact with network resources as the provided user.




### References

[Meterpreter elevator source](https://github.com/rapid7/meterpreter/tree/master/source/elevator)
[What happens when I type getsystem? - Cobalt Strike](https://blog.cobaltstrike.com/2014/04/02/what-happens-when-i-type-getsystem/)
