FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["BillTracker.Api/BillTracker.Api.csproj", "BillTracker.Api/"]
COPY ["BillTracker/BillTracker.csproj", "BillTracker/"]
RUN dotnet restore "BillTracker.Api/BillTracker.Api.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "BillTracker.Api/BillTracker.Api.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "BillTracker.Api.dll"]