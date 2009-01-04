set out=NunitResult.wiki
echo ^#summary nunit testing results > %out%
echo ^<p^>>> %out%
date /t >> %out%
echo ^<p^>>> %out%
echo ^{^{^{ >> %out%
..\lib\nunit\nunit-console.exe ProjectGenerator.nunit /nologo /nodots /labels  >> %out%
echo ^}^}^} >> %out%
