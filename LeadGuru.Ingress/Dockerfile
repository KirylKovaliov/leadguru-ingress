FROM mcr.microsoft.com/dotnet/sdk:5.0

ENV GOOGLE_APPLICATION_CREDENTIALS=/creds/

WORKDIR /app
COPY . /app

RUN dotnet restore /app/LeadGuru.Ingress

ENTRYPOINT ['dotnet', 'run', '--project' 'LeadGuru.Ingress']