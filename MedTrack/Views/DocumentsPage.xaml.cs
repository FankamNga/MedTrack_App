using MedTrack.Helpers;
using MedTrack.Services;
using System.Collections.ObjectModel;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Views;

public partial class DocumentsPage : ContentPage
{
    private readonly IDocumentsService _documentsService;
    private ObservableCollection<Document> _documents;
    private ObservableCollection<Document> _filteredDocuments;
    private string _selectedAddType = "";
    private string _selectedEditType = "";
    private FileResult _selectedAddFile;
    private FileResult _selectedEditFile;
    private Document _currentEditDocument;

    public DocumentsPage(IDocumentsService documentsService)
    {
        InitializeComponent();
        _documentsService = documentsService;
        _documents = new ObservableCollection<Document>();
        _filteredDocuments = new ObservableCollection<Document>();
        DocumentsCollection.ItemsSource = _filteredDocuments;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDocumentsAsync();
    }

    private async Task LoadDocumentsAsync()
    {
        try
        {
            LoadingIndicator.IsRunning = true;
            EmptyStateLayout.IsVisible = false;

            // Charger depuis le backend
            var documents = await _documentsService.GetDocumentsAsync();

            _documents.Clear();
            _filteredDocuments.Clear();

            foreach (var doc in documents.OrderByDescending(d => d.CreatedAt))
            {
                _documents.Add(doc);
                _filteredDocuments.Add(doc);
            }

            EmptyStateLayout.IsVisible = _documents.Count == 0;
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
            await DisplayAlert("Erreur", "Impossible de charger les documents", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? "";

        _filteredDocuments.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var doc in _documents)
            {
                _filteredDocuments.Add(doc);
            }
        }
        else
        {
            foreach (var doc in _documents)
            {
                if (doc.Nom.ToLower().Contains(searchText) ||
                    doc.TypeDocument.ToLower().Contains(searchText))
                {
                    _filteredDocuments.Add(doc);
                }
            }
        }

        EmptyStateLayout.IsVisible = _filteredDocuments.Count == 0;
    }

    // ===== MODAL AJOUT =====

    private void OnAddClicked(object sender, EventArgs e)
    {
        // Réinitialiser
        _selectedAddFile = null;
        AddNomEntry.Text = "";
        AddFileNameLabel.Text = "télécharger un fichier";
        _selectedAddType = "";
        ResetAddTypeButtons();

        // Afficher le modal
        AddModal.IsVisible = true;
    }

    private async void OnPickFileClicked(object sender, EventArgs e)
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Sélectionner un document",
                FileTypes = FilePickerFileType.Pdf
            };

            var result = await FilePicker.PickAsync(options);
            if (result != null)
            {
                _selectedAddFile = result;
                AddFileNameLabel.Text = result.FileName;

                // Pré-remplir le nom si vide
                if (string.IsNullOrEmpty(AddNomEntry.Text))
                {
                    AddNomEntry.Text = Path.GetFileNameWithoutExtension(result.FileName);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible de sélectionner le fichier", "OK");
        }
    }

    private void OnAddOrdonnanceClicked(object sender, EventArgs e)
    {
        SelectAddType("Ordonnance", AddOrdonnanceBorder, AddOrdonnanceLabel);
    }

    private void OnAddAnalyseClicked(object sender, EventArgs e)
    {
        SelectAddType("Analyse", AddAnalyseBorder, AddAnalyseLabel);
    }

    private void OnAddReproClicked(object sender, EventArgs e)
    {
        SelectAddType("Repro", AddReproBorder, AddReproLabel);
    }

    private void SelectAddType(string type, Border border, Label label)
    {
        ResetAddTypeButtons();

        _selectedAddType = type;
        border.BackgroundColor = Color.FromArgb("#CCFF66");
        label.TextColor = Colors.White;
    }

    private void ResetAddTypeButtons()
    {
        AddOrdonnanceBorder.BackgroundColor = Colors.White;
        AddOrdonnanceLabel.TextColor = Color.FromArgb("#666666");

        AddAnalyseBorder.BackgroundColor = Colors.White;
        AddAnalyseLabel.TextColor = Color.FromArgb("#666666");

        AddReproBorder.BackgroundColor = Colors.White;
        AddReproLabel.TextColor = Color.FromArgb("#666666");
    }

    private async void OnAjouterClicked(object sender, EventArgs e)
    {
        // Validation
        if (_selectedAddFile == null)
        {
            await DisplayAlert("Erreur", "Veuillez sélectionner un fichier", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(AddNomEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer le nom du fichier", "OK");
            return;
        }
        if (string.IsNullOrEmpty(_selectedAddType))
        {
            await DisplayAlert("Erreur", "Veuillez sélectionner le type de document", "OK");
            return;
        }

        try
        {
            AddLoadingIndicator.IsRunning = true;

            // Lire le fichier
            using var stream = await _selectedAddFile.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Uploader via le backend avec le type de document
            var document = await _documentsService.UploadDocumentAsync(
                fileBytes,
                AddNomEntry.Text.Trim() + Path.GetExtension(_selectedAddFile.FileName),
                _selectedAddFile.ContentType ?? "application/pdf",
                _selectedAddType  // ← Passer le type de document sélectionné
            );

            await DisplayAlert("Succès", "Document ajouté avec succès", "OK");

            // Fermer le modal
            AddModal.IsVisible = false;

            // Recharger la liste
            await LoadDocumentsAsync();
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible d'ajouter le document: {ex.Message}", "OK");
        }
        finally
        {
            AddLoadingIndicator.IsRunning = false;
        }
    }
    private void OnCloseAddModalClicked(object sender, EventArgs e)
    {
        AddModal.IsVisible = false;
    }

    // ===== MODAL MODIFICATION =====

    private void OnEditDocumentClicked(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Document document)
        {
            _currentEditDocument = document;
            _selectedEditFile = null;

            // Pré-remplir les champs
            EditNomEntry.Text = document.Nom;
            EditFileNameLabel.Text = document.Nom;

            // Sélectionner le type
            if (document.TypeDocument == "Ordonnance")
            {
                SelectEditType("Ordonnance", EditOrdonnanceBorder, EditOrdonnanceLabel);
            }
            else if (document.TypeDocument == "Analyse")
            {
                SelectEditType("Analyse", EditAnalyseBorder, EditAnalyseLabel);
            }
            else if (document.TypeDocument == "Repro")
            {
                SelectEditType("Repro", EditReproBorder, EditReproLabel);
            }
            else
            {
                ResetEditTypeButtons();
            }

            // Afficher le modal
            EditModal.IsVisible = true;
        }
    }

    private async void OnPickEditFileClicked(object sender, EventArgs e)
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Sélectionner un document",
                FileTypes = FilePickerFileType.Pdf
            };

            var result = await FilePicker.PickAsync(options);
            if (result != null)
            {
                _selectedEditFile = result;
                EditFileNameLabel.Text = result.FileName;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible de sélectionner le fichier", "OK");
        }
    }

    private void OnEditOrdonnanceClicked(object sender, EventArgs e)
    {
        SelectEditType("Ordonnance", EditOrdonnanceBorder, EditOrdonnanceLabel);
    }

    private void OnEditAnalyseClicked(object sender, EventArgs e)
    {
        SelectEditType("Analyse", EditAnalyseBorder, EditAnalyseLabel);
    }

    private void OnEditReproClicked(object sender, EventArgs e)
    {
        SelectEditType("Repro", EditReproBorder, EditReproLabel);
    }

    private void SelectEditType(string type, Border border, Label label)
    {
        ResetEditTypeButtons();

        _selectedEditType = type;
        border.BackgroundColor = Color.FromArgb("#CCFF66");
        label.TextColor = Colors.White;
    }

    private void ResetEditTypeButtons()
    {
        EditOrdonnanceBorder.BackgroundColor = Colors.White;
        EditOrdonnanceLabel.TextColor = Color.FromArgb("#666666");

        EditAnalyseBorder.BackgroundColor = Colors.White;
        EditAnalyseLabel.TextColor = Color.FromArgb("#666666");

        EditReproBorder.BackgroundColor = Colors.White;
        EditReproLabel.TextColor = Color.FromArgb("#666666");
    }

    private async void OnModifierClicked(object sender, EventArgs e)
    {
        if (_currentEditDocument == null) return;

        // Note: L'API backend ne supporte pas la modification
        // On supprime l'ancien et on reupload le nouveau
        await DisplayAlert("Info", "La modification de documents n'est pas encore supportée par le backend", "OK");
    }

    private async void OnSupprimerClicked(object sender, EventArgs e)
    {
        if (_currentEditDocument == null) return;

        var confirm = await DisplayAlert(
            "Confirmation",
            $"Voulez-vous vraiment supprimer {_currentEditDocument.Nom} ?",
            "Oui",
            "Non"
        );

        if (!confirm) return;

        try
        {
            EditLoadingIndicator.IsRunning = true;

            // Supprimer via le backend
            await _documentsService.DeleteDocumentAsync(_currentEditDocument.Id);

            await DisplayAlert("Succès", "Document supprimé", "OK");

            // Fermer le modal
            EditModal.IsVisible = false;

            // Recharger la liste
            await LoadDocumentsAsync();
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Impossible de supprimer le document", "OK");
        }
        finally
        {
            EditLoadingIndicator.IsRunning = false;
        }
    }

    private void OnCloseEditModalClicked(object sender, EventArgs e)
    {
        EditModal.IsVisible = false;
    }

    // ===== AUTRES =====

    private void OnModalBackgroundClicked(object sender, EventArgs e)
    {
        AddModal.IsVisible = false;
        EditModal.IsVisible = false;
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