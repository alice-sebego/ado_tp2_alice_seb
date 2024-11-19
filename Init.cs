using System;
using System.Data.SqlClient;

class Init {
    public static void InitializeDatabase() {
        string connectionString = "Server=localhost;Database=master;Trusted_Connection=True;";
        string storeDbConnectionString = "Server=localhost;Database=StoreDB;Trusted_Connection=True;";

        string createDatabaseQuery = @"
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StoreDB')
            BEGIN
                CREATE DATABASE StoreDB;
            END";

        string createCategoriesTableQuery = @"
            USE StoreDB;
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
            BEGIN
                CREATE TABLE Categories (
                    pk_cat INT IDENTITY(1,1) PRIMARY KEY,
                    nom_cat NVARCHAR(255) NOT NULL
                );
            END";

        string createProductsTableQuery = @"
            USE StoreDB;
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
            BEGIN
                CREATE TABLE Products (
                    pk_pro INT IDENTITY(1,1) PRIMARY KEY,
                    nom_pro NVARCHAR(255) NOT NULL,
                    quantite_pro INT NOT NULL,
                    prix_pro DECIMAL(10, 2) NOT NULL,
                    fk_cat INT NOT NULL,
                    FOREIGN KEY (fk_cat) REFERENCES Categories(pk_cat)
                );
            END";

        using (SqlConnection connection = new SqlConnection(connectionString)) {
            try {
                connection.Open();

                // Create database
                using (SqlCommand command = new SqlCommand(createDatabaseQuery, connection)) {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Base de données 'StoreDB' créée ou déjà existante.");
                }

                // Create "Categories" table
                using (SqlCommand command = new SqlCommand(createCategoriesTableQuery, connection)) {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Table 'Categories' créée ou déjà existante.");
                }

                // Create "Products" table
                using (SqlCommand command = new SqlCommand(createProductsTableQuery, connection)) {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Table 'Products' créée ou déjà existante.");
                }
            } catch (Exception e) {
                Console.WriteLine($"Erreur lors de l'initialisation de la base de données : {e.Message}");
            }
        }

        // Insert data into tables
        SetData.InsertData(storeDbConnectionString);
    }
}
