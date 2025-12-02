using MedTrack.Helpers;
using MedTrack.Services;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Views;

public partial class InscriptionPage : ContentPage
{
    private readonly IAuthService _authService;
    private readonly IProfilService _profilService;
    private bool _isPasswordVisible = false;

    public InscriptionPage(IAuthService authService, IProfilService profilService)
    {
        InitializeComponent();
        _authService = authService;
        _profilService = profilService;
    }

    private async void OnSignupClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"=== POST REQUEST ===");
        // Validation
        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer votre email", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(NomEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer votre nom", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer un mot de passe", "OK");
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Erreur", "Les mots de passe ne correspondent pas", "OK");
            return;
        }

        if (PasswordEntry.Text.Length < 6)
        {
            await DisplayAlert("Erreur", "Le mot de passe doit contenir au moins 6 caractères", "OK");
            return;
        }

        try
        {
            LoadingIndicator.IsRunning = true;
            System.Diagnostics.Debug.WriteLine($"=== POST REQUEST ===");
            // Inscription via le service backend
            var request = new SignupRequest
            {
                Email = EmailEntry.Text.Trim(),
                Password = PasswordEntry.Text
            };
            
            var response = await _authService.SignupAsync(request);

            // Créer/Mettre à jour le profil avec le nom
            try
            {
                await _profilService.UpdateProfilAsync(new UpdateProfilRequest
                {
                    Nom = NomEntry.Text.Trim()
                });
            }
            catch
            {
                // Ignorer si le profil n'existe pas encore
            }

            await DisplayAlert("Succès", "Votre compte a été créé avec succès !", "OK");

            // Rediriger vers l'accueil
            await Shell.Current.GoToAsync("//AccueilPage");
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == 400)
            {
                await DisplayAlert("Erreur", ex.ToString(), "OK2");
            }
            else
            {
                await DisplayAlert("Erreur", ex.ToString(), "OKi");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.ToString(), "OK1");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ConnexionPage");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void OnPasswordToggleClicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordEntry.IsPassword = !_isPasswordVisible;
    }

    // Validation du nom en temps réel
    private void OnNomChanged(object sender, TextChangedEventArgs e)
    {
        NomValidIcon.IsVisible = !string.IsNullOrWhiteSpace(e.NewTextValue);
    }
}
