using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Babushka_Cabinet
{
    class Program
    {
        public static readonly char[] VetoChars = { '(', ')', '[', ']', '{', '}', ',', '|' };

        static void Main(string[] args)
        {
            List<Babushka> babushkaList = new List<Babushka>();
            string saveDirectory = Directory.GetCurrentDirectory() + @"\Saves\";
            string saveFile = @"Save.csv";

            bool keepRunning = true;
            string input = "";

            Babushka selectedBabushka = null;
            Babushka babushkaReturn = null;
            string selectedBabushkaMessage = "";

            CSVLoad(ref babushkaList, saveDirectory, saveFile);

            while (keepRunning)
            {
                selectedBabushkaMessage = "Currently Selected Babushka: " + (selectedBabushka == null ? "None" : selectedBabushka.Name);

                Console.Write(
                    "|--------------------------|\n" +
                    "| CPSC1012 Babushka Cabinet|\n" +
                    "|--------------------------|\n" +
                    "\n" +
                    selectedBabushkaMessage + "\n" +
                    "\n" +
                    "1. Select previously added Babushka\n" +
                    "2. Add a new Babushka\n" +
                    "3. Remove a Babushka from the system\n" +
                    "4. View Babushka's Recipes\n" +
                    "5. Add new recipe for Babushka\n" +
                    "6. Exit Program\n" +
                    "Please enter the number of the action you wish to take: ");
                input = Console.ReadLine();

                Console.WriteLine("");
                try
                {
                    switch (input)
                    {
                        //Select Babushka
                        case "1":
                            babushkaReturn = SelectBabushka(ref babushkaList);
                            selectedBabushka = babushkaReturn != null ? babushkaReturn : selectedBabushka;
                            break;

                        //Add Babushka
                        case "2":
                            babushkaReturn = AddBabushka(ref babushkaList);
                            selectedBabushka = babushkaReturn != null ? babushkaReturn : selectedBabushka;
                            break;

                        //Remove Babushka
                        case "3":
                            DeleteBabushka(ref selectedBabushka, ref babushkaList);
                            break;

                        //View Babushka's Recipes
                        case "4":
                            ViewRecipeSafe(ref selectedBabushka);
                            break;

                        //Add Recipe to Babushka
                        case "5":
                            AddRecipeSafe(ref selectedBabushka);
                            break;

                        //Exit
                        case "6":
                            keepRunning = false;
                            break;

                        default:
                            throw new Exception("Must select an option between 1-6(incusive)");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                Console.Clear();
            }
            Console.WriteLine("Good-bye and Remember stay frosty");
            Console.WriteLine("\nSaving Babushka's do not close Window until saving is complete.");
            CSVSave(babushkaList, saveDirectory + saveFile);
            Console.WriteLine("Save Complete. Press any key to exit...");
            Console.ReadKey();
        }

        static Babushka PromtForBabushkaInfoSafe()
        {
            Babushka ret = new Babushka();
            string input = "";
            bool valid = false;

            //setting Name
            while (!valid)
            {
                Console.Write("Enter the name of your Babushka: ");
                input = Console.ReadLine();
                try
                {
                    ret.Name = input;
                    valid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid input value. " + ex.Message);
                }
            }

            //setting Age
            valid = false;
            while (!valid)
            {
                Console.Write("Enter the age in years of your Babushka: ");
                input = Console.ReadLine();
                try
                {
                    ret.Age = Convert.ToInt32(input);
                    valid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid input value. " + ex.Message);
                }
            }

            //setting NumberOfDescendents
            valid = false;
            while (!valid)
            {
                Console.Write("Enter the Number of Descendents your Babushka has: ");
                input = Console.ReadLine();
                try
                {
                    ret.NumberOfDescendants = Convert.ToInt32(input);
                    valid = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid input value. " + ex.Message);
                }
            }

            //validating information is correct with user
            valid = false;
            while (!valid)
            {
                Console.WriteLine($"\nName: {ret.Name}, Age: {ret.Age} Years, number of descendents: {ret.NumberOfDescendants}");
                Console.Write("Is the information above correct? Enter y or n: ");
                input = Console.ReadLine();
                try
                {
                    switch (input)
                    {
                        case "y":
                        case "Y":
                        case "yes":
                        case "Yes":
                            valid = true;
                            break;
                        case "n":
                        case "N":
                        case "no":
                        case "No":
                            valid = true;
                            return null;
                            break;

                        default:
                            throw new ArgumentException($"Expecting: y, Y, yes, Yes, n, N, no, No. {input} is not valid.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return ret;
        }

        static void ViewRecipeSafe(ref Babushka babushka)
        {
            if (babushka == null)
            {
                Console.WriteLine("No Babushka Selected");
            }
            else if (babushka.Recipes.Count == 0)
            {
                Console.WriteLine($"{babushka.Name} has no Recipes");
            }
            else
            {
                Recipe selectedRecipe = null;

                if (babushka.Recipes.Count == 1)
                {
                    selectedRecipe = babushka.Recipes[0];
                }
                else
                {
                    for (int i = 0; i < babushka.Recipes.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {babushka.Recipes[i].Name}");
                    }

                    string input = "";
                    int index = 0;
                    bool valid = false;

                    while (!valid)
                    {
                        try
                        {
                            Console.Write("Write the number of the Recipe you'd like to view: ");
                            input = Console.ReadLine();
                            index = Convert.ToInt32(input) - 1;

                            selectedRecipe = babushka.Recipes[index];
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                Console.WriteLine($"    {selectedRecipe.Name}");
                Console.WriteLine($"\nIngredients:");
                foreach (Ingredient ingredient in selectedRecipe.Ingredients)
                {
                    Console.WriteLine($"{ingredient.Description,-30}{ingredient.Amount,7:n2} gram(s)");
                }

                Console.WriteLine($"\nDirections:");
                foreach (string direction in selectedRecipe.Directions)
                {
                    Console.WriteLine(direction);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void AddRecipeSafe(ref Babushka babushka)
        {
            if (babushka == null)
            {
                Console.WriteLine("No Babushka Selected");
            }
            else
            {
                Recipe newRecipe = null;
                string input = "";
                bool valid = false;
                bool keepRunning = true;

                //setting Name
                while (!valid)
                {
                    Console.Write($"Enter the name of {babushka.Name}'s Recipe: ");
                    input = Console.ReadLine();
                    try
                    {
                        newRecipe = new Recipe(input);
                        valid = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Invalid input value. " + ex.Message);
                    }
                }

                //Adding Ingredients
                string ingredientDescription = "";
                double ingredientAmount = 0d;
                keepRunning = true;
                while (keepRunning == true)
                {
                    Console.Write("Enter a brief Description of the Ingredient (30 chars max): ");
                    input = Console.ReadLine();
                    ingredientDescription = input;

                    valid = false;
                    while (!valid)
                    {
                        Console.Write("Enter an amount in grams for the Ingredient: ");
                        input = Console.ReadLine();
                        try
                        {
                            ingredientAmount = Convert.ToDouble(input);
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Invalid input value. " + ex.Message);
                        }
                    }
                    try
                    {
                        newRecipe.AddIngredient(ingredientDescription, ingredientAmount);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to add Ingredient. Exception: " + ex.Message);
                    }

                    Console.Write("\nWould you like to add another Ingredient? enter y or n: ");

                    valid = false;
                    while (valid == false) {
                        input = Console.ReadLine();
                        try
                        {
                            switch (input)
                            {
                                case "y":
                                case "Y":
                                case "yes":
                                case "Yes":
                                    keepRunning = true;
                                    valid = true;
                                    break;
                                case "n":
                                case "N":
                                case "no":
                                case "No":
                                    keepRunning = false;
                                    valid = true;
                                    break;

                                default:
                                    throw new ArgumentException($"Expecting: y, Y, yes, Yes, n, N, no, No. {input} is not valid.");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                //setting Directions
                keepRunning = true;
                while (keepRunning == true)
                {
                    valid = false;
                    while (!valid)
                    {
                        Console.Write($"Enter Direction No. {newRecipe.Directions.Count + 1}: ");
                        input = Console.ReadLine();
                        try
                        {
                            newRecipe.AddDirection(input);
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to add Direction. Exception: " + ex.Message);
                        }
                    }

                    Console.Write("\nWould you like to add more Directions? enter y or n: ");

                    valid = false;
                    while (valid == false)
                    {
                        input = Console.ReadLine();
                        try
                        {
                            switch (input)
                            {
                                case "y":
                                case "Y":
                                case "yes":
                                case "Yes":
                                    keepRunning = true;
                                    valid = true;
                                    break;
                                case "n":
                                case "N":
                                case "no":
                                case "No":
                                    keepRunning = false;
                                    valid = true;
                                    break;

                                default:
                                    throw new ArgumentException($"Expecting: y, Y, yes, Yes, n, N, no, No. {input} is not valid.");
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                babushka.AddRecipe(newRecipe);
            }
        }

        //creates a CSVFile From babushkaList in format "Name, Age, Weight, Type(First Character)"
        static void CSVSave(List<Babushka> babushkaList, string fileDirectory)
        {
            string[] csvFormattedArray = new string[babushkaList.Count];
            string[] babushkaProperties = new string[4];
            string[] recipesArray;
            string[] ingredientsArray;
            string[] directionsArray;
            string directionsTemp;
            string ingredientsTemp;
            string recipesTemp;
            for (int b = 0; b < csvFormattedArray.Length; b++)
            {
                //(Name[Description,Amount][Description,Amount]{Direction,Direction})
                if (babushkaList[b].Recipes.Count > 0)
                {
                    recipesArray = new string[babushkaList[b].Recipes.Count];
                    //recipes
                    for (int r = 0; r < babushkaList[b].Recipes.Count; r++)
                    {
                        //ingredients
                        ingredientsArray = new string[babushkaList[b].Recipes[r].Ingredients.Count];
                        for (int i = 0; i < babushkaList[b].Recipes[r].Ingredients.Count; i++)
                        {
                            ingredientsArray[i] = $"[{babushkaList[b].Recipes[r].Ingredients[i].Description}|{babushkaList[b].Recipes[r].Ingredients[i].Amount.ToString()}]";
                        }
                        ingredientsTemp = String.Join("", ingredientsArray);

                        //directions
                        directionsArray = new string[babushkaList[b].Recipes[r].Directions.Count];
                        for (int d = 0; d < babushkaList[b].Recipes[r].Directions.Count; d++)
                        {
                            directionsArray[d] = $"{babushkaList[b].Recipes[r].Directions[d]}";
                        }
                        directionsTemp = String.Join("|", directionsArray);

                        //recipe
                        recipesArray[r] = $"({babushkaList[b].Recipes[r].Name}{ingredientsTemp}{{{directionsTemp}}})";
                    }
                    recipesTemp = String.Join("", recipesArray);

                    //babushkas
                    babushkaProperties = new string[]
                    {
                    babushkaList[b].Name,
                    babushkaList[b].Age.ToString(),
                    babushkaList[b].NumberOfDescendants.ToString(),
                    recipesTemp
                    };
                }
                else
                {
                    //babushkas
                    babushkaProperties = new string[]
                    {
                    babushkaList[b].Name,
                    babushkaList[b].Age.ToString(),
                    babushkaList[b].NumberOfDescendants.ToString()
                    };
                }

                csvFormattedArray[b] = String.Join(",", babushkaProperties);
            }

            File.WriteAllLines(fileDirectory, csvFormattedArray);
        }

        //loads a CSVFile into babushkaList in format "Name, Age, Weight, Type"
        static void CSVLoad(ref List<Babushka> babushkaList, string directory, string file)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                File.WriteAllText(directory + file, "");
                return;
            }
            else if (!File.Exists(directory + file))
            {
                File.WriteAllText(directory + file, "");
                return;
            }
            else
            {
                string[] csvFormattedArray;
                csvFormattedArray = File.ReadAllLines(directory + file);

                string[] babushkaProperties = new string[4];
                Babushka babushkaTemp;
                int ageTemp = 0;
                int numberOfDescendantsTemp = 0;
                for (int i = 0; i < csvFormattedArray.Length; i++)
                {
                    babushkaProperties = csvFormattedArray[i].Split(',');

                    ageTemp = Convert.ToInt32(babushkaProperties[1]);
                    numberOfDescendantsTemp = Convert.ToInt32(babushkaProperties[2]);

                    babushkaTemp = new Babushka(babushkaProperties[0], ageTemp, numberOfDescendantsTemp);

                    //(Name[Description,Amount][Description,Amount]{Direction,Direction})
                    if (babushkaProperties[3] != null)
                    {
                        Recipe recipeTemp;
                        int firstNameIndex = 1;
                        int lastNameIndex = -1;
                        List<int> firstIngredientIndexes;
                        List<int> lastIngredientIndexes;
                        int firstDirectionsIndex = 0;
                        int lastDirectionsIndex = 0;

                        string name;
                        List<string[]> ingredientStrings;
                        string[] directions;


                        int c = 0;
                        while (c < babushkaProperties[3].Length)
                        {
                            lastNameIndex = -1;
                            firstIngredientIndexes = new List<int>();
                            lastIngredientIndexes = new List<int>();
                            ingredientStrings = new List<string[]>();


                            while (babushkaProperties[3][c] != ')')
                            {
                                switch (babushkaProperties[3][c])
                                {
                                    case '(':
                                        firstNameIndex = c + 1;
                                        break;

                                    case '[':
                                        if (lastNameIndex == -1)
                                        {
                                            lastNameIndex = c;
                                        }
                                        firstIngredientIndexes.Add(c + 1);
                                        break;

                                    case ']':
                                        lastIngredientIndexes.Add(c);
                                        break;

                                    case '{':
                                        firstDirectionsIndex = c + 1;
                                        break;

                                    case '}':
                                        lastDirectionsIndex = c;
                                        break;
                                }

                                //leave at bottom
                                c++;
                            }

                            name = babushkaProperties[3].Substring(firstNameIndex, lastNameIndex - firstNameIndex);
                            recipeTemp = new Recipe(name);

                            string[] ingredientStringsTemp;
                            for (int index = 0; index < lastIngredientIndexes.Count; index++)
                            {
                                ingredientStringsTemp = babushkaProperties[3].Substring(firstIngredientIndexes[index], lastIngredientIndexes[index] - firstIngredientIndexes[index]).Split('|');
                                recipeTemp.AddIngredient(ingredientStringsTemp[0], Convert.ToDouble(ingredientStringsTemp[1]));
                            }

                            directions = babushkaProperties[3].Substring(firstDirectionsIndex, lastDirectionsIndex - firstDirectionsIndex).Split('|');
                            foreach (string direction in directions)
                            {
                                recipeTemp.AddDirectionFromSave(direction);
                            }

                            babushkaTemp.AddRecipe(recipeTemp);

                            //leave at bottom
                            c++;
                        }
                    }
                    babushkaList.Add(babushkaTemp);
                }
            }
        }

        static Babushka AddBabushka(ref List<Babushka> babushkaList)
        {
            Babushka newBabushka = PromtForBabushkaInfoSafe();

            if (newBabushka != null)
            {
                babushkaList.Add(newBabushka);

                return newBabushka;
            }
            else
            {
                return null;
            }
        }

        static int? FindBabushka(ref List<Babushka> babushkaList)
        {
            string name = "";
            string input = "";

            List<int> indexes = new List<int>();

            Console.Write("What is the name of the Babushka you would like to Select? or enter List to see a list. Enter name: ");
            name = Console.ReadLine();

            if (name.ToLower() == "list")
            {
                for (int i = 0; i < babushkaList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {babushkaList[i].Name}");
                }

                Console.Write("Enter the index of the Babushka you'd like to select:");
                input = Console.ReadLine();
                try
                {
                    indexes.Add(Convert.ToInt32(input) - 1);
                    if (indexes[0] >= babushkaList.Count() || indexes[0] < 0)
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            else
            {
                for (int i = 0; i < babushkaList.Count; i++)
                {
                    if (babushkaList[i].Name.ToLower() == name.ToLower())
                    {
                        indexes.Add(i);
                    }
                }
            }

            Console.WriteLine("");

            bool escape = false;
            for (int i = 0; i < indexes.Count && !escape; i++)
            {
                Console.WriteLine($"Name: {babushkaList[indexes[i]].Name}, Age: {babushkaList[indexes[i]].Age} Years, Number of Descedents: {babushkaList[indexes[i]].NumberOfDescendants}");
                Console.Write("Is the information above the Babushka you would like to Select, correct? Enter y, n or any other key to escape: ");
                input = Console.ReadLine();
                switch (input)
                {
                    case "y":
                    case "Y":
                    case "yes":
                    case "Yes":
                        return indexes[i];
                        break;
                    case "n":
                    case "N":
                    case "no":
                    case "No":
                        //loop again
                        break;

                    default:
                        escape = true;
                        break;
                }
            }

            Console.WriteLine("");

            if (!escape)
            {
                if (indexes.Count == 0)
                {
                    Console.WriteLine($"Could not find any Babushka's with the name {name}");
                }
                else
                {
                    Console.WriteLine($"There are no more Babushka's with the name {name}");
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        static void DeleteBabushka(ref Babushka babushka, ref List<Babushka> babushkaList)
        {
            if (babushka == null)
            {
                Console.WriteLine("There is no selected Babushka to Delete");
                return;
            }
            else
            {
                string input = "";

                Console.WriteLine($"Name: {babushka.Name}, Age: {babushka.Age} Years, Number of Descedents: {babushka.NumberOfDescendants}");
                Console.Write("Are you sure you want to delete this Babushka? Enter DELETE to delete the babushka and anything else to exit: ");
                input = Console.ReadLine();

                if (input == "DELETE")
                {
                    babushkaList.Remove(babushka);
                    Console.WriteLine("Babushka deleted");
                    babushka = null;
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        static Babushka SelectBabushka(ref List<Babushka> babushkaList)
        {
            int? index;
            index = FindBabushka(ref babushkaList);

            if (index == null)
            {
                return null;
            }
            else
            {
                return babushkaList[(int)index];
            }
        }
    }

    public class Babushka
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (value.IndexOfAny(Program.VetoChars) != -1)
                    {
                        throw new StringInvalidCharsException("Name");
                    }
                    else
                    {
                        _name = value;
                    }
                }
                else
                {
                    throw new EmptyStringException("Name");
                }
            }
        }

        private int _age;
        public int Age
        {
            get => _age;
            set
            {
                if (value >= 1)
                {
                    _age = value;
                }
                else
                {
                    throw new ArgumentException("Age must be at least 1 years old.");
                }
            }
        }

        private int _numberOfDescendants;

        public int NumberOfDescendants
        {
            get => _numberOfDescendants;
            set
            {
                if (value >= 0)
                {
                    _numberOfDescendants = value;
                }
                else
                {
                    throw new ArgumentException("Number of Decsendents must be a positive number");
                }
            }
        }

        private List<Recipe> _recipes = new List<Recipe>();

        public List<Recipe> Recipes
        {
            get => _recipes;
        }

        public Babushka(string name, int age, int numberOfDecendents)
        {
            Name = name;
            Age = age;
            NumberOfDescendants = numberOfDecendents;
        }

        public Babushka() : this("TempName", 100, 0) { }

        public void AddRecipe(Recipe newRecipe)
        {
            Recipes.Add(newRecipe);
        }

    }

    public class Recipe
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (value.IndexOfAny(Program.VetoChars) != -1)
                    {
                        throw new StringInvalidCharsException("Name");
                    }
                    else
                    {
                        _name = value;
                    }
                }
                else
                {
                    throw new EmptyStringException("Name");
                }
            }
        }

        private List<Ingredient> _ingredients = new List<Ingredient>();

        public List<Ingredient> Ingredients
        {
            get => _ingredients;
        }

        private List<string> _directions = new List<string>();

        public List<string> Directions
        {
            get => _directions;
        }

        public Recipe(string name)
        {
            Name = name;
        }

        public void AddIngredient(string description, double amount)
        {
            if (!String.IsNullOrWhiteSpace(description))
            {
                if (description.IndexOfAny(Program.VetoChars) != -1)
                {
                    throw new StringInvalidCharsException("Description");
                }
                else
                {
                    Ingredients.Add(new Ingredient(description, amount));
                }
            }
            else
            {
                throw new EmptyStringException("description");
            }
        }

        public void AddDirection(string direction)
        {
            if (!String.IsNullOrWhiteSpace(direction))
            {
                if (direction.IndexOfAny(Program.VetoChars) != -1)
                {
                    throw new StringInvalidCharsException("Direction");
                }
                else
                {
                    Directions.Add($"{Directions.Count + 1}. {direction}");
                }
            } 
            else
            {
                throw new EmptyStringException("Direction");
            }
        }

        public void AddDirectionFromSave(string direction)
        {
            if (!String.IsNullOrWhiteSpace(direction))
            {
                Directions.Add($"{direction}");
            }
            else
            {
                throw new EmptyStringException("Direction");
            }
        }
    }


    public class EmptyStringException : ArgumentException
    {
        public EmptyStringException(string callerName)
        {
            Message = callerName + " can not be empty, null, or whitespace";
        }

        public override string Message { get; }
    }

    public class StringInvalidCharsException : ArgumentException
    {
        public StringInvalidCharsException(string callerName)
        {
            Message = callerName + " can not Contain any of these characters => '()[]{},|'";
        }

        public override string Message { get; }
    }

    public struct Ingredient 
    {
        public string Description;
        public double Amount;

        public Ingredient(string description, double amount)
        {
            if (description.Length > 30)
            {
                throw new ArgumentException("Description must be less than 30 Characters");
            }
            else
            {
                Description = description;
            }
            Amount = amount;
        }
    }
}
