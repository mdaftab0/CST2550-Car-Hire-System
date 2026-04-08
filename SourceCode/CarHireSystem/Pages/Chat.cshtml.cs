using CarHireSystem.Database;
using CarHireSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace CarHireSystem.Pages;

public class ChatMessage
{
    public string Role    { get; set; } = "";
    public string Content { get; set; } = "";
}

[IgnoreAntiforgeryToken]
public class ChatModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration     _config;
    private readonly CarHireDbContext   _db;

    private const string SessionKey = "ChatHistory";

    public ChatModel(IHttpClientFactory httpClientFactory, IConfiguration config, CarHireDbContext db)
    {
        _httpClientFactory = httpClientFactory;
        _config            = config;
        _db                = db;
    }

    // Direct visit to /Chat — send home
    public IActionResult OnGet() => RedirectToPage("/Index");

    // Widget: GET /Chat?handler=Messages — returns history as JSON
    public IActionResult OnGetMessages()
    {
        var history = LoadHistory();
        return new JsonResult(history.Select(m => new { role = m.Role, content = m.Content }));
    }

    // Widget: POST /Chat?handler=Send
    public async Task<IActionResult> OnPostSendAsync([FromForm] string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
            return new JsonResult(new { reply = "" });

        var history = LoadHistory();
        history = AppendMessage(history, new ChatMessage { Role = "user", Content = userInput.Trim() });
        var reply = await CallClaudeAsync(history);
        history = AppendMessage(history, new ChatMessage { Role = "assistant", Content = reply });
        SaveHistory(history);

        return new JsonResult(new { reply });
    }

    // Widget: POST /Chat?handler=ClearHistory
    public IActionResult OnPostClearHistory()
    {
        HttpContext.Session.Remove(SessionKey);
        return new JsonResult(new { ok = true });
    }

    // ── System prompt built dynamically from live DB state ───────────────────

    private async Task<string> BuildSystemPromptAsync()
    {
        var cars = await _db.Cars.OrderBy(c => c.PricePerDay).ToArrayAsync();

        // Build fleet table with live availability
        var fleetBuilder = new StringBuilder();
        decimal minPrice = 0, maxPrice = 0;
        int availableCount = 0;

        for (int i = 0; i < cars.Length; i++)
        {
            var c = cars[i];
            var status = c.IsAvailable ? "AVAILABLE" : "unavailable";
            fleetBuilder.AppendLine(
                $"  Car #{c.Id,-3} {c.Make} {c.Model,-18} £{c.PricePerDay,6}/day  {c.Seats} seats  [{status}]");

            if (i == 0 || c.PricePerDay < minPrice) minPrice = c.PricePerDay;
            if (c.PricePerDay > maxPrice) maxPrice = c.PricePerDay;
            if (c.IsAvailable) availableCount++;
        }

        return $"""
            You are a quick-help assistant on the EasyHire car hire website (https://carhire.deancimatu.com).

            Your role is to route customers — not to resolve everything yourself.
            Answer simple questions directly (prices, availability, how the site works).
            For anything complex, account issues, disputes, or anything you cannot answer with certainty,
            direct the customer to the contact details below and stop there.

            Be brief and friendly. Use British English. Plain text only — no bullet symbols or markdown.
            Keep replies to 1–3 sentences unless showing a car list.

            ═══ LIVE FLEET ({availableCount} of {cars.Length} cars currently available) ═══
            {fleetBuilder}
            Price range: £{minPrice}/day to £{maxPrice}/day

            ═══ SITE PAGES ═══
            Search Cars   — browse and filter the fleet by price range
            Book a Car    — select a car, enter dates, pay via card
            My Bookings   — view your booking history (login required)
            Return a Car  — return an active hire (login required)

            ═══ BOOKING IN BRIEF ═══
            Search → click Book → fill in details → pay → save the Booking ID shown at the end.
            Cost = daily rate × number of days. Cars with end dates in the past are returned automatically.

            ═══ CONTACT (direct customers here for anything beyond basic questions) ═══
            Email: easyhire@fake.com
            Phone: 0123456789

            ═══ RULES ═══
            - Never invent booking IDs, registration plates, or availability you cannot see above.
            - If asked what is available, list only cars marked [AVAILABLE] from the fleet table.
            - If you cannot answer with confidence, say "For that, please contact us at easyhire@fake.com or call 0123456789."
            - Do not attempt to process bookings, access accounts, or resolve complaints — route to contact instead.
            - This is a demonstration system. No real payments are taken.
            """;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static ChatMessage[] AppendMessage(ChatMessage[] history, ChatMessage message)
    {
        var expanded = new ChatMessage[history.Length + 1];
        Array.Copy(history, expanded, history.Length);
        expanded[history.Length] = message;
        return expanded;
    }

    private ChatMessage[] LoadHistory()
    {
        var json = HttpContext.Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json))
            return Array.Empty<ChatMessage>();
        return JsonSerializer.Deserialize<ChatMessage[]>(json) ?? Array.Empty<ChatMessage>();
    }

    private void SaveHistory(ChatMessage[] history)
    {
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(history));
    }

    private async Task<string> CallClaudeAsync(ChatMessage[] history)
    {
        var apiKey = _config["Claude:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return "The AI assistant is not configured. Please contact the site administrator.";

        var systemPrompt = await BuildSystemPromptAsync();

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var messages = history.Select(m => new { role = m.Role, content = m.Content }).ToArray();

        var body = new
        {
            model      = "claude-haiku-4-5-20251001",
            max_tokens = 1024,
            system     = systemPrompt,
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

            using var doc = JsonDocument.Parse(raw);
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
