using MedTrack.Helpers;
using MedTrack.Services;
using System.Collections.ObjectModel;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Views;

public partial class RendezVousPage : ContentPage
{
    private readonly IRendezVousService _rendezvousService;
    private ObservableCollection<RendezVous> _rendezvous;
    private ObservableCollection<RendezVous> _filteredRendezVous;

    public RendezVousPage(IRendezVousService rendezvousService)
    {
        InitializeComponent();
        _rendezvousService = rendezvousService;
        _rendezvous = new ObservableCollection<RendezVous>();
        _filteredRendezVous = new ObservableCollection<RendezVous>();
        RendezVousCollection.ItemsSource = _filteredRendezVous;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRendezVousAsync();
    }

    private async Task LoadRendezVousAsync()
    {
        try
        {
            LoadingIndicator.IsRunning = true;
            EmptyStateLayout.IsVisible = false;

            // Charger depuis le backend
            var rendezvous = await _rendezvousService.GetRendezVousAsync();

            _rendezvous.Clear();
            _filteredRendezVous.Clear();

            foreach (var rdv in rendezvous.OrderBy(r => r.Date))
            {
                _rendezvous.Add(rdv);
                _filteredRendezVous.Add(rdv);
            }

            EmptyStateLayout.IsVisible = _rendezvous.Count == 0;
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
            await DisplayAlert("Erreur", "Impossible de charger les rendez-vous", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? "";

        _filteredRendezVous.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var rdv in _rendezvous)
            {
                _filteredRendezVous.Add(rdv);
            }
        }
        else
        {
            foreach (var rdv in _rendezvous)
            {
                if (rdv.Lieu.ToLower().Contains(searchText) ||
                    rdv.Type.ToLower().Contains(searchText))
                {
                    _filteredRendezVous.Add(rdv);
                }
            }
        }

        EmptyStateLayout.IsVisible = _filteredRendezVous.Count == 0;
    }

    private async void OnRendezVousClicked(object sender, TappedEventArgs e)
    {
        if (e.Parameter is RendezVous rdv)
        {
            await Shell.Current.GoToAsync($"///{nameof(DetailRendezVousPage)}?id={rdv.Id}");
        }
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        // Afficher le modal
        AddModal.IsVisible = true;

        // Réinitialiser les champs
        DatePicker.Date = DateTime.Now;
        TimePicker.Time = DateTime.Now.TimeOfDay;
        LieuEntry.Text = "";
        TypeEntry.Text = "";
    }

    private void OnModalBackgroundClicked(object sender, EventArgs e)
    {
        AddModal.IsVisible = false;
    }

    private void OnCloseModalClicked(object sender, EventArgs e)
    {
        AddModal.IsVisible = false;
    }

    private async void OnAjouterClicked(object sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(LieuEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer le lieu", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(TypeEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer le type de rendez-vous", "OK");
            return;
        }

        try
        {
            AddLoadingIndicator.IsRunning = true;

            // Créer le rendez-vous via le backend
            var request = new CreateRendezVousRequest
            {
                Date = DatePicker.Date.ToString(),
                Heure = TimePicker.Time.ToString(),
                Lieu = LieuEntry.Text.Trim(),
                Type = TypeEntry.Text.Trim()
            };

            await _rendezvousService.CreateRendezVousAsync(request);

            await DisplayAlert("Succès", "Rendez-vous ajouté avec succès", "OK");

            // Fermer le modal
            AddModal.IsVisible = false;

            // Recharger la liste
            await LoadRendezVousAsync();
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible d'ajouter le rendez-vous", "OK");
        }
        finally
        {
            AddLoadingIndicator.IsRunning = false;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AccueilPage");
    }

    private async void OnFilterClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Filtres", "Fonctionnalité à venir", "OK");
    }
}