# Build on the host's architecture and cross-compile for the target one, which keeps multi-platform builds fast.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /source

# Restore in its own layer so it is cached until a project file changes.
COPY Amethyst.Abstractions/Amethyst.Abstractions.csproj Amethyst.Abstractions/
COPY Amethyst.Extensions/Amethyst.Extensions.csproj Amethyst.Extensions/
COPY Amethyst/Amethyst.csproj Amethyst/
COPY Amethyst.Standalone/Amethyst.Standalone.csproj Amethyst.Standalone/
RUN dotnet restore Amethyst.Standalone -a $TARGETARCH

COPY . .
RUN dotnet publish Amethyst.Standalone -c Release -a $TARGETARCH --no-restore -o /app

# Chiseled images have no shell or package manager and run as a non-root user.
FROM mcr.microsoft.com/dotnet/runtime:10.0-noble-chiseled
WORKDIR /app
COPY --from=build /app .

EXPOSE 25565
ENTRYPOINT ["dotnet", "Amethyst.Standalone.dll"]