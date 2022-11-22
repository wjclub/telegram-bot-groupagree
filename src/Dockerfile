# super standard dockerfile for dotnet
# more info here: https://docs.microsoft.com/en-us/dotnet/core/docker/building-net-docker-images

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /out ./
ENTRYPOINT ["dotnet", "groupagreebot.dll"]