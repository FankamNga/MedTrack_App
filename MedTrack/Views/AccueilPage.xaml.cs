using MedTrack.Helpers;
using MedTrack.Services;

namespace MedTrack.Views;

public partial class AccueilPage : ContentPage
{
    private readonly IProfilService _profilService;
    private readonly IAuthService _authService;

    public AccueilPage(IProfilService profilService, IAuthService authService)
    {
        InitializeComponent();
        _profilService = profilService;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserProfileAsync();
    }

    private async Task LoadUserProfileAsync()
    {
        try
        {
            // Charger le profil depuis le backend
            var profil = await _profilService.GetProfilAsync();

            // Afficher le nom de l'utilisateur
            if (!string.IsNullOrEmpty(profil.Nom))
            {
                UserNameLabel.Text = profil.Nom;
            }
        }
        catch (UnauthorizedException)
        {
            // Session expirée, rediriger vers login
            await Shell.Current.GoToAsync("//ConnexionPage");
        }
        catch (Exception ex)
        {
            // Erreur silencieuse, garder le nom par défaut
            System.Diagnostics.Debug.WriteLine($"Erreur chargement profil: {ex.Message}");
        }
    }

    private async void OnMedicamentsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MedicamentsPage");
    }

    private async void OnRendezVousClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//RendezVousPage");
    }

    private async void OnDocumentsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//DocumentsPage");
    }

    private async void OnProfilClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProfilPage");
    }

    private async void OnVoirToutActualitesClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ActualitesPage");
    }

    private async void OnVoirActualiteClicked(object sender, EventArgs e)
    {
        // TODO: Naviguer vers le détail de l'actualité
        await DisplayAlert("Actualité", "Zoom sur les médicaments importants...", "OK");
    }
}