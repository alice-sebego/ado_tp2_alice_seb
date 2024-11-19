using System;
using System.Data.SqlClient;
using System.Globalization;

class MainClass {
    static string connectionString = "Server=localhost;Database=StoreDB;Trusted_Connection=True;";

    public static void Main(string[] args) {
        while (true) {
            // Display the menu
            Console.Clear();
            Console.WriteLine("Choisissez une option :");
            Console.WriteLine("1. Ajouter un produit");
            Console.WriteLine("2. Modifier le prix d'un produit");
            Console.WriteLine("3. Supprimer un produit");
            Console.WriteLine("4. Afficher tous les produits par catégorie");
            Console.WriteLine("5. Afficher le prix total de tous les produits");
            Console.WriteLine("6. Afficher les catégories");
            Console.WriteLine("7. Quitter");
            Console.Write("Entrez votre choix : ");
            string choice = Console.ReadLine();

            switch (choice) {
                case "1":
                    AddProduct();
                    break;
                case "2":
                    ModifyProductPrice();
                    break;
                case "3":
                    DeleteProduct();
                    break;
                case "4":
                    DisplayProductsByCategory();
                    break;
                case "5":
                    DisplayTotalPrice();
                    break;
                case "6":
                    DisplayCategories();
                    break;
                case "7":
                    return; // Exit the application
                default:
                    Console.WriteLine("Choix invalide. Veuillez réessayer.");
                    break;
            }

            // Wait for user input before returning to the menu
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
    }

   
    // Method to add a product
    static void AddProduct() {
        // Get product details from the user
        Console.Write("Entrez le nom du produit : ");
        string name = Console.ReadLine();
        Console.Write("Entrez la quantité : ");
        int quantity = int.Parse(Console.ReadLine());

        decimal price;
        while (true) {
            Console.Write("Entrez le prix : ");
            string priceInput = Console.ReadLine();

            // Use a specific culture (e.g., en-US) that uses the dot as the decimal separator
            if (decimal.TryParse(priceInput, NumberStyles.Number, new CultureInfo("en-US"), out price)) {
                break; // Exit loop if parsing is successful
            } else {
                Console.WriteLine("Le format du prix est invalide. Veuillez entrer un prix valide.");
            }
        }

        Console.Write("Entrez le nom de la catégorie : ");
        string category = Console.ReadLine();

        string query = @"
            INSERT INTO Products (nom_pro, quantite_pro, prix_pro, fk_cat)
            VALUES (@name, @quantity, @price, 
                (SELECT pk_cat FROM Categories WHERE nom_cat = @category));
        ";

        using (SqlConnection connection = new SqlConnection(connectionString)) {
            try {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@price", price);
                    command.Parameters.AddWithValue("@category", category);
                    int rowsAffected = command.ExecuteNonQuery();
                    // Inform the user of the successful operation
                    Console.WriteLine($"{rowsAffected} produit(s) ajouté(s) avec succès.");
                }
            } catch (Exception e) {
                // Display error message if there's an issue
                Console.WriteLine($"Erreur lors de l'ajout du produit : {e.Message}");
            }
        }
    }

    
    // Method to modify the price of a product
    static void ModifyProductPrice() {
        // Ask the user for the product name and the new price
        Console.Write("Entrez le nom du produit à modifier : ");
        string productName = Console.ReadLine();

        decimal newPrice;
        while (true) {
            Console.Write("Entrez le nouveau prix : ");
            string priceInput = Console.ReadLine();

            // Use a specific culture (e.g., en-US) that uses the dot as the decimal separator
            if (decimal.TryParse(priceInput, NumberStyles.Number, new CultureInfo("en-US"), out newPrice)) {
                break; // Exit loop if parsing is successful
            } else {
                Console.WriteLine("Le format du prix est invalide. Veuillez entrer un prix valide.");
            }
        }

        string query = @"
            UPDATE Products
            SET prix_pro = @newPrice
            WHERE nom_pro = @productName;
        ";

        using (SqlConnection connection = new SqlConnection(connectionString)) {
            try {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@newPrice", newPrice);
                    command.Parameters.AddWithValue("@productName", productName);
                    int rowsAffected = command.ExecuteNonQuery();
                    // Inform the user of the successful operation
                    Console.WriteLine($"{rowsAffected} produit(s) modifié(s).");
                }
            } catch (Exception e) {
                // Display error message if there's an issue
                Console.WriteLine($"Erreur lors de la modification du prix : {e.Message}");
            }
        }
    }

    // Method to delete a product
    static void DeleteProduct() {
        // Ask the user for the product name to delete
        Console.Write("Entrez le nom du produit à supprimer : ");
        string productName = Console.ReadLine();

        string query = @"
            DELETE FROM Products
            WHERE nom_pro = @productName;
        ";

        using (SqlConnection connection = new SqlConnection(connectionString)) {
            try {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@productName", productName);
                    int rowsAffected = command.ExecuteNonQuery();
                    // Inform the user of the successful operation
                    Console.WriteLine($"{rowsAffected} produit(s) supprimé(s).");
                }
            } catch (Exception e) {
                // Display error message if there's an issue
                Console.WriteLine($"Erreur lors de la suppression du produit : {e.Message}");
            }
        }
    }

    // Method to display products by category
    static void DisplayProductsByCategory() {
        // Ask the user for the category name
        Console.Write("Entrez le nom de la catégorie : ");
        string categoryName = Console.ReadLine();

        string query = @"
            SELECT nom_pro, quantite_pro, prix_pro
            FROM Products
            WHERE fk_cat = (SELECT pk_cat FROM Categories WHERE nom_cat = @categoryName);
        ";

        using (SqlConnection connection = new SqlConnection(connectionString)) {
            try {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@categoryName", categoryName);
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        // Display the products of the specified category
                        Console.WriteLine($"Produits dans la catégorie '{categoryName}' :");
                        while (reader.Read()) {
                            Console.WriteLine($"Nom : {reader["nom_pro"]}, Quantité : {reader["quantite_pro"]}, Prix : {reader["prix_pro"]}");
                        }
                    }
                }
            } catch (Exception e) {
                // Display error message if there's an issue
                Console.WriteLine($"Erreur lors de l'affichage des produits par catégorie : {e.Message}");
            }
        }
    }

    // Method to display the total price of all products
    static void DisplayTotalPrice() {
        string query = @"
            SELECT SUM(prix_pro * quantite_pro) AS totalPrice
            FROM Products;
        ";

        using (SqlConnection connection = new SqlConnection(connectionString)) {
            try {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    var result = command.ExecuteScalar();
                     // Display the total price
                    Console.WriteLine($"Prix total de tous les produits : {result:C}");
                }
            } catch (Exception e) {
                // Display error message if there's an issue
                Console.WriteLine($"Erreur lors du calcul du prix total : {e.Message}");
            }
        }
    }

    // Method to display categories
    static void DisplayCategories() {
        string query = "SELECT pk_cat, nom_cat FROM Categories";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("Catégories disponibles :");
                        Console.WriteLine("------------------------");

                        while (reader.Read())
                        {
                            int categoryId = reader.GetInt32(0);
                            string categoryName = reader.GetString(1);
                            Console.WriteLine($"ID: {categoryId} | Nom: {categoryName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'affichage des catégories : {ex.Message}");
            }
        }
    }
}
 