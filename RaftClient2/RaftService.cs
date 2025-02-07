using Raft_5._2_Class_Library;

public class RaftService
{
    HttpClient _httpClient = new();
    public string[] urls { get; set; } = [];

    public RaftService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        urls = Environment.GetEnvironmentVariable("NODE_URLS").Split(',');
    }

     public async Task<NodeData> GetDataFromApi(string url)
    {
        Console.WriteLine("Url: " + url);
        return await _httpClient.GetFromJsonAsync<NodeData>(url + "/nodeData");
    }
}