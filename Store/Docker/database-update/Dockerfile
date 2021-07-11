FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# Install dotnet-ef and ensure global tools dir is on the PATH: 
# https://github.com/dotnet/dotnet-docker/issues/520. This will be needed for the database update.
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /app

COPY ./Docker/database-update/entrypoint.sh /usr/local/bin/

RUN chmod +x /usr/local/bin/entrypoint.sh

ENTRYPOINT ["entrypoint.sh"]