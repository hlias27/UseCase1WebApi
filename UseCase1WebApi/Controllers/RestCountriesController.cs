using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

namespace UseCase1WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RestCountriesController : ControllerBase
{
	private readonly string URL = "https://restcountries.com/v3.1/all";

	public RestCountriesController()
	{
	}

	[HttpGet(Name = "GetRestCountries")]
	public async Task<IEnumerable<object>?> Get(string? param1 = null, int? param2 = null, string? param3 = null)
	{
		using var httpClient = new HttpClient();

		var response = await httpClient.GetAsync(URL);
		var countriesString = await response.Content.ReadAsStringAsync();

		return JsonSerializer.Deserialize<List<object>>(countriesString);
	}
}
