###### Dev Stage, server should be mounted to /app in container
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
#Work in app directory
WORKDIR /app
# Expose for hot reload or browser
EXPOSE 5000
# Use dotnet watch for hot reload
CMD ["dotnet", "watch", "run", "--urls=http://0.0.0.0:5000"]
######

###### Build phase
#asp.net 8.0 sdk image, this is the image that will be used to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#Directory commands are taking place in is /app for the container
WORKDIR /app
# copy csproj and related files
COPY *.csproj ./
#Reuse the cache for the project, then restore dependencies
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore
# Copy the rest
COPY . ./
#Publish the project with the cache
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -c Release -r linux-x64 --self-contained false -o /app/publish
######

###### Run phase for production
#asp.net 8.0 runtime image, much smaller than the build sdk image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS production
#Working in the directory /app
WORKDIR /app
#Copy the published project from the build phase to the run phase
COPY --from=build /app/publish .
#Dotnet command to run the project
ENTRYPOINT ["dotnet", "Server.dll"]
#Running on port 5000
EXPOSE 5000
######