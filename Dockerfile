FROM microsoft/aspnetcore-build:2 AS build-env

WORKDIR /apps

# RESTORE PACKAGES
COPY CorpoGameApp/CorpoGameApp.csproj ./CorpoGameApp/
RUN dotnet restore CorpoGameApp/CorpoGameApp.csproj
COPY CorpoGameApp.Test/CorpoGameApp.Test.csproj ./CorpoGameApp.Test/
RUN dotnet restore CorpoGameApp.Test/CorpoGameApp.Test.csproj

# COPY SOURCE
COPY . .

# RUN ALL UNITTESTS
RUN dotnet test CorpoGameApp.Test/CorpoGameApp.Test.csproj

# PUBLISH
RUN dotnet publish CorpoGameApp/CorpoGameApp.csproj -o /publish

# SETUP RUNTIME
FROM microsoft/aspnetcore:2
COPY --from=build-env /publish /publish
WORKDIR /publish
ENTRYPOINT [ "dotnet", "CorpoGameApp.dll" ]