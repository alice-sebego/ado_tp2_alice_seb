using System;
using System.Data.SqlClient;

class SetData {
    public static void InsertData(string connectionString) {
        // Query to insert categories if not already existing
        string insertCategoriesQuery = @"
            IF NOT EXISTS (SELECT 1 FROM Categories WHERE nom_cat = 'Football')
                INSERT INTO Categories (nom_cat) VALUES ('Football');
            IF NOT EXISTS (SELECT 1 FROM Categories WHERE nom_cat = 'Athletisme')
                INSERT INTO Categories (nom_cat) VALUES ('Athletisme');
            IF NOT EXISTS (SELECT 1 FROM Categories WHERE nom_cat = 'Judo')
                INSERT INTO Categories (nom_cat) VALUES ('Judo');
        ";

        // Query to insert products if not already existing
        string insertProductsQuery = @"
            -- Football products
            IF NOT EXISTS (SELECT 1 FROM Products WHERE nom_pro = 'Ballon de football')
                INSERT INTO Products (nom_pro, quantite_pro, prix_pro, fk_cat) 
                VALUES ('Ballon de football', 20, 25.99, (SELECT pk_cat FROM Categories WHERE nom_cat = 'Football'));
            IF NOT EXISTS (SELECT 1 FROM Products WHERE nom_pro = 'Maillot de football')
                INSERT INTO Products (nom_pro, quantite_pro, prix_pro, fk_cat) 
                VALUES ('Maillot de football', 30, 49.99, (SELECT pk_cat FROM Categories WHERE nom_cat = 'Football'));

            -- Athletisme products
            IF NOT EXISTS (SELECT 1 FROM Products WHERE nom_pro = 'Chaussures de course')
                INSERT INTO Products (nom_pro, quantite_pro, prix_pro, fk_cat) 
                VALUES ('Chaussures de course', 15, 89.99, (SELECT pk_cat FROM Categories WHERE nom_cat = 'Athletisme'));
            IF NOT EXISTS (SELECT 1 FROM Products WHERE nom_pro = 'Chronometre')
                INSERT INTO Products (nom_pro, quantite_pro, prix_pro, fk_cat) 
                VALUES ('Chronometre', 10, 29.99, (SELECT pk_cat FROM Categories WHERE nom_cat = 'Athletisme'));

            -- Judo products
            IF NOT EXISTS (SELECT 1 FROM Products WHERE nom_pro = 'Kimono')
                INSERT INTO Products (nom_pro, quantite_pro, prix_pro, fk_cat) 
                VALUES ('Kimono', 25, 59.99, (SELECT pk_cat FROM Categories WHERE nom_cat = 'Judo'));
            IF NOT EXISTS (SELECT 1 FROM Products WHERE nom_pro = 'Ceinture de judo')
                INSERT INTO Products (nom_pro, quantite_pro, prix_pro, fk_cat) 
                VALUES ('Ceinture de judo', 40, 15.99, (SELECT pk_cat FROM Categories WHERE nom_cat = 'Judo'));
        ";

        using (SqlConnection connection = new SqlConnection(connectionString)) {
            try {
                connection.Open();

                // Insert categories
                using (SqlCommand command = new SqlCommand(insertCategoriesQuery, connection)) {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Donnees inserees dans la table 'Categories'.");
                }

                // Insert products
                using (SqlCommand command = new SqlCommand(insertProductsQuery, connection)) {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Donnees inserees dans la table 'Products'.");
                }
            } catch (Exception e) {
                Console.WriteLine($"Erreur lors de l'insertion des donnees : {e.Message}");
            }
        }
    }
}
