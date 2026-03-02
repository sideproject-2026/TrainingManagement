var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TrainingManagement_WebAPI>("trainingmanagement-webapi");

builder.Build().Run();
