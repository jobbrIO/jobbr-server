rd /q /s ilmerged 2>nul
mkdir ilmerged
"C:/Program Files (x86)/Microsoft/ILMerge/ilmerge.exe" /out:ilmerged/Jobbr.Client.dll Jobbr.Client/bin/Release/*.dll /target:library /targetplatform:v4,C:\Windows\Microsoft.NET\Framework64\v4.0.30319 /wildcards /internalize
"C:/Program Files (x86)/Microsoft/ILMerge/ilmerge.exe" /out:ilmerged/Jobbr.Runtime.dll Jobbr.Runtime/bin/Release/*.dll /target:library /targetplatform:v4,C:\Windows\Microsoft.NET\Framework64\v4.0.30319 /wildcards /internalize
"C:/Program Files (x86)/Microsoft/ILMerge/ilmerge.exe" /out:ilmerged/Jobbr.Server.dll Jobbr.Server/bin/Release/*.dll Jobbr.Server.Dapper/bin/Release/Dapper.dll Jobbr.Server.Dapper/bin/Release/Jobbr.Server.Dapper.dll /allowDup /target:library /targetplatform:v4,C:\Windows\Microsoft.NET\Framework64\v4.0.30319 /wildcards /internalize
