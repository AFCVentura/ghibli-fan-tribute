# Etapa 1: compila e publica com o SDK completo
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Só o .csproj primeiro: se nenhum pacote mudou, o Docker reaproveita o restore do build anterior
COPY src/GhibliTribute/GhibliTribute.csproj src/GhibliTribute/
RUN dotnet restore src/GhibliTribute/GhibliTribute.csproj

# O Tailwind não roda aqui (não há node_modules), então o site usa o wwwroot/css/theme.css commitado.
# Sem --no-restore: com só o .csproj, o restore acima não enxerga os componentes Razor e deixa de fora
# o pacote que traz o _framework/blazor.web.js. Este segundo restore completa isso (o resto vem do cache).
COPY src/ src/
RUN dotnet publish src/GhibliTribute/GhibliTribute.csproj -c Release -o /app

# Etapa 2: só o runtime do ASP.NET, bem menor e sem SDK
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Usuário sem privilégios que já vem na imagem; o .NET escuta na 8080 por padrão
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "GhibliTribute.dll"]
