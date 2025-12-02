using MedTrack.Services;
using MedTrack.Views;
using Microsoft.Extensions.Logging;

namespace MedTrack
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            // Enregistrement des services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IProfilService, ProfilService>();
            builder.Services.AddSingleton<IDocumentsService, DocumentsService>();
            builder.Services.AddSingleton<IMedicamentsService, MedicamentsService>();
            builder.Services.AddSingleton<IRendezVousService, RendezVousService>();

            // Enregistrement des pages
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<ConnexionPage>();
            builder.Services.AddTransient<InscriptionPage>();
            builder.Services.AddTransient<AccueilPage>();
            builder.Services.AddTransient<MedicamentsPage>();
            builder.Services.AddTransient<DetailMedicamentPage>();
            builder.Services.AddTransient<RendezVousPage>();
            builder.Services.AddTransient<DetailRendezVousPage>();
            builder.Services.AddTransient<ProfilPage>();
            
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
