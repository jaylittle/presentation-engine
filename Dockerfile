FROM node:lts-slim as node-build

WORKDIR /usr/src/app
# copy package.json and restore as distinct layers
COPY PEngine.Core.Web/package.json ./PEngine.Core.Web/
COPY PEngine.Core.Web/yarn.lock ./PEngine.Core.Web/

WORKDIR /usr/src/app/PEngine.Core.Web
RUN rm -rf wwwroot/dist
RUN rm -rf node_modules
RUN yarn install --force

# copy everything else and build front end code
COPY PEngine.Core.Web/. .
RUN NODE_ENV=production yarn build


# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY PEngine.Core.Shared/*.csproj ./PEngine.Core.Shared/
COPY PEngine.Core.Data/*.csproj ./PEngine.Core.Data/
COPY PEngine.Core.Logic/*.csproj ./PEngine.Core.Logic/
COPY --from=node-build /usr/src/app/PEngine.Core.Web/*.csproj ./PEngine.Core.Web/
COPY PEngine.Core.Tests/*.csproj ./PEngine.Core.Tests/
RUN dotnet restore

# copy everything else and build app
COPY PEngine.Core.Shared/. ./PEngine.Core.Shared/
COPY PEngine.Core.Data/. ./PEngine.Core.Data/
COPY PEngine.Core.Logic/. ./PEngine.Core.Logic/
COPY --from=node-build /usr/src/app/PEngine.Core.Web/. ./PEngine.Core.Web/
COPY PEngine.Core.Tests/. ./PEngine.Core.Tests/
COPY Scripts/. ./Scripts/
COPY docker-version.txt ./
WORKDIR /source
RUN bash ./Scripts/linux_docker_build.sh

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=dotnet-build /app ./
VOLUME /app
ENTRYPOINT ["dotnet", "PEngine.Core.Web.dll"]