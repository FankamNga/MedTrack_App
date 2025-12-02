using MedTrack.Helpers;
using MedTrack.Services;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Views;

[QueryProperty(nameof(MedicamentId), "id")]
public partial class DetailMedicamentPage : ContentPage
{
    private readonly IMedicamentsService _medicamentsService;
    private string _medicamentId;
    private Medicament _medicament;
    private string _selectedFrequence = "";

    public string MedicamentId
    {
        get => _medicamentId;
        set
        {
            _medicamentId = value;
            LoadMedicamentAsync();
        }
    }

    public DetailMedicamentPage(IMedicamentsService medicamentsService)
    {
        InitializeComponent();
        _medicamentsService = medicamentsService;
    }

    private async 
    Task
LoadMedicamentAsync()
    {
        try
        {
            LoadingIndicator.IsRunning = true;

            // Charger tous les médicaments et trouver celui qui correspond
            var medicaments = await _medicamentsService.GetMedicamentsAsync();
            _medicament = medicaments.FirstOrDefault(m => m.Id == _medicamentId);

            if (_medicament != null)
            {
                // Afficher les informations
                NomLabel.Text = _medicament.Nom;
                FrequenceLabel.Text = _medicament.Frequence;
                DosageLabel.Text = _medicament.Dosage;
                SubstanceLabel.Text = _medicament.Nom.ToLower();
            }
            else
            {
                await DisplayAlert("Erreur", "Médicament introuvable", "OK");
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
            await DisplayAlert("Erreur", "Impossible de charger le médicament", "OK");
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }

    private void OnModifierClicked(object sender, EventArgs e)
    {
        if (_medicament == null) return;

        // Pré-remplir les champs du modal
        EditNomEntry.Text = _medicament.Nom;
        EditDosageEntry.Text = _medicament.Dosage;

        // Sélectionner la fréquence si elle correspond à un bouton
        if (_medicament.Frequence == "Journalier")
        {
            SelectEditFrequence("Journalier", EditJournalierBorder, EditJournalierLabel);
        }
        else if (_medicament.Frequence == "2x par jour")
        {
            SelectEditFrequence("2x par jour", EditDeuxFoisBorder, EditDeuxFoisLabel);
        }
        else if (_medicament.Frequence == "Hebdomadaire")
        {
            SelectEditFrequence("Hebdomadaire", EditHebdomadaireBorder, EditHebdomadaireLabel);
        }
        else
        {
            EditAutreFrequenceEntry.Text = _medicament.Frequence;
            ResetEditFrequenceButtons();
        }

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

    private void OnEditJournalierClicked(object sender, EventArgs e)
    {
        SelectEditFrequence("Journalier", EditJournalierBorder, EditJournalierLabel);
    }

    private void OnEditDeuxFoisClicked(object sender, EventArgs e)
    {
        SelectEditFrequence("2x par jour", EditDeuxFoisBorder, EditDeuxFoisLabel);
    }

    private void OnEditHebdomadaireClicked(object sender, EventArgs e)
    {
        SelectEditFrequence("Hebdomadaire", EditHebdomadaireBorder, EditHebdomadaireLabel);
    }

    private void SelectEditFrequence(string frequence, Border border, Label label)
    {
        ResetEditFrequenceButtons();

        _selectedFrequence = frequence;
        border.BackgroundColor = Color.FromArgb("#0066FF");
        label.TextColor = Colors.White;
    }

    private void ResetEditFrequenceButtons()
    {
        EditJournalierBorder.BackgroundColor = Colors.White;
        EditJournalierLabel.TextColor = Color.FromArgb("#666666");

        EditDeuxFoisBorder.BackgroundColor = Colors.White;
        EditDeuxFoisLabel.TextColor = Color.FromArgb("#666666");

        EditHebdomadaireBorder.BackgroundColor = Colors.White;
        EditHebdomadaireLabel.TextColor = Color.FromArgb("#666666");
    }

    private async void OnSaveModificationClicked(object sender, EventArgs e)
    {
        if (_medicament == null) return;

        // Validation
        if (string.IsNullOrWhiteSpace(EditNomEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer le nom du médicament", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(EditDosageEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer le dosage", "OK");
            return;
        }

        // Déterminer la fréquence
        string frequence;
        if (!string.IsNullOrWhiteSpace(EditAutreFrequenceEntry.Text))
        {
            frequence = EditAutreFrequenceEntry.Text.Trim();
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
            ModifyLoadingIndicator.IsRunning = true;

            // Mettre à jour via le backend
            var request = new UpdateMedicamentRequest
            {
                Nom = EditNomEntry.Text.Trim(),
                Dosage = EditDosageEntry.Text.Trim(),
                Frequence = frequence
            };

            await _medicamentsService.UpdateMedicamentAsync(_medicament.Id, request);

            await DisplayAlert("Succès", "Médicament modifié avec succès", "OK");

            // Fermer le modal
            ModifyModal.IsVisible = false;

            // Recharger les données
            await LoadMedicamentAsync();
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible de modifier le médicament", "OK");
        }
        finally
        {
            ModifyLoadingIndicator.IsRunning = false;
        }
    }

    private async void OnSupprimerClicked(object sender, EventArgs e)
    {
        if (_medicament == null) return;

        var confirm = await DisplayAlert(
            "Confirmation",
            $"Voulez-vous vraiment supprimer {_medicament.Nom} ?",
            "Oui",
            "Non"
        );

        if (!confirm) return;

        try
        {
            LoadingIndicator.IsRunning = true;

            // Supprimer via le backend
            await _medicamentsService.DeleteMedicamentAsync(_medicament.Id);

            await DisplayAlert("Succès", "Médicament supprimé", "OK");

            // Retour à la liste
            await Shell.Current.GoToAsync("//MedicamentsPage");
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible de supprimer le médicament", "OK");
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