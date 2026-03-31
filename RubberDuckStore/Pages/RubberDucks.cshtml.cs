using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

//using rendering, razor and sqlite, and collection classes like lists

namespace RubberDuckStore.Pages
{
    // This is a PageModel for a Razor Page that handles displaying rubber duck products
    // We are now creating the pages for the ducks to display
    public class RubberDucksModel : PageModel
    {
        // Property that will store the selected duck ID from form submissions
        // Need to set up a bind property to the form field
        // This is public integer (get/set)
        [BindProperty]
        public int SelectedDuckId { get; set; }

        // List that will hold all ducks for the dropdown selection
        //SelectListItem is the dropdown
        public List<SelectListItem> DuckList { get; set; }
        
        // Property that will store the currently selected duck object
        // Once the user picks the duck, it is stored here
        public Duck SelectedDuck { get; set; }

        // Handles HTTP GET requests to the page - loads the list of ducks
        // This will show all the list on the dropdown list
        public void OnGet()
        {
            LoadDuckList();
        }


        // Handles HTTP POST requests (when user selects a duck) - loads the duck list
        // and retrieves the selected duck's details
        // IActionResult 
        public IActionResult OnPost()
        {
            //Call the load duck list method
            LoadDuckList();
            // Error handling - make sure there is a valid duck ID
            if (SelectedDuckId != 0)
            {
                // Get the duck from DB and create the duck
                SelectedDuck = GetDuckById(SelectedDuckId);
            }

            // Return the Page so that it can be displayed in the browser
            return Page();

        }  //end Post


        // Helper method that loads the list of ducks from the SQLite database
        // for displaying in a dropdown menu
        // This is a private method
        private void LoadDuckList()
        {
            // Create a new list
            DuckList = new List<SelectListItem>();

            // Create a connection to the SQL DB
            // Create the constructor 
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db"))
            {
                // Open the Connection 
                connection.Open();
                
                // Create a SQL command and set up SQL query to select duck
                // User will see name but we will pick from the ID
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Ducks";

                // Create reader to Read each of the row set from the DB
                using (var reader = command.ExecuteReader())
                {
                    // Use while loop to iterate through all the records in the result
                    // set from the database
                    // This will just read the next record in the result set
                    // If there is no record, it will read false
                    while (reader.Read())
                    {
                        // Create a new list from the current duck result list
                        // and add that ducj to the dropdown list
                        // Value is the hidden ID: Text is what is shown to the user
                        DuckList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(), // Duck ID as the value
                            Text = reader.GetString(1)             // Duck name as the display text
                        }); 
                    } 
                }  
            }
        }

        // Helper method that retrieves a specific duck by its ID from the database
        // Returns all details of the duck
        private Duck GetDuckById(int id)
        {
            // Create DB Connection
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db"))
            {
                // Open the connection to the DB
                connection.Open();
                // Create command to execute the SQL query to get duck
                // The previous method displayed the list of ducks query
                // This method selects the duck/ THAT is the COMMAND
                var command = connection.CreateCommand();
                // 
                command.CommandText = "SELECT * FROM Ducks WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id); // Using parameterized query for security

                // Create a reader to read the result set - This is only 1 result                
                using (var reader = command.ExecuteReader())
                {
                    // Error handling - If the reader has another record, then read it
                    // There should only be one record since this is a unique result
                    if (reader.Read())
                    {
                        return new Duck
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageFileName = reader.GetString(4)
                        };
                    } // end if
                } // end using reader
            } // end helper method
            return null; // Return null if none found
        } // end method
    } // end model class


    // Simple model class representing a rubber duck product
    // This stores data about the duck and displays it
    // This also mimicks the database display.
    public class Duck
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageFileName { get; set; }
    } // end duck class
}    