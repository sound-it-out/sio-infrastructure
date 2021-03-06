dotnet pack ./src/SIO.Infrastructure/SIO.Infrastructure.csproj -c Release
dotnet pack ./src/SIO.Infrastructure.Azure.ServiceBus/SIO.Infrastructure.Azure.ServiceBus.csproj -c Release
dotnet pack ./src/SIO.Infrastructure.Azure.Storage/SIO.Infrastructure.Azure.Storage.csproj -c Release
dotnet pack ./src/SIO.Infrastructure.EntityFrameworkCore/SIO.Infrastructure.EntityFrameworkCore.csproj -c Release
dotnet pack ./src/SIO.Infrastructure.EntityFrameworkCore.SqlServer/SIO.Infrastructure.EntityFrameworkCore.SqlServer.csproj -c Release
dotnet pack ./src/SIO.Infrastructure.RabbitMQ/SIO.Infrastructure.RabbitMQ.csproj -c Release
dotnet pack ./src/SIO.Infrastructure.Serialization.Json/SIO.Infrastructure.Serialization.Json.csproj -c Release
dotnet pack ./src/SIO.Infrastructure.Testing/SIO.Infrastructure.Testing.csproj -c Release
dotnet nuget push ./src/SIO.Infrastructure/bin/Release/SIO.Infrastructure.0.0.6.nupkg -k oy2gwt4kqd764a2e6djqreyraexxgwlujpf5fog7xdtzzy -s https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push ./src/SIO.Infrastructure.Azure.ServiceBus/bin/Release/SIO.Infrastructure.Azure.ServiceBus.0.0.6.nupkg -k oy2gwt4kqd764a2e6djqreyraexxgwlujpf5fog7xdtzzy -s https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push ./src/SIO.Infrastructure.Azure.Storage/bin/Release/SIO.Infrastructure.Azure.Storage.0.0.6.nupkg -k oy2gwt4kqd764a2e6djqreyraexxgwlujpf5fog7xdtzzy -s https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push ./src/SIO.Infrastructure.EntityFrameworkCore/bin/Release/SIO.Infrastructure.EntityFrameworkCore.0.0.6.nupkg -k oy2gwt4kqd764a2e6djqreyraexxgwlujpf5fog7xdtzzy -s https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push ./src/SIO.Infrastructure.EntityFrameworkCore.SqlServer/bin/Release/SIO.Infrastructure.EntityFrameworkCore.SqlServer.0.0.6.nupkg -k oy2gwt4kqd764a2e6djqreyraexxgwlujpf5fog7xdtzzy -s https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push ./src/SIO.Infrastructure.RabbitMQ/bin/Release/SIO.Infrastructure.RabbitMQ.0.0.6.nupkg -k oy2gwt4kqd764a2e6djqreyraexxgwlujpf5fog7xdtzzy -s https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push ./src/SIO.Infrastructure.Serialization.Json/bin/Release/SIO.Infrastructure.Serialization.Json.0.0.6.nupkg -k oy2gwt4kqd764a2e6djqreyraexxgwlujpf5fog7xdtzzy -s https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push ./src/SIO.Infrastructure.Testing/bin/Release/SIO.Infrastructure.Testing.0.0.6.nupkg -k oy2gwt4kqd764a2e6djqreyraexxgwlujpf5fog7xdtzzy -s https://api.nuget.org/v3/index.json --skip-duplicate