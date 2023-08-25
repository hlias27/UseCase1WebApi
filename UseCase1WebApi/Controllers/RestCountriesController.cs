using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

namespace UseCase1WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RestCountriesController : ControllerBase
{
	private readonly string URL = "https://restcountries.com/v3.1/all";

	private readonly HttpClient _httpClient;

	public RestCountriesController(HttpClient client)
	{
		_httpClient = client;
	}

	[HttpGet(Name = "GetRestCountries")]
	public async Task<IEnumerable<object>?> Get(int firstRecordsCount, string? filter = null, int? population = null, string? sortBy = null)
	{
		var response = await _httpClient.GetAsync(URL);
		var countriesString = await response.Content.ReadAsStringAsync();

		var countries = JsonSerializer.Deserialize<IEnumerable<RestCountries>>(countriesString);

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

		if (!string.IsNullOrWhiteSpace(sortBy))
		{
			countries = SortBy(countries, sortBy);
		}

		return countries.Take(firstRecordsCount);
	}

	private List<RestCountries> FilterByName(IEnumerable<RestCountries> countries, string filter)
	{
		var filterValue = filter.ToLower().Trim();

		return countries.Where(c => c.name.common.ToLower().Contains(filterValue)).ToList();
	}

	private List<RestCountries> FilterByPopulation(IEnumerable<RestCountries> countries, int population) =>
		countries.Where(c => c.population / 10000000 < population).ToList();

	private IEnumerable<RestCountries> SortBy(IEnumerable<RestCountries> countries, string sort) =>
		sort switch
		{
			"ascend" => countries.OrderBy(x => x.name.common),
			"descend" => countries.OrderByDescending(x => x.name.common),
			_ => countries
		};
}
