using System.Text.Json;
using Raft_5._2_Class_Library;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");
builder.Services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);

var nodeId = Environment.GetEnvironmentVariable("NODE_ID") ?? throw new Exception("NODE_ID environment variable not set");
var otherNodesRaw = Environment.GetEnvironmentVariable("OTHER_NODES") ?? throw new Exception("OTHER_NODES environment variable not set");
var nodeIntervalScalarRaw = Environment.GetEnvironmentVariable("NODE_INTERVAL_SCALAR") ?? throw new Exception("NODE_INTERVAL_SCALAR environment variable not set");

INode[] otherNodes = otherNodesRaw
  .Split(";")
  .Select(s => new Node(int.Parse(s.Split(",")[0]), s.Split(",")[1]))
  .ToArray();

INode node = new Node()
{
  Id = int.Parse(nodeId),
};

List<INode> nodes = new List<INode>{ node };
if (otherNodes.Length >= 2)
{
    nodes.AddRange(otherNodes);
}
builder.Services.AddLogging();
builder.Services.AddSingleton<INode>(node);

var serviceName = "Node" + nodeId;

var app = builder.Build();

app.MapGet("/nodeData", () =>
{
  Console.WriteLine($"Fetching NodeData - Id: {node.Id}, Term: {node.term}, Status: {node.serverType}");
  return new NodeData(
    Id: node.Id,
    Status: node.serverType,
    ElectionTimeout: node.electionTimeout,
    Term: node.term,
    CurrentTermLeader: node.leaderId,
    CommittedEntryIndex: node.committedIndex,
    Log: node.log,
    State: node.serverType,
    Responsive: node.responsive
  );
});

Cluster cluster = new();
Timer? timer;
timer = new Timer(_ =>
{
  Console.WriteLine($"Node election timeout for node {node.Id}: " + node.electionTimeout);
  cluster.runCluster(nodes);
}, null, 0, 200);

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