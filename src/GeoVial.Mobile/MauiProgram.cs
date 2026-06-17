using GeoVial.Mobile.Servicios;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace GeoVial.Mobile;

public static class MauiProgram
{
	// Base de la API. En el emulador Android, 10.0.2.2 enruta al host de desarrollo;
	// en un dispositivo real debe apuntar a la URL del backend desplegado.
	private const string ApiBaseUrl = "http://10.0.2.2:5080/";

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddMudServices();

		// Sesión y cliente de la API (un único usuario por dispositivo → singletons).
		builder.Services.AddSingleton<EstadoSesion>();
		builder.Services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri(ApiBaseUrl) });
		builder.Services.AddSingleton<ClienteApi>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
