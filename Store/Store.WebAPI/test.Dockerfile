FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app
COPY ./Store.WebAPI .

ENTRYPOINT ["dotnet", "Store.WebAPI.dll"]