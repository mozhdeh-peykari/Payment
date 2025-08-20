# Stage 1 - Base SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8080
EXPOSE 8081

# Copy solution file and restore dependencies
COPY Payment.sln ./
COPY src/Presentation/Presentation.csproj src/Presentation/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Application/Application.csproj src/Application/
COPY src/Persistence/Persistence.csproj src/Persistence/

RUN dotnet restore Payment.sln

# Copy everything and build
COPY . .
WORKDIR /src/src/Presentation
RUN dotnet publish -c Release -o /app/publish

# Stage 2 - Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Presentation.dll"]