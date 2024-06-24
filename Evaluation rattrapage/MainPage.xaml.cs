using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace PrenomsApp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public object AnneePicker { get; }

        public MainPage()
        {
            InitializeComponent();

            // Ajouter les années dans le Picker
            for (int year = 2003; year <= DateTime.Now.Year; year++)
            {
                AnneePicker.Items.Add(year.ToString());
            }
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private async void OnRechercherClicked(object sender, EventArgs e)
        {
            string prenom = PrenomEntry.Text;
            string annee = AnneePicker.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(prenom) || string.IsNullOrEmpty(annee))
            {
                await DisplayAlert("Erreur", "Veuillez entrer un prénom et sélectionner une année.", "OK");
                return;
            }

            int nombre = await GetNombreEnfantsAsync(prenom, annee);
            ResultatLabel.Text = $"{nombre} enfant(s) nommé(s) {prenom} né(s) à Nantes en {annee}.";
        }

        private async Task<int> GetNombreEnfantsAsync(string prenom, string annee)
        {
            string url = $"https://data.nantesmetropole.fr/api/records/1.0/search/?dataset=244400404_prenoms-enfants-nes-nantes&q={prenom}&refine.annee={annee}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Root>(jsonResponse);

                return data.Records.Count;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
                return 0;
            }
        }
    }

    public class Root
    {
        public List<Record> Records { get; set; }
    }

    public class Record
    {
        // Définir les propriétés nécessaires
    }
}
