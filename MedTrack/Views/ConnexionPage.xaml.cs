using MedTrack.Helpers;
using MedTrack.Services;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Views;

public partial class ConnexionPage : ContentPage
{
    private readonly IAuthService _authService;
    private bool _isPasswordVisible = false;

    public ConnexionPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer votre email", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer votre mot de passe", "OK");
            return;
        }

        try
        {
            LoadingIndicator.IsRunning = true;

            // Connexion via le service backend
            var request = new LoginRequest
            {
                Email = EmailEntry.Text.Trim(),
                Password = PasswordEntry.Text
            };

            var response = await _authService.LoginAsync(request);

            // Connexion réussie
            await Shell.Current.GoToAsync("//AccueilPage");
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == 401)
            {
                await DisplayAlert("Erreur", "Email ou mot de passe incorrect", "OK");
            }
            else
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnSignupClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//InscriptionPage");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Mot de passe oublié", "Cette fonctionnalité sera bientôt disponible", "OK");
        // TODO: Implémenter la page de récupération de mot de passe
    }

    private void OnPasswordToggleClicked(object sender, EventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordEntry.IsPassword = !_isPasswordVisible;
    }
}