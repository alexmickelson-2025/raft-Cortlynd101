using System.Text.Json;
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
        try
        {
            var response = await _httpClient.GetAsync(url + "/nodeData");
            response.EnsureSuccessStatusCode(); // Throws an error if response is not 200 OK

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response from {url}: {jsonString}");

            return JsonSerializer.Deserialize<NodeData>(jsonString);
            // return await _httpClient.GetFromJsonAsync<NodeData>(url + "/nodeData");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Error fetching data: {ex.Message}");
        }
    }
}