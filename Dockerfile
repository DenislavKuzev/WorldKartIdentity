# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install Node.js + npm (Node 20 LTS)
RUN apt-get update \
    && apt-get install -y curl ca-certificates gnupg \
    && mkdir -p /etc/apt/keyrings \
    && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key \
       | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" \
       > /etc/apt/sources.list.d/nodesource.list \
    && apt-get update \
    && apt-get install -y nodejs \
    && node -v && npm -v \
    && rm -rf /var/lib/apt/lists/*

# copy solution + csproj for restore cache
COPY WorldKartIdentity.sln ./
COPY WorldKartIdentity/WorldKartIdentity.csproj WorldKartIdentity/
RUN dotnet restore WorldKartIdentity/WorldKartIdentity.csproj

# copy the rest and publish (this is where your csproj npm target runs)
COPY . ./
RUN dotnet publish WorldKartIdentity/WorldKartIdentity.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- run ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENTRYPOINT ["dotnet", "WorldKartIdentity.dll"]
