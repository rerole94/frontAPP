using MongoDB.Driver;
using System.Collections.Generic;
using System.Windows;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using System.Threading.Tasks;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private List<Plat> plats;
        private List<Plat> panier;
        private decimal total;

        public MainWindow()
        {
            InitializeComponent();
            plats = new List<Plat>();
            panier = new List<Plat>();
            total = 0;

            LoadPlatsFromApi(); // Charge les plats à partir de l'API
        }

        private async Task<List<Plat>> GetPlatsFromApi()
        {
            var plats = new List<Plat>();

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://localhost:7267/api/Books");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var allPlats = JsonSerializer.Deserialize<List<Plat>>(jsonString);

                        // Seuls les noms et les prix sont nécessaires
                        foreach (var plat in allPlats)
                        {
                            plats.Add(new Plat { Nom = plat.Nom, Price = plat.Price });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite lors de la récupération des données : " + ex.Message);
            }

            return plats;
        }

        private async void LoadPlatsFromApi()
        {
            var platsFromApi = await GetPlatsFromApi();

            foreach (var plat in platsFromApi)
            {
                listBoxPlats.Items.Add(plat); // Ajoute les plats à la liste des plats
            }
        }

        private void buttonAjouter_Click(object sender, RoutedEventArgs e)
        {
            Plat platSelectionne = (Plat)listBoxPlats.SelectedItem;

            if (platSelectionne != null)
            {
                panier.Add(platSelectionne); // Ajoute le plat sélectionné au panier
                listBoxPanier.Items.Add(platSelectionne); // Ajoute le plat à la liste du panier
                total += platSelectionne.Price; // Ajoute le prix du plat au total
                labelTotal.Text = "Total : " + total.ToString("C"); // Met à jour l'affichage du total
            }
        }

        private void buttonSupprimer_Click(object sender, RoutedEventArgs e)
        {
            Plat platSelectionne = (Plat)listBoxPanier.SelectedItem;

            if (platSelectionne != null)
            {
                panier.Remove(platSelectionne); // Supprime le plat sélectionné du panier
                listBoxPanier.Items.Remove(platSelectionne); // Supprime le plat de la liste du panier
                total -= platSelectionne.Price; // Déduit le prix du plat du total
                labelTotal.Text = "Total : " + total.ToString("C"); // Met à jour l'affichage du total
            }
        }

        private void buttonFinaliser_Click(object sender, RoutedEventArgs e)
        {
            if (panier.Count > 0)
            {
                string recapitulatif = "Récapitulatif de la commande :\n\n";
                foreach (Plat plat in panier)
                {
                    recapitulatif += plat.Nom + " - " + plat.Price.ToString("C") + "\n"; // Ajoute les plats et leurs prix au récapitulatif
                }
                recapitulatif += "\nTotal : " + total.ToString("C"); // Ajoute le total au récapitulatif

                MessageBox.Show(recapitulatif, "Votre commande"); // Affiche le récapitulatif de la commande dans une boîte de dialogue

                panier.Clear(); // Réinitialise le panier
                total = 0; // Réinitialise le total

                listBoxPanier.Items.Clear(); // Efface la liste du panier
                labelTotal.Text = "Total : " + total.ToString("C"); // Met à jour l'affichage du total
            }
            else
            {
                MessageBox.Show("Le panier est vide !", "Erreur"); // Affiche un message d'erreur si le panier est vide
            }
        }

        public class Plat
        {
            [JsonPropertyName("Name")]
            public string Nom { get; set; }

            public decimal Price { get; set; }

            public override string ToString()
            {
                return Nom + " - " + Price.ToString("C");
            }
        }
    }
}

