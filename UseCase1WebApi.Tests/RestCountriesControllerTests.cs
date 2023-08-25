using System.Net;
using Moq;
using Moq.Protected;
using UseCase1WebApi.Controllers;

namespace UseCase1WebApi.Tests
{
	public class RestCountriesControllerTests
	{
		private const string MOCKED_RESPONSE = "[{\"name\":{\"common\":\"Mauritius\"},\"population\":126574000},{\"name\":{\"common\":\"Spain\"},\"population\":1207040},{\"name\":{\"common\":\"Ukraine\"},\"population\":440840},{\"name\":{\"common\":\"Poland\"},\"population\":11002345},{\"name\":{\"common\":\"UnitedStates\"},\"population\":2176740},{\"name\":{\"common\":\"France\"},\"population\":543807783},{\"name\":{\"common\":\"Mexico\"},\"population\":21365456},{\"name\":{\"common\":\"Canada\"},\"population\":512365521}]";


		[Fact]
		public async Task Get_ValidatePagination_AssertCount()
		{

			var httpClient = GetHttpClient();

			var controller = new RestCountriesController(httpClient);

			var result = await controller.Get(2);

			Assert.Equal(2, result?.Count());
		}

		[Fact]
		public async Task Get_SortingAscending_ValidateNameCommon()
		{

			var httpClient = GetHttpClient();

			var controller = new RestCountriesController(httpClient);

			var result = await controller.Get(1, sortBy:"ascend");

			var firstItem =(RestCountries)result.FirstOrDefault();

			Assert.Equal("Canada", firstItem.name.common);
		}

		[Fact]
		public async Task Get_SortingDescending_ValidateNameCommon()
		{

			var httpClient = GetHttpClient();

			var controller = new RestCountriesController(httpClient);

			var result = await controller.Get(1, sortBy: "descend");

			var firstItem = (RestCountries)result.FirstOrDefault();

			Assert.Equal("UnitedStates", firstItem.name.common);
		}


		[Fact]
		public async Task Get_Filter_ValidateNameCommon()
		{

			var httpClient = GetHttpClient();

			var controller = new RestCountriesController(httpClient);

			var result = await controller.Get(1, filter: "fra");

			var firstItem = (RestCountries)result.FirstOrDefault();

			Assert.Equal("France", firstItem.name.common);
		}


		[Fact]
		public async Task Get_FilterByPopulation_ValidateNameCommon()
		{

			var httpClient = GetHttpClient();

			var controller = new RestCountriesController(httpClient);

			var result = await controller.Get(10, population: 1);

			Assert.Equal(3, result.Count());
		}


		private HttpClient GetHttpClient()
		{
			var handlerMock = new Mock<HttpMessageHandler>();
			var response = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(MOCKED_RESPONSE)
			};

			handlerMock.Protected()
				.Setup<Task<HttpResponseMessage>>(
					"SendAsync",
					ItExpr.IsAny<HttpRequestMessage>(),
					ItExpr.IsAny<CancellationToken>()
				)
				.ReturnsAsync(response);
			
			return new HttpClient(handlerMock.Object);
		}
	}
}