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

[Meterpreter Elevator Source](https://github.com/rapid7/meterpreter/tree/master/source/extensions/priv/server/elevate)

[What happens when I type getsystem? - Cobalt Strike](https://blog.cobaltstrike.com/2014/04/02/what-happens-when-i-type-getsystem/)

[Windows Access Tokens and Alternate Credentials - Cobalt Strike](https://blog.cobaltstrike.com/2015/12/16/windows-access-tokens-and-alternate-credentials/)

[Understanding and Defending Against Access Token Theft: Finding Alternatives to winlogon.exe - SpecterOps](https://posts.specterops.io/understanding-and-defending-against-access-token-theft-finding-alternatives-to-winlogon-exe-80696c8a73b)

[GhostPack\SharpWMI Source](https://github.com/GhostPack/SharpWMI)

[@monoxgas](https://twitter.com/monoxgas/status/1109892490566336512?s=20)
