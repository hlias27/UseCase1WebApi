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
	public async Task<IEnumerable<object>?> Get(string? filter = null, int? population = null, string? param3 = null)
	{
		using var httpClient = new HttpClient();
		var response = await httpClient.GetAsync(URL);
		var countriesString = await response.Content.ReadAsStringAsync();

		var countries = JsonSerializer.Deserialize<List<RestCountries>>(countriesString);

		if (countries == null)
		{
			return null;
		}

		if (!string.IsNullOrWhiteSpace(filter))
		{
			countries = FilterByName(countries, filter);
		}

		if (population.HasValue)
		{
			countries = FilterByPopulation(countries, population.Value);
		}


		return countries;
	}

	private List<RestCountries> FilterByName(List<RestCountries> countries, string filter)
	{
		var filterValue = filter.ToLower().Trim();
			
		return countries.Where(c => c.name.common.ToLower().Contains(filterValue)).ToList();
	}

	private List<RestCountries> FilterByPopulation(List<RestCountries> countries, int population) => 
		countries.Where(c => c.population/10000000 < population).ToList();
}
