using MedTrack.Services;

namespace MedTrack.Views;

public partial class SplashPage : ContentPage
{
    private readonly IAuthService _authService;

    public SplashPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        CheckAuthentication();
    }
    private async void CheckAuthentication()
    {
        await Task.Delay(2000); // Afficher le splash 2 secondes

        // Vérifier si l'utilisateur est déjà connecté
        var isAuth = await _authService.IsAuthenticatedAsync();

        if (isAuth)
        {
            await Shell.Current.GoToAsync("//AccueilPage");
        }
        else
        {
            await Shell.Current.GoToAsync("//ConnexionPage");
        }
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ConnexionPage");
    }
}