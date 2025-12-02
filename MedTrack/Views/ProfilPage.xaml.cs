using MedTrack.Helpers;
using MedTrack.Services;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Views;

public partial class ProfilPage : ContentPage
{
    private readonly IProfilService _profilService;
    private readonly IAuthService _authService;
    private Profil _profil;
    private string _selectedSexe = "";

    public ProfilPage(IProfilService profilService, IAuthService authService)
	{
		InitializeComponent();
        _profilService = profilService;
        _authService = authService;
    }
    protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadProfilAsync();
        }

        private async Task LoadProfilAsync()
        {
            try
            {
                LoadingIndicator.IsRunning = true;

                // Charger le profil depuis le backend
                _profil = await _profilService.GetProfilAsync();

                if (_profil != null)
                {
                    // Afficher les informations de base
                    NomLabel.Text = $"{_profil.Nom}";
                    //EmailLabel.Text = _profil.Email;
                    
                    // Calculer l'âge si date de naissance présente
                    //if (_profil.DateNaissance.HasValue)
                    //{
                    //    var age = CalculerAge(_profil.DateNaissance.Value);
                    //    AgeLabel.Text = $"{age} ans";
                    //}
                    //else
                    //{
                        AgeLabel.Text = "-- ans";
                    //}

                    // Groupe sanguin
                    GroupeSanguinLabel.Text = string.IsNullOrEmpty(_profil.GroupeSanguin) ? "--" : _profil.GroupeSanguin;

                    // Sexe
                    SexeLabel.Text = _profil.Sexe == "Masculin" ? "♂" : 
                                    _profil.Sexe == "Feminin" ? "♀" : "--";

                    // Allergies
                    AllergiesLayout.Children.Clear();
                    if (!string.IsNullOrEmpty(_profil.Allergies))
                    {
                        var allergies = _profil.Allergies.Split(',');
                        foreach (var allergie in allergies)
                        {
                            var label = new Label
                            {
                                Text = $"● {allergie.Trim()}",
                                FontSize = 14,
                                TextColor = Color.FromArgb("#0066FF")
                            };
                            AllergiesLayout.Children.Add(label);
                        }
                    }
                    else
                    {
                        AllergiesLayout.Children.Add(new Label 
                        { 
                            Text = "Aucune allergie renseignée",
                            FontSize = 14,
                            TextColor = Color.FromArgb("#999999")
                        });
                    }

                    // Maladies
                    MaladiesLayout.Children.Clear();
                    if (!string.IsNullOrEmpty(_profil.Maladies))
                    {
                        var maladies = _profil.Maladies.Split(',');
                        foreach (var maladie in maladies)
                        {
                            var label = new Label
                            {
                                Text = $"● {maladie.Trim()}",
                                FontSize = 14,
                                TextColor = Color.FromArgb("#0066FF")
                            };
                            MaladiesLayout.Children.Add(label);
                        }
                    }
                    else
                    {
                        MaladiesLayout.Children.Add(new Label 
                        { 
                            Text = "Aucune maladie renseignée",
                            FontSize = 14,
                            TextColor = Color.FromArgb("#999999")
                        });
                    }
                }
            }
            catch (UnauthorizedException)
            {
                await Shell.Current.GoToAsync("//ConnexionPage");
            }
            catch (ApiException ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", "Impossible de charger le profil", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
            }
        }

        private int CalculerAge(DateTime dateNaissance)
        {
            var today = DateTime.Today;
            var age = today.Year - dateNaissance.Year;
            if (dateNaissance.Date > today.AddYears(-age)) age--;
            return age;
        }

        private void OnModifierClicked(object sender, EventArgs e)
        {
            if (_profil == null) return;

            // Pré-remplir les champs du modal
            EditNomEntry.Text = _profil.Nom;
            //EditPrenomEntry.Text = _profil.Prenom;
            //EditEmailEntry.Text = _profil.Email;
            
            // Groupe sanguin
            if (!string.IsNullOrEmpty(_profil.GroupeSanguin))
            {
                var index = GroupeSanguinPicker.Items.IndexOf(_profil.GroupeSanguin);
                if (index >= 0)
                {
                    GroupeSanguinPicker.SelectedIndex = index;
                }
            }

            // Sexe
            if (_profil.Sexe == "Masculin")
            {
                SelectSexe("Masculin", MasculinBorder, MasculinLabel);
            }
            else if (_profil.Sexe == "Feminin")
            {
                SelectSexe("Feminin", FemininBorder, FemininLabel);
            }

            // Allergies
            EditAllergiesEntry.Text = _profil.Allergies;

            // Afficher le modal
            ModifyModal.IsVisible = true;
        }

        private void OnModalBackgroundClicked(object sender, EventArgs e)
        {
            ModifyModal.IsVisible = false;
        }

        private void OnCloseModalClicked(object sender, EventArgs e)
        {
            ModifyModal.IsVisible = false;
        }

        private void OnFemininClicked(object sender, EventArgs e)
        {
            SelectSexe("Feminin", FemininBorder, FemininLabel);
        }

        private void OnMasculinClicked(object sender, EventArgs e)
        {
            SelectSexe("Masculin", MasculinBorder, MasculinLabel);
        }

        private void SelectSexe(string sexe, Border border, Label label)
        {
            // Réinitialiser
            FemininBorder.BackgroundColor = Colors.White;
            FemininLabel.TextColor = Color.FromArgb("#666666");
            MasculinBorder.BackgroundColor = Colors.White;
            MasculinLabel.TextColor = Color.FromArgb("#666666");

            // Sélectionner
            _selectedSexe = sexe;
            border.BackgroundColor = Color.FromArgb("#0066FF");
            label.TextColor = Colors.White;
        }

        private async void OnEditPhotoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Photo", "Fonctionnalité à venir", "OK");
        }

        private async void OnSaveModificationClicked(object sender, EventArgs e)
        {
            if (_profil == null) return;

            // Validation
            if (string.IsNullOrWhiteSpace(EditNomEntry.Text))
            {
                await DisplayAlert("Erreur", "Veuillez entrer le nom", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditPrenomEntry.Text))
            {
                await DisplayAlert("Erreur", "Veuillez entrer le prénom", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditEmailEntry.Text))
            {
                await DisplayAlert("Erreur", "Veuillez entrer l'email", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_selectedSexe))
            {
                await DisplayAlert("Erreur", "Veuillez sélectionner le sexe", "OK");
                return;
            }

            try
            {
                ModifyLoadingIndicator.IsRunning = true;

                // Mettre à jour via le backend
                var request = new UpdateProfilRequest
                {
                    Nom = EditNomEntry.Text.Trim(),
                    //Prenom = EditPrenomEntry.Text.Trim(),
                    //Email = EditEmailEntry.Text.Trim(),
                    Sexe = _selectedSexe,
                    GroupeSanguin = GroupeSanguinPicker.SelectedItem?.ToString(),
                    Allergies = EditAllergiesEntry.Text?.Trim()
                };

                await _profilService.UpdateProfilAsync(request);

                await DisplayAlert("Succès", "Profil modifié avec succès", "OK");

                // Fermer le modal
                ModifyModal.IsVisible = false;

                // Recharger les données
                await LoadProfilAsync();
            }
            catch (ApiException ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", "Impossible de modifier le profil", "OK");
            }
            finally
            {
                ModifyLoadingIndicator.IsRunning = false;
            }
        }

        private async void OnDeconnexionClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert(
                "Déconnexion",
                "Voulez-vous vraiment vous déconnecter ?",
                "Oui",
                "Non"
            );

            if (!confirm) return;

            try
            {
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync("//ConnexionPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", "Impossible de se déconnecter", "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AccueilPage");
        }
}