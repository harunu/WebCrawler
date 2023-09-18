

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory
WORKDIR /src

# Copy csproj files and restore any dependencies (via dotnet restore)
COPY ["WebCrawler/WebCrawler.csproj", "WebCrawler/"]
COPY ["WebCrawler.Data/WebCrawler.Data.csproj", "WebCrawler.Data/"]
RUN dotnet restore "WebCrawler/WebCrawler.csproj"
RUN dotnet restore "WebCrawler.Data/WebCrawler.Data.csproj"

# Copy all the other files for building the project
COPY WebCrawler/ WebCrawler/
COPY WebCrawler.Data/ WebCrawler.Data/

# Set the working directory to the WebCrawler project directory
WORKDIR "/src/WebCrawler"

# Build the project
RUN dotnet build "WebCrawler.csproj" -c Release -o /app/build

# Use the ASP.NET runtime image to create a runtime container
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy the build app to the base image and define entry point
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "WebCrawler.dll"]
