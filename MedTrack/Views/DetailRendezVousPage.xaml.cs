using MedTrack.Helpers;
using MedTrack.Services;
using System.Globalization;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Views;

[QueryProperty(nameof(RendezVousId), "id")]
public partial class DetailRendezVousPage : ContentPage
{
    private readonly IRendezVousService _rendezvousService;
    private string _rendezvousId;
    private RendezVous _rendezvous;

    public string RendezVousId
    {
        get => _rendezvousId;
        set
        {
            _rendezvousId = value;
            LoadRendezVousAsync();
        }
    }

    public DetailRendezVousPage(IRendezVousService rendezvousService)
    {
        InitializeComponent();
        _rendezvousService = rendezvousService;
    }

    private async 
    Task
LoadRendezVousAsync()
    {
        try
        {
            LoadingIndicator.IsRunning = true;

            // Charger le rendez-vous
            var rendezvousList = await _rendezvousService.GetRendezVousAsync();
            _rendezvous = rendezvousList.FirstOrDefault(r => r.Id == _rendezvousId);

            if (_rendezvous != null)
            {
                // Afficher les informations
                DateLabel.Text = _rendezvous.Date.ToString("dd MMMM yyyy", new CultureInfo("fr-FR"));
                HeureLabel.Text = _rendezvous.Heure;
                LieuLabel.Text = _rendezvous.Lieu;
                TypeLabel.Text = _rendezvous.Type;

                // Notes (optionnel, peut être ajouté au modèle si nécessaire)
                NotesLabel.Text = "aucune autre note...";
            }
            else
            {
                await DisplayAlert("Erreur", "Rendez-vous introuvable", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (UnauthorizedException)
        {
            await Shell.Current.GoToAsync("//ConnexionPage");
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible de charger le rendez-vous", "OK");
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }

    private void OnModifierClicked(object sender, EventArgs e)
    {
        if (_rendezvous == null) return;

        // Pré-remplir les champs du modal
        EditDatePicker.Date = _rendezvous.Date;

        // Parser l'heure (format "HH:mm")
        if (TimeSpan.TryParse(_rendezvous.Heure, out TimeSpan heure))
        {
            EditTimePicker.Time = heure;
        }

        EditLieuEntry.Text = _rendezvous.Lieu;
        EditTypeEntry.Text = _rendezvous.Type;

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

    private async void OnSaveModificationClicked(object sender, EventArgs e)
    {
        if (_rendezvous == null) return;

        // Validation
        if (string.IsNullOrWhiteSpace(EditLieuEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer le lieu", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(EditTypeEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer le type de rendez-vous", "OK");
            return;
        }

        try
        {
            ModifyLoadingIndicator.IsRunning = true;

            // Mettre à jour via le backend
            var request = new UpdateRendezVousRequest
            {
                Date = EditDatePicker.Date.ToString(),
                Heure = EditTimePicker.Time.ToString(),
                Lieu = EditLieuEntry.Text.Trim(),
                Type = EditTypeEntry.Text.Trim()
            };

            await _rendezvousService.UpdateRendezVousAsync(_rendezvous.Id, request);

            await DisplayAlert("Succès", "Rendez-vous modifié avec succès", "OK");

            // Fermer le modal
            ModifyModal.IsVisible = false;

            // Recharger les données
            await LoadRendezVousAsync();
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Erreur", "Impossible de modifier le rendez-vous", "OK");
        }
        finally
        {
            ModifyLoadingIndicator.IsRunning = false;
        }
    }

    private async void OnSupprimerClicked(object sender, EventArgs e)
    {
        if (_rendezvous == null) return;

        var confirm = await DisplayAlert(
            "Confirmation",
            $"Voulez-vous vraiment supprimer ce rendez-vous du {_rendezvous.Date:dd/MM/yyyy} ?",
            "Oui",
            "Non"
        );

        if (!confirm) return;

        try
        {
            LoadingIndicator.IsRunning = true;

            // Supprimer via le backend
            await _rendezvousService.DeleteRendezVousAsync(_rendezvous.Id);

            await DisplayAlert("Succès", "Rendez-vous supprimé", "OK");

            // Retour à la liste
            await Shell.Current.GoToAsync("//RendezVousPage");
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible de supprimer le rendez-vous", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}