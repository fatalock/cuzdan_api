
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Cuzdan.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cuzdan.Tests.Integration.Api;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    // Test sunucusunu ve HTTP istemcisini ayarla
    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WhenEmailIsInvalid_ShouldReturnBadRequestWithValidationError()
    {
        // ARRANGE (Hazırlık)
        // Geçersiz bir e-posta adresi içeren bir kayıt isteği oluştur
        var registerDto = new RegisterUserDto
        {
            Name = "Test",
            Email = "gecersiz-eposta",
            Password = "ValidPassword123!",
            ConfirmPassword = "ValidPassword123!"
        };

        // ACT (Eylem)
        // API'nin /api/Auth/register endpoint'ine POST isteği gönder
        var response = await _httpClient.PostAsJsonAsync("/api/Auth/register", registerDto);

        // ASSERT (Doğrulama)
        // 1. Yanıtın durum kodunun 400 Bad Request olduğunu doğrula
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // 2. Yanıtın içeriğini oku ve bir ProblemDetails nesnesine dönüştür
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(responseContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // 3. ProblemDetails nesnesinin boş olmadığını doğrula
        problemDetails.Should().NotBeNull();

        // 4. Hatalar (errors) listesinde "Email" anahtarının olduğunu doğrula
        problemDetails!.Errors.Should().ContainKey("Email");

        // 5. "Email" hataları arasında beklediğimiz validasyon mesajının olduğunu doğrula
        // Not: Bu metin, RegisterUserDtoValidator'daki mesaj ile tam olarak eşleşmelidir.
        problemDetails.Errors["Email"].Should().Contain("A valid email address is required."); 
    }
}