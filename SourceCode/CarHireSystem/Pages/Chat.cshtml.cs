using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CarHireSystem.Pages;

public class ChatMessage
{
    public string Role    { get; set; } = "";
    public string Content { get; set; } = "";
}

public class ChatModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration     _config;

    private const string SessionKey = "ChatHistory";

    private const string SystemPrompt = """
        You are an AI assistant for EasyHire Car Hire, a car rental service based in the UK.
        Your role is to help customers find the right car, understand pricing, and navigate the booking process.
        Be concise, friendly, and helpful. Do not process real payments or store personal data.

        Available fleet:
        - Car 1:  Toyota Corolla     — £35/day  — 5 seats
        - Car 2:  BMW X5             — £95/day  — 5 seats
        - Car 3:  Ford Fiesta        — £28/day  — 5 seats
        - Car 4:  Mercedes C-Class   — £120/day — 5 seats
        - Car 5:  Vauxhall Astra     — £45/day  — 5 seats
        - Car 6:  Porsche Taycan     — £75/day  — 5 seats
        - Car 7:  Ford Raptor        — £45/day  — 5 seats
        - Car 8:  Land Rover Defender — £150/day — 7 seats
        - Car 9:  Volkswagen Golf    — £20/day  — 5 seats
        - Car 10: MiniCooper Countryman — £30/day — 5 seats

        How to search: Visit the Search Cars page and filter by price range.
        How to book: Find a car on the Search page, note the Car ID, then go to the Booking page, enter your details and the Car ID.
        How to return: Go to the Returns page and enter your Booking ID.
        Contact: easyhire@fake.com | 0123456789

        Important: This is a demonstration website — no real bookings, payments or transactions are processed.
        When asked about cars under a certain price, list the matching cars with their IDs and prices, and suggest visiting the Search page.
        """;

    public List<ChatMessage> Messages { get; private set; } = new();

    [BindProperty]
    public string UserInput { get; set; } = "";

    public ChatModel(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config            = config;
    }

    public void OnGet()
    {
        Messages = LoadHistory();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(UserInput))
            return RedirectToPage();

        var history = LoadHistory();
        history.Add(new ChatMessage { Role = "user", Content = UserInput.Trim() });

        var reply = await CallClaudeAsync(history);
        history.Add(new ChatMessage { Role = "assistant", Content = reply });

        SaveHistory(history);
        return RedirectToPage();
    }

    public IActionResult OnPostClear()
    {
        HttpContext.Session.Remove(SessionKey);
        return RedirectToPage();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private List<ChatMessage> LoadHistory()
    {
        var json = HttpContext.Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json))
            return new List<ChatMessage>();
        return JsonSerializer.Deserialize<List<ChatMessage>>(json) ?? new List<ChatMessage>();
    }

    private void SaveHistory(List<ChatMessage> history)
    {
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(history));
    }

    private async Task<string> CallClaudeAsync(List<ChatMessage> history)
    {
        var apiKey = _config["Claude:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return "The AI assistant is not configured. Please add the Claude API key to the application settings.";

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var messages = history.Select(m => new { role = m.Role, content = m.Content }).ToArray();

        var body = new
        {
            model      = "claude-haiku-4-5-20251001",
            max_tokens = 512,
            system     = SystemPrompt,
            messages
        };

        var json    = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync("https://api.anthropic.com/v1/messages", content);
            var raw      = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"Sorry, I couldn't get a response right now. (Error {(int)response.StatusCode})";

            using var doc  = JsonDocument.Parse(raw);
            var text = doc.RootElement
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "Sorry, I received an empty response.";
        }
        catch
        {
            return "Sorry, something went wrong connecting to the AI service. Please try again.";
        }
    }
}
