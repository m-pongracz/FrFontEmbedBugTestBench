FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# copy and publish app and libraries
COPY . .

COPY fonts/ ../usr/local/share/fonts/

RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0.13-buster-slim

WORKDIR /app
COPY --from=build /usr/local/share/fonts/ ../usr/local/share/fonts/

RUN apt update

RUN apt install libgdiplus --yes

COPY --from=build /app .

ENTRYPOINT ["dotnet", "FrFontSubsetBug.dll"]
#CMD tail -f /dev/null