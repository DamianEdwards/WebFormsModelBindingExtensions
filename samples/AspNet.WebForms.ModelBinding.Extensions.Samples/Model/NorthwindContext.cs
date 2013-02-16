using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AspNet.WebForms.ModelBinding.Extensions.Samples.Model
{
    public class NorthwindContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }

    public class NorthwindContextDbInitializer : DropCreateDatabaseAlways<NorthwindContext>
    {
        protected override void Seed(NorthwindContext context)
        {
            // Beverages
            var beverages = new Category { Name = "Beverages" };
            beverages.Products.Add(new Product { Name = "Chai", UnitPrice = 18.00, StockOnHand = 39, Category = beverages });
            beverages.Products.Add(new Product { Name = "Chang", UnitPrice = 19.00, StockOnHand = 17, Category = beverages });
            beverages.Products.Add(new Product { Name = "Guaraná Fantástica", UnitPrice = 4.50, StockOnHand = 20, Category = beverages });
            beverages.Products.Add(new Product { Name = "Sasquatch Ale", UnitPrice = 14.00, StockOnHand = 111, Category = beverages });
            beverages.Products.Add(new Product { Name = "Steeleye Stout", UnitPrice = 18.00, StockOnHand = 20, Category = beverages });
            beverages.Products.Add(new Product { Name = "Côte de Blaye", UnitPrice = 263.50, StockOnHand = 17, Category = beverages });
            beverages.Products.Add(new Product { Name = "Chartreuse verte", UnitPrice = 18.00, StockOnHand = 69, Category = beverages });
            beverages.Products.Add(new Product { Name = "Ipoh Coffee", UnitPrice = 46.00, StockOnHand = 17, Category = beverages });
            beverages.Products.Add(new Product { Name = "Outback Lager", UnitPrice = 15.00, StockOnHand = 15, Category = beverages });
            beverages.Products.Add(new Product { Name = "Laughing Lumberjack Lager", UnitPrice = 14.00, StockOnHand = 52, Category = beverages });
            beverages.Products.Add(new Product { Name = "Rhönbräu Klosterbier", UnitPrice = 7.75, StockOnHand = 125, Category = beverages });
            beverages.Products.Add(new Product { Name = "Lakkalikööri", UnitPrice = 18.00, StockOnHand = 57, Category = beverages });
            context.Categories.Add(beverages);

            // Condiments
            var condiments = new Category { Name = "Condiments" };
            condiments.Products.Add(new Product { Name = "Original Frankfurter grüne Soße", UnitPrice = 13.00, StockOnHand = 32, Category = condiments });
            condiments.Products.Add(new Product { Name = "Sirop d'érable", UnitPrice = 28.50, StockOnHand = 113, Category = condiments });
            condiments.Products.Add(new Product { Name = "Vegie-spread", UnitPrice = 43.90, StockOnHand = 24, Category = condiments });
            condiments.Products.Add(new Product { Name = "Louisiana Fiery Hot Pepper Sauce", UnitPrice = 21.05, StockOnHand = 76, Category = condiments });
            condiments.Products.Add(new Product { Name = "Louisiana Hot Spiced Okra", UnitPrice = 17.00, StockOnHand = 4, Category = condiments });
            condiments.Products.Add(new Product { Name = "Gula Malacca", UnitPrice = 19.45, StockOnHand = 27, Category = condiments });
            condiments.Products.Add(new Product { Name = "Aniseed Syrup", UnitPrice = 10.00, StockOnHand = 13, Category = condiments });
            condiments.Products.Add(new Product { Name = "Chef Anton's Cajun Seasoning", UnitPrice = 22.00, StockOnHand = 53, Category = condiments });
            condiments.Products.Add(new Product { Name = "Chef Anton's Gumbo Mix", UnitPrice = 21.35, StockOnHand = 0, Category = condiments });
            condiments.Products.Add(new Product { Name = "Grandma's Boysenberry Spread", UnitPrice = 25.00, StockOnHand = 120, Category = condiments });
            condiments.Products.Add(new Product { Name = "Northwoods Cranberry Sauce", UnitPrice = 40.00, StockOnHand = 6, Category = condiments });
            condiments.Products.Add(new Product { Name = "Genen Shouyu", UnitPrice = 15.50, StockOnHand = 39, Category = condiments });
            context.Categories.Add(condiments);

            //// Confections
            var confections = new Category { Name = "Confections" };
            confections.Products.Add(new Product { Name = "Pavlova", UnitPrice = 17.45, StockOnHand = 29, Category = confections });
            confections.Products.Add(new Product { Name = "Teatime Chocolate Biscuits", UnitPrice = 9.20, StockOnHand = 25, Category = confections });
            confections.Products.Add(new Product { Name = "Sir Rodney's Marmalade", UnitPrice = 81.00, StockOnHand = 40, Category = confections });
            confections.Products.Add(new Product { Name = "Sir Rodney's Scones", UnitPrice = 10.00, StockOnHand = 3, Category = confections });
            confections.Products.Add(new Product { Name = "NuNuCa Nuß-Nougat-Creme", UnitPrice = 14.00, StockOnHand = 76, Category = confections });
            confections.Products.Add(new Product { Name = "Gumbär Gummibärchen", UnitPrice = 31.23, StockOnHand = 15, Category = confections });
            confections.Products.Add(new Product { Name = "Schoggi Schokolade", UnitPrice = 43.90, StockOnHand = 49, Category = confections });
            confections.Products.Add(new Product { Name = "Zaanse koeken", UnitPrice = 9.50, StockOnHand = 36, Category = confections });
            confections.Products.Add(new Product { Name = "Chocolade", UnitPrice = 12.75, StockOnHand = 15, Category = confections });
            confections.Products.Add(new Product { Name = "Maxilaku", UnitPrice = 20.00, StockOnHand = 10, Category = confections });
            confections.Products.Add(new Product { Name = "Valkoinen suklaa", UnitPrice = 16.25, StockOnHand = 65, Category = confections });
            confections.Products.Add(new Product { Name = "Tarte au sucre", UnitPrice = 49.30, StockOnHand = 17, Category = confections });
            confections.Products.Add(new Product { Name = "Scottish Longbreads", UnitPrice = 12.50, StockOnHand = 6, Category = confections });
            context.Categories.Add(confections);

            //// Dairy Products
            var dairyproducts = new Category { Name = "Dairy Products" };
            dairyproducts.Products.Add(new Product { Name = "Gudbrandsdalsost", UnitPrice = 36.00, StockOnHand = 26, Category = dairyproducts });
            dairyproducts.Products.Add(new Product { Name = "Flotemysost", UnitPrice = 21.50, StockOnHand = 26, Category = dairyproducts });
            dairyproducts.Products.Add(new Product { Name = "Mozzarella di Giovanni", UnitPrice = 34.80, StockOnHand = 14, Category = dairyproducts });
            dairyproducts.Products.Add(new Product { Name = "Raclette Courdavault", UnitPrice = 55.00, StockOnHand = 79, Category = dairyproducts });
            dairyproducts.Products.Add(new Product { Name = "Camembert Pierrot", UnitPrice = 34.00, StockOnHand = 19, Category = dairyproducts });
            dairyproducts.Products.Add(new Product { Name = "Gorgonzola Telino", UnitPrice = 12.50, StockOnHand = 0, Category = dairyproducts });
            dairyproducts.Products.Add(new Product { Name = "Mascarpone Fabioli", UnitPrice = 32.00, StockOnHand = 9, Category = dairyproducts });
            dairyproducts.Products.Add(new Product { Name = "Geitost", UnitPrice = 2.50, StockOnHand = 112, Category = dairyproducts });
            dairyproducts.Products.Add(new Product { Name = "Queso Cabrales", UnitPrice = 21.00, StockOnHand = 22, Category = dairyproducts });
            dairyproducts.Products.Add(new Product { Name = "Queso Manchego La Pastora", UnitPrice = 38.00, StockOnHand = 86, Category = dairyproducts });
            context.Categories.Add(dairyproducts);

            //// Grains/Cereals
            var grainscereals = new Category { Name = "Grains/Cereals" };
            grainscereals.Products.Add(new Product { Name = "Gustaf's Knäckebröd", UnitPrice = 21.00, StockOnHand = 104, Category = grainscereals });
            grainscereals.Products.Add(new Product { Name = "Tunnbröd", UnitPrice = 9.00, StockOnHand = 61, Category = grainscereals });
            grainscereals.Products.Add(new Product { Name = "Wimmers gute Semmelknödel", UnitPrice = 33.25, StockOnHand = 22, Category = grainscereals });
            grainscereals.Products.Add(new Product { Name = "Filo Mix", UnitPrice = 7.00, StockOnHand = 38, Category = grainscereals });
            grainscereals.Products.Add(new Product { Name = "Singaporean Hokkien Fried Mee", UnitPrice = 14.00, StockOnHand = 26, Category = grainscereals });
            grainscereals.Products.Add(new Product { Name = "Gnocchi di nonna Alice", UnitPrice = 38.00, StockOnHand = 21, Category = grainscereals });
            grainscereals.Products.Add(new Product { Name = "Ravioli Angelo", UnitPrice = 19.50, StockOnHand = 36, Category = grainscereals });
            context.Categories.Add(grainscereals);

            //// Meat/Poultry
            var meatpoultry = new Category { Name = "Meat/Poultry" };
            meatpoultry.Products.Add(new Product { Name = "Perth Pasties", UnitPrice = 32.80, StockOnHand = 0, Category = meatpoultry });
            meatpoultry.Products.Add(new Product { Name = "Tourtière", UnitPrice = 7.45, StockOnHand = 21, Category = meatpoultry });
            meatpoultry.Products.Add(new Product { Name = "Pâté chinois", UnitPrice = 24.00, StockOnHand = 115, Category = meatpoultry });
            meatpoultry.Products.Add(new Product { Name = "Alice Mutton", UnitPrice = 39.00, StockOnHand = 0, Category = meatpoultry });
            meatpoultry.Products.Add(new Product { Name = "Mishi Kobe Niku", UnitPrice = 97.00, StockOnHand = 29, Category = meatpoultry });
            meatpoultry.Products.Add(new Product { Name = "Thüringer Rostbratwurst", UnitPrice = 123.79, StockOnHand = 0, Category = meatpoultry });
            context.Categories.Add(meatpoultry);

            //// Produce
            var produce = new Category { Name = "Produce" };
            produce.Products.Add(new Product { Name = "Rössle Sauerkraut", UnitPrice = 45.60, StockOnHand = 26, Category = produce });
            produce.Products.Add(new Product { Name = "Uncle Bob's Organic Dried Pears", UnitPrice = 30.00, StockOnHand = 15, Category = produce });
            produce.Products.Add(new Product { Name = "Tofu", UnitPrice = 23.25, StockOnHand = 35, Category = produce });
            produce.Products.Add(new Product { Name = "Manjimup Dried Apples", UnitPrice = 53.00, StockOnHand = 20, Category = produce });
            produce.Products.Add(new Product { Name = "Longlife Tofu", UnitPrice = 10.00, StockOnHand = 4, Category = produce });
            context.Categories.Add(produce);

            //// Seafood
            var seafood = new Category { Name = "Seafood" };
            seafood.Products.Add(new Product { Name = "Rogede sild", UnitPrice = 9.50, StockOnHand = 5, Category = seafood });
            seafood.Products.Add(new Product { Name = "Spegesild", UnitPrice = 12.00, StockOnHand = 95, Category = seafood });
            seafood.Products.Add(new Product { Name = "Escargots de Bourgogne", UnitPrice = 13.25, StockOnHand = 62, Category = seafood });
            seafood.Products.Add(new Product { Name = "Röd Kaviar", UnitPrice = 15.00, StockOnHand = 101, Category = seafood });
            seafood.Products.Add(new Product { Name = "Ikura", UnitPrice = 31.00, StockOnHand = 31, Category = seafood });
            seafood.Products.Add(new Product { Name = "Konbu", UnitPrice = 6.00, StockOnHand = 24, Category = seafood });
            seafood.Products.Add(new Product { Name = "Carnarvon Tigers", UnitPrice = 62.50, StockOnHand = 42, Category = seafood });
            seafood.Products.Add(new Product { Name = "Nord-Ost Matjeshering", UnitPrice = 25.89, StockOnHand = 10, Category = seafood });
            seafood.Products.Add(new Product { Name = "Boston Crab Meat", UnitPrice = 18.40, StockOnHand = 123, Category = seafood });
            seafood.Products.Add(new Product { Name = "Jack's New England Clam Chowder", UnitPrice = 9.65, StockOnHand = 85, Category = seafood });
            seafood.Products.Add(new Product { Name = "Inlagd Sill", UnitPrice = 19.00, StockOnHand = 112, Category = seafood });
            seafood.Products.Add(new Product { Name = "Gravad lax", UnitPrice = 26.00, StockOnHand = 11, Category = seafood });
            context.Categories.Add(seafood);

            context.SaveChanges();
        }
    }
}