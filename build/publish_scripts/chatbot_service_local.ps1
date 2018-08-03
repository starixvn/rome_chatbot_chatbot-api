New-Item -Path \\10.18.200.104\www\Rome\Chatbot.API\Local02\app_offline.htm -type file -Force 

dotnet publish -o \\10.18.200.104\www\Rome\Chatbot.API\Local02\ -f netcoreapp2.1 ..\..\src\chatbot\Chatbot.WebApi\Chatbot.WebApi.csproj

Copy-Item -Path \\10.18.200.104\www\Rome\Chatbot.API\Local02\appsettings.local.json -Destination \\10.18.200.104\www\Rome\Chatbot.API\Local02\appsettings.json;    

Remove-Item -Path \\10.18.200.104\www\Rome\Chatbot.API\Local02\app_offline.htm -Force 


New-Item -Path \\10.18.200.103\www\Rome\Chatbot.API\Local02\app_offline.htm -type file -Force 

dotnet publish -o \\10.18.200.103\www\Rome\Chatbot.API\Local02\ -f netcoreapp2.1 ..\..\src\chatbot\Chatbot.WebApi\Chatbot.WebApi.csproj

Copy-Item -Path \\10.18.200.103\www\Rome\Chatbot.API\Local02\appsettings.local.json -Destination \\10.18.200.103\www\Rome\Chatbot.API\Local02\appsettings.json;    

Remove-Item -Path \\10.18.200.103\www\Rome\Chatbot.API\Local02\app_offline.htm -Force 
