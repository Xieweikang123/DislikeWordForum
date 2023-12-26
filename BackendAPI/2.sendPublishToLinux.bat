echo delete appsettings.json---------------
del  publish\appsettings.json

echo input password to deploy--------------
scp -r publishUpdateFiles/* root@43.138.32.181:/www/dislikewordforum/backendapi

echo  run systemctl restart dislikeword-dotnet-service--------------
ssh root@43.138.32.181 bash -c "'systemctl restart dislikeword-dotnet-service'"


pause