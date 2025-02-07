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

// var logger = app.Services.GetService<ILogger<Program>>();
// logger?.LogInformation("Node ID {name}", nodeId);
// logger?.LogInformation("Other nodes environment config: {}", otherNodesRaw);

INode[] otherNodes = otherNodesRaw
  .Split(";")
  .Select(s => new Node(int.Parse(s.Split(",")[0]), s.Split(",")[1]))
  .ToArray();

// logger?.LogInformation("other nodes {nodes}", JsonSerializer.Serialize(otherNodes));

INode node = new Node()
{
  Id = int.Parse(nodeId),
  forcedOutcome = true,
};

List<INode> nodes = new List<INode>();
nodes.Add(node);
otherNodes[0].forcedOutcome = true;
otherNodes[1].forcedOutcome = true;
nodes.Add(otherNodes[0]);
nodes.Add(otherNodes[1]);

Cluster cluster = new();
cluster.runCluster(nodes);

Timer? timer;
timer = new Timer(_ =>
{
    cluster.runCluster(nodes);
}, null, 0, 200);

app.MapGet("/health", () => "healthy");

app.MapGet("/nodeData", () =>
{
  return new NodeData(
    Id: node.Id,
    Status: node.serverType,
    //ElectionTimeout: node.electionTimeout.ToString(),
    Term: node.term,
    CurrentTermLeader: node.leaderId,
    CommittedEntryIndex: node.committedIndex,
    Log: node.log,
    State: node.responsive
  );
});

// var node = new Node(otherNodes)
// {
//   Id = int.Parse(nodeId),
//   logger = app.Services.GetService<ILogger<Node>>()
// };

// Node.NodeIntervalScalar = double.Parse(nodeIntervalScalarRaw);
// node.RunElectionLoop();

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