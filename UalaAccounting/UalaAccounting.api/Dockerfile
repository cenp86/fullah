# Usa la imagen base oficial del SDK de .NET 8 para la etapa de construcción
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH

# Establece el directorio de trabajo
WORKDIR /app 
EXPOSE 80
EXPOSE 443

# Copia el archivo de solución y el archivo del proyecto
COPY UalaAccounting/*.sln ./
COPY UalaAccounting/UalaAccounting.api/UalaAccounting.api.csproj UalaAccounting/UalaAccounting.api/
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

# Restaura las dependencias del proyecto
RUN dotnet restore UalaAccounting/UalaAccounting.api/UalaAccounting.api.csproj

# Copia el resto del código fuente
COPY UalaAccounting/. ./UalaAccounting/

# Establece el directorio de trabajo para la compilación
WORKDIR /app/UalaAccounting/UalaAccounting.api

# Compila y publica el proyecto
RUN dotnet publish -c Release -o /app/publish -a $TARGETARCH

# Usa la imagen base oficial del runtime de .NET 8 para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Establece el directorio de trabajo
WORKDIR /app

# Instala el cliente MySQL
RUN apt-get update \
    && apt-get install -y curl \
    && apt-get install -y default-mysql-client   

# Copia los archivos publicados desde la etapa de construcción
COPY --from=build /app/publish .

# Define el punto de entrada para la aplicación
ENTRYPOINT ["dotnet", "UalaAccounting.api.dll"]