FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS runtime
RUN mkdir -p /home/dotnet/apiserver
WORKDIR /home/dotnet/apiserver
COPY publish/. ./
ENTRYPOINT ["dotnet", "SignalRTemplate.dll"]