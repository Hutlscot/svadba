using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/rsvp", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    try
    {
        var form = await context.Request.ReadFormAsync();

        var payload = new
        {
            fullName = form["full_name"].ToString(),
            registrationAttendance = form["registration_attendance"].ToString(),
            celebrationAttendance = form["celebration_attendance"].ToString(),
            foodPreferences = form["food_preferences"].ToString(),
            alcoholPreferences = form["alcohol_preferences"].ToString(),
            song = form["song"].ToString(),
            comment = form["comment"].ToString()
        };

        var scriptUrl = "https://script.google.com/macros/s/AKfycbwWtjdwyAli137QU8DhLLYFDCnPkUmyzOiqIAavy8Eup48JjM7S_IRrGVA_oBC17DZr/exec";

        var client = httpClientFactory.CreateClient();

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(scriptUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem("Ошибка отправки данных в Google Sheets");
        }

        return Results.Ok(new { success = true });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Ошибка сервера: {ex.Message}");
    }
});

app.Run();