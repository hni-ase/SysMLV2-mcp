# SysML v2 MCP server — Docker image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY mcp/SysMLv2Mcp.Tools.csproj mcp/
RUN dotnet restore mcp/SysMLv2Mcp.Tools.csproj

COPY . .
RUN dotnet publish mcp/SysMLv2Mcp.Tools.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5214
EXPOSE 5214

ENTRYPOINT ["dotnet", "SysMLv2Mcp.Tools.dll", "--mode", "http"]
