#Get the base net8.0 SDK from Microsoft
FROM mcr.microsoft.com/dotnet/sdk AS build-env

# Set the working directory
WORKDIR /app

# Copy the project file and restore the dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the project files and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "EMS.BACKEND.API.dll"]
