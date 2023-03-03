FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /source

# copy files for restore
COPY ./*/*.csproj ./*.sln ./

# move csproj files to correct directory
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

# restore packages
RUN dotnet restore /source/campaign-calculator.sln

# copy rest of source
COPY . .

# check .editorconfig settings
RUN dotnet tool restore
RUN dotnet format --verify-no-changes

# build and check code style
RUN dotnet build /source/campaign-calculator.sln /p:TreatWarningsAsErrors="true"

# run unit tests
RUN dotnet test -c Release

