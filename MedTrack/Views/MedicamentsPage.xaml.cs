using MedTrack.Helpers;
using MedTrack.Services;
using System.Collections.ObjectModel;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Views;

public partial class MedicamentsPage : ContentPage
{
    private readonly IMedicamentsService _medicamentsService;
    private ObservableCollection<Medicament> _medicaments;
    private ObservableCollection<Medicament> _filteredMedicaments;
    private string _selectedFrequence = "";

    public MedicamentsPage(IMedicamentsService medicamentsService)
    {
        InitializeComponent();
        _medicamentsService = medicamentsService;
        _medicaments = new ObservableCollection<Medicament>();
        _filteredMedicaments = new ObservableCollection<Medicament>();
        MedicamentsCollection.ItemsSource = _filteredMedicaments;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMedicamentsAsync();
    }

    private async Task LoadMedicamentsAsync()
    {
        try
        {
            LoadingIndicator.IsRunning = true;
            EmptyStateLayout.IsVisible = false;

            // Charger depuis le backend
            var medicaments = await _medicamentsService.GetMedicamentsAsync();

            _medicaments.Clear();
            _filteredMedicaments.Clear();

            foreach (var med in medicaments)
            {
                _medicaments.Add(med);
                _filteredMedicaments.Add(med);
            }

            EmptyStateLayout.IsVisible = _medicaments.Count == 0;
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
            await DisplayAlert("Erreur", "Impossible de charger les médicaments", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? "";

        _filteredMedicaments.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var med in _medicaments)
            {
                _filteredMedicaments.Add(med);
            }
        }
        else
        {
            foreach (var med in _medicaments)
            {
                if (med.Nom.ToLower().Contains(searchText))
                {
                    _filteredMedicaments.Add(med);
                }
            }
        }

        EmptyStateLayout.IsVisible = _filteredMedicaments.Count == 0;
    }

    private async void OnMedicamentClicked(object sender, TappedEventArgs e)
    {
        try
        {
            Console.WriteLine("➡️ OnMedicamentClicked fired");
            if (e == null)
            {
                Console.WriteLine("⚠️ e is null");
                return;
            }

            // Try to get the parameter safely
            object param = null;
            try
            {
                // If your TappedEventArgs supports Parameter (some toolkits), try that first:
                var prop = e.GetType().GetProperty("Parameter");
                if (prop != null) param = prop.GetValue(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ reading Parameter via reflection failed: " + ex.Message);
            }

            // fallback: if sender is a View bound to a BindingContext
            if (param == null && sender is VisualElement ve)
            {
                param = ve.BindingContext;
                Console.WriteLine("ℹ️ fallback: BindingContext used as param");
            }

            Console.WriteLine("➡️ param type: " + (param?.GetType().FullName ?? "NULL"));

            if (param is Medicament medicament)
            {
                Console.WriteLine("➡️ Navigating with id: " + medicament.Id);
                await Shell.Current.GoToAsync($"///{nameof(DetailMedicamentPage)}?id={System.Net.WebUtility.UrlEncode(medicament.Id)}");
            }
            else
            {
                Console.WriteLine("❌ param is not a Medicament! param=" + (param?.ToString() ?? "NULL"));
                await DisplayAlert("Erreur", "Impossible d'ouvrir le détail : paramètre invalide", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Exception in OnMedicamentClicked: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }


    private void OnAddClicked(object sender, EventArgs e)
    {
        // Afficher le modal
        AddModal.IsVisible = true;

        // Réinitialiser les champs
        NomEntry.Text = "";
        DosageEntry.Text = "";
        AutreFrequenceEntry.Text = "";
        _selectedFrequence = "";
        ResetFrequenceButtons();
    }

    private void OnModalBackgroundClicked(object sender, EventArgs e)
    {
        AddModal.IsVisible = false;
    }

    private void OnCloseModalClicked(object sender, EventArgs e)
    {
        AddModal.IsVisible = false;
    }

    private void OnJournalierClicked(object sender, EventArgs e)
    {
        SelectFrequence("Journalier", JournalierBorder, JournalierLabel);
    }

    private void OnDeuxFoisClicked(object sender, EventArgs e)
    {
        SelectFrequence("2x par jour", DeuxFoisBorder, DeuxFoisLabel);
    }

    private void OnHebdomadaireClicked(object sender, EventArgs e)
    {
        SelectFrequence("Hebdomadaire", HebdomadaireBorder, HebdomadaireLabel);
    }

    private void SelectFrequence(string frequence, Border border, Label label)
    {
        ResetFrequenceButtons();

        _selectedFrequence = frequence;
        border.BackgroundColor = Color.FromArgb("#0066FF");
        label.TextColor = Colors.White;
    }

    private void ResetFrequenceButtons()
    {
        JournalierBorder.BackgroundColor = Colors.White;
        JournalierLabel.TextColor = Color.FromArgb("#666666");

        DeuxFoisBorder.BackgroundColor = Colors.White;
        DeuxFoisLabel.TextColor = Color.FromArgb("#666666");

        HebdomadaireBorder.BackgroundColor = Colors.White;
        HebdomadaireLabel.TextColor = Color.FromArgb("#666666");
    }

    private async void OnAjouterClicked(object sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(NomEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer le nom du médicament", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(DosageEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer le dosage", "OK");
            return;
        }

        // Déterminer la fréquence
        string frequence;
        if (!string.IsNullOrWhiteSpace(AutreFrequenceEntry.Text))
        {
            frequence = AutreFrequenceEntry.Text.Trim();
        }
        else if (!string.IsNullOrEmpty(_selectedFrequence))
        {
            frequence = _selectedFrequence;
        }
        else
        {
            await DisplayAlert("Erreur", "Veuillez sélectionner ou saisir une fréquence", "OK");
            return;
        }

        try
        {
            AddLoadingIndicator.IsRunning = true;

            // Créer le médicament via le backend
            var request = new CreateMedicamentRequest
            {
                Nom = NomEntry.Text.Trim(),
                Dosage = DosageEntry.Text.Trim(),
                Frequence = frequence
            };

            await _medicamentsService.CreateMedicamentAsync(request);

            await DisplayAlert("Succès", "Médicament ajouté avec succès", "OK");

            // Fermer le modal
            AddModal.IsVisible = false;

            // Recharger la liste
            await LoadMedicamentsAsync();
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible d'ajouter le médicament", "OK");
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