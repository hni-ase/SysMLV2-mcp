FROM mcr.microsoft.com/devcontainers/dotnet:1-8.0-bookworm

# Install PostgreSQL client tools and Java
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends \
        postgresql-client \
        openjdk-17-jdk \
        maven \
        gradle \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/*

# Set JAVA_HOME
ENV JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64
ENV PATH="${JAVA_HOME}/bin:${PATH}"

# Install Node.js LTS and TypeScript
RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && nvm install --lts && nvm use --lts && npm install -g typescript" 2>&1

# Install .NET global tools that might be useful
RUN dotnet tool install --global dotnet-ef
RUN dotnet tool install --global dotnet-aspnet-codegenerator

# Add dotnet tools to PATH
ENV PATH="$PATH:/home/vscode/.dotnet/tools"
