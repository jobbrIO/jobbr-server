cd ..\source
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" Jobbr.Server.sln /Build "Release|Any CPU"

packages\ILMerge.Tools.2.14.1208\tools\ilmerge.exe /out:../tools/Jobbr.Server.dll Jobbr.Server/bin/Release/*.dll /target:library /targetplatform:v4,C:\Windows\Microsoft.NET\Framework64\v4.0.30319 /wildcards /internalize:../tools/internalize_exclude.txt /allowDup:HttpRequestMessageExtensions /allowDup:MediaTypeFormatterExtensions

cd ..\tools
