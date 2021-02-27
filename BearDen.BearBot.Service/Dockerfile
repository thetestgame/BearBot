FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BearDen.BearBot.Service/BearDen.BearBot.Service.csproj", "BearDen.BearBot.Service/"]
RUN dotnet restore "BearDen.BearBot.Service/BearDen.BearBot.Service.csproj"
COPY . .
WORKDIR "/src/BearDen.BearBot.Service"
RUN dotnet build "BearDen.BearBot.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BearDen.BearBot.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BearDen.BearBot.Service.dll"]
