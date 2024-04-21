using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

builder
    .AddResource(new ContainerResource("seq"))
    .WithAnnotation(new EndpointAnnotation(ProtocolType.Tcp, uriScheme: "http", name: "seq", port: 5341, targetPort: 80))
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithAnnotation(new ContainerImageAnnotation { Image = "datalust/seq", Tag = "latest" });


var apiService = builder.AddProject<Projects.AspireWithSeq_ApiService>("apiservice");

builder.AddProject<Projects.AspireWithSeq_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
