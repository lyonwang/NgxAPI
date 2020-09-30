FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["NgxAPI/NgxAPI.csproj", "NgxAPI/"]
RUN dotnet restore "NgxAPI/NgxAPI.csproj"
COPY . .
WORKDIR "/src/NgxAPI"
RUN dotnet build "NgxAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "NgxAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NgxAPI.dll"]