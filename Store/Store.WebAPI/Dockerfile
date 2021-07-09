FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY Store.WebAPI/*.csproj ./Store.WebAPI/
COPY Store.Cache/*.csproj ./Store.Cache/
COPY Store.FileProvider.Common/*.csproj ./Store.FileProvider.Common/
COPY Store.FileProvider/*.csproj ./Store.FileProvider/
COPY Store.Messaging.Common/*.csproj ./Store.Messaging.Common/
COPY Store.Messaging.Templates/*.csproj ./Store.Messaging.Templates/
COPY Store.Messaging/*.csproj ./Store.Messaging/
COPY Store.Model/*.csproj ./Store.Model/
COPY Store.Repository/*.csproj ./Store.Repository/
COPY Store.Service.Common/*.csproj ./Store.Service.Common/
COPY Store.Service/*.csproj ./Store.Service/

COPY Store.Entity/*.csproj ./Store.Entity/
COPY Store.DAL/*.csproj ./Store.DAL/
COPY Store.Common/*.csproj ./Store.Common/
COPY Store.Model.Common/*.csproj ./Store.Model.Common/
COPY Store.Repository.Common/*.csproj ./Store.Repository.Common/
COPY Store.Cache.Common/*.csproj ./Store.Cache.Common/
COPY Store.Reporting/*.csproj ./Store.Reporting/
COPY Store.Generator/*.csproj ./Store.Generator/
# COPY Store.Repository.Tests/*.csproj ./Store.Repository.Tests/
# COPY Store.Service.Tests/*.csproj ./Store.Service.Tests/
# COPY Store.WebAPI.Tests/*.csproj ./Store.WebAPI.Tests/

RUN dotnet restore

# Copy everything else and build
COPY . ./
WORKDIR /app/Store.WebAPI
RUN dotnet publish -c Release -o ../out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Store.WebAPI.dll"]