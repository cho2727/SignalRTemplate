FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY SignalRTemplate/*.csproj ./SignalRTemplate/
COPY SignalRClient/*.csproj ./SignalRClient/
COPY SignalRTemplate/. ./SignalRTemplate/
COPY SignalRClient/. ./SignalRClient/
#RUN dotnet restore


WORKDIR /source/SignalRTemplate
RUN dotnet publish -r linux-x64 -c release -p:PublishReadyToRun=true -o /publish


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /publish
COPY --from=build /publish ./
ENTRYPOINT ["dotnet", "SignalRTemplate.dll"]