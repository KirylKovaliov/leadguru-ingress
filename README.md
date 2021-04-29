# Step by Step instructions how to connect to the production

- Make sure dotnet 5 is installed
- Download **credentials.json** from the shared link
- Set **GOOGLE_APPLICATION_CREDENTIALS** environment variable with file path to the local file. For the details please refer to <a href="https://cloud.google.com/pubsub/docs/quickstart-client-libraries">official docs</a>
- Run application

#### Powershell

```Powershell
$env:GOOGLE_APPLICATION_CREDENTIALS="C:\Users\<User>\Downloads\credentials.json"
dotnet restore LeadGuru.Ingress
dotnet run --project LeadGuru.Ingress
```

#### Bash

```Bash
export GOOGLE_APPLICATION_CREDENTIALS="~/Downloads/credentials.json"
dotnet restore
dotnet run --project LeadGuru.Ingress
```

#### Docker

```cmd
docker build . -f LeadGuru.Ingress/Dockerfile  -t leadguru-ingress
docker run -v <path to the credentials file>:/creds/credentials.json leadguru-ingress
```
