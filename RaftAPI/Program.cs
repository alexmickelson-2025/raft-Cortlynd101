using System.Text.Json;
using Raft_5._2_Class_Library;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");


var nodeId = Environment.GetEnvironmentVariable("NODE_ID") ?? throw new Exception("NODE_ID environment variable not set");
var otherNodesRaw = Environment.GetEnvironmentVariable("OTHER_NODES") ?? throw new Exception("OTHER_NODES environment variable not set");
var nodeIntervalScalarRaw = Environment.GetEnvironmentVariable("NODE_INTERVAL_SCALAR") ?? throw new Exception("NODE_INTERVAL_SCALAR environment variable not set");

builder.Services.AddLogging();
var serviceName = "Node" + nodeId;

var app = builder.Build();

var logger = app.Services.GetService<ILogger<Program>>();
// logger.LogInformation("Node ID {name}", nodeId);
// logger.LogInformation("Other nodes environment config: {}", otherNodesRaw);


// INode[] otherNodes = otherNodesRaw
//   .Split(";")
//   .Select(s => new HttpRpcOtherNode(int.Parse(s.Split(",")[0]), s.Split(",")[1]))
//   .ToArray();


// logger.LogInformation("other nodes {nodes}", JsonSerializer.Serialize(otherNodes));


// // var node = new Node(otherNodes)
// // {
// //   Id = int.Parse(nodeId),
// //   logger = app.Services.GetService<ILogger<Node>>()
// // };

INode node = new Node()
{
  Id = int.Parse(nodeId),
};
List<INode> nodes = new List<INode>();
nodes.Add(node);

Cluster cluster = new();
cluster.runCluster(nodes);

Timer? timer;
timer = new Timer(_ =>
{
    cluster.runCluster(nodes);
}, null, 0, 300);

// Node.NodeIntervalScalar = double.Parse(nodeIntervalScalarRaw);

// node.RunElectionLoop();

app.MapGet("/health", () => "healthy");

// app.MapGet("/nodeData", () =>
// {
//   return new NodeData(
//     Id: node.Id,
//     Status: node.Status,
//     ElectionTimeout: node.ElectionTimeout,
//     Term: node.CurrentTerm,
//     CurrentTermLeader: node.CurrentTermLeader,
//     CommittedEntryIndex: node.CommittedEntryIndex,
//     Log: node.Log,
//     State: node.State,
//     NodeIntervalScalar: Node.NodeIntervalScalar
//   );
// });

// app.MapPost("/request/appendEntries", async (AppendEntriesData request) =>
// {
//   logger.LogInformation("received append entries request {request}", request);
//   await node.RequestAppendEntries(request);
// });

// app.MapPost("/request/vote", async (VoteRequestData request) =>
// {
//   logger.LogInformation("received vote request {request}", request);
//   await node.RequestVote(request);
// });

// app.MapPost("/response/appendEntries", async (RespondEntriesData response) =>
// {
//   logger.LogInformation("received append entries response {response}", response);
//   await node.RespondAppendEntries(response);
// });

// app.MapPost("/response/vote", async (VoteResponseData response) =>
// {
//   logger.LogInformation("received vote response {response}", response);
//   await node.ResponseVote(response);
// });

// app.MapPost("/request/command", async (ClientCommandData data) =>
// {
//   await node.SendCommand(data);
// });

app.Run();

// using System;

// class Program
// {
//     static void Main()
//     {
//         Console.WriteLine("Hello, World!");
//     }
// }