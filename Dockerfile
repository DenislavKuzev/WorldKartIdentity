FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy solution + project files first (better caching + clearer errors)
COPY WorldKartIdentity.sln ./
COPY WorldKartIdentity/WorldKartIdentity.csproj WorldKartIdentity/

RUN dotnet restore WorldKartIdentity/WorldKartIdentity.csproj

# copy everything else
COPY . ./

RUN dotnet publish WorldKartIdentity/WorldKartIdentity.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- run ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

# Render provides PORT; ASP.NET must bind to it
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 10000

ENTRYPOINT ["dotnet", "WorldKartIdentity.dll"]
