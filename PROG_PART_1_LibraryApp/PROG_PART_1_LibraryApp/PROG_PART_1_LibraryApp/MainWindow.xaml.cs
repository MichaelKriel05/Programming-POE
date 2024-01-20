using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


//References used:
//https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.timer.tick?view=windowsdesktop-7.0
//https://www.tutorialspoint.com/how-to-select-a-random-element-from-a-chash-list
//https://www.tutorialsteacher.com/csharp/csharp-dictionary
//https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-7.0
//https://stackoverflow.com/questions/69897203/match-the-columns-using-a-data-dictionary-in-c-sharp-winform
//https://medium.com/geekculture/converting-dictionary-to-generic-list-t-with-custom-attribute-1bea1c2f3a1d


namespace PROG_PART_1_LibraryApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private ReplaceBook replaceBook = new ReplaceBook();
        public IdentifyArea identifyArea;
        public DeweyCategories deweyCategories;
        private SolidColorBrush highlightBrush = new SolidColorBrush(Colors.LightSkyBlue);
        public bool isCallNumbersOnLeft = true;
        public int IAroundcount = 0;

        private List<Category> deweyDecimalHierarchy;
        private Category currentThirdLevelCategory;


        public MainWindow()
        {
            InitializeComponent();

            // Initialize and configure the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            identifyArea = new IdentifyArea();
            deweyCategories = new DeweyCategories();

            LoadDeweyDecimalHierarchy();
        }   

        public void Timer_Tick(object sender, EventArgs e)
        {            
            MotivateLabel.Visibility = Visibility.Hidden;
            MotivateCorrectIcon.Visibility = Visibility.Hidden;
            IncorrectLabel.Visibility = Visibility.Hidden;
            IncorrectIcon.Visibility = Visibility.Hidden;
            MotivateLabelIA.Visibility = Visibility.Hidden;
            MotivateCorrectIconIA.Visibility = Visibility.Hidden;
            IncorrectLabelIA.Visibility = Visibility.Hidden;
            IncorrectIconIA.Visibility = Visibility.Hidden;
            timer.Stop();
        }

        private void ShowCorrectStuff()
        {
            // Show the label and start the timer
            MotivateLabel.Visibility = Visibility.Visible;
            MotivateCorrectIcon.Visibility = Visibility.Visible;
            timer.Start();
        }
        private void ShowWrongStuff()
        {
            // Show the label and start the timer
            IncorrectLabel.Visibility = Visibility.Visible;
            IncorrectIcon.Visibility = Visibility.Visible;
            timer.Start();
        }
        public void btnReplaceBooks_Click(object sender, RoutedEventArgs e)
        {
            MenuGrid.Visibility = Visibility.Collapsed;
            ReplaceBooksGrid.Visibility = Visibility.Visible;
            HowToPlayReplaceBooks.Visibility = Visibility.Visible;
            StartNewReplaceBooksExercise();
        }

        private void RandCallNumbersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item
            string selectedItem = (string)randCallNumbersLB.SelectedItem;

            // Add the selected item to the UserOrderLB ListBox
            if (!string.IsNullOrEmpty(selectedItem))
            {
                UserOrderLB.Items.Add(selectedItem);
                btnDeleteLast.Visibility = Visibility.Visible; // Show the "Delete Last" button

                // Highlight the selected item
                ListBoxItem listBoxItem = (ListBoxItem)randCallNumbersLB.ItemContainerGenerator.ContainerFromItem(selectedItem);
                if (listBoxItem != null)
                {
                    listBoxItem.Background = highlightBrush;
                }

                // Check if all items have been selected
                if (UserOrderLB.Items.Count == randCallNumbersLB.Items.Count)
                {
                    // Perform the order check automatically
                    List<string> userOrder = UserOrderLB.Items.Cast<string>().ToList();
                    bool isCorrectOrder = replaceBook.CheckOrder(userOrder);

                    if (isCorrectOrder)
                    {
                        // Set the background color to green
                        UserOrderLB.Background = (SolidColorBrush)Resources["CorrectBackgroundBrush"];

                        ResetHighlight();

                        replaceBook.roundCount++;

                        // Check if all rounds are completed
                        if (replaceBook.roundCount > 2)
                        {
                            ReplaceBooksPB.Value = 100;
                            PopupEndRound.Visibility = Visibility.Visible;

                            replaceBook.roundCount = 1;
                            ThirdAchievePB.Value = 100;
                            replaceBook.exercisesPlayed++;

                            if (replaceBook.exercisesPlayed < 2)
                            {
                                ThirdAchieveNumber.Content = replaceBook.exercisesPlayed + "/1";
                                FirstAchievePB.Value += 10;
                                FirstAchieveNumber.Content = replaceBook.exercisesPlayed + "/10";

                                if (replaceBook.exerciseErrors == 0)
                                {
                                    SecondAchievePB.Value += 20;
                                    replaceBook.errorfreerounds++;
                                    SecondAchieveNumber.Content = replaceBook.errorfreerounds + "/5";
                                }
                            }
                            else
                            {                      
                                FirstAchievePB.Value += 10;
                                FirstAchieveNumber.Content = replaceBook.exercisesPlayed + "/10";

                                if (replaceBook.exerciseErrors == 0)
                                {
                                    SecondAchievePB.Value += 20;
                                    replaceBook.errorfreerounds++;
                                    SecondAchieveNumber.Content = replaceBook.errorfreerounds + "/5";
                                }
                            }                               
                            
                        }
                        else
                        {
                            ReplaceBooksPB.Value += 50;
                            ShowCorrectStuff();                           
                            RoundNumlbl.Content = replaceBook.roundCount + "/2";
                            btnDeleteLast.Visibility = Visibility.Collapsed; // Hide the "Delete Last" button
                            ResetHighlight();
                            StartAnotherReplaceBooksRound();
                        }
                    }
                    else
                    {
                        ResetHighlight();
                        UserOrderLB.Items.Clear();
                        ShowWrongStuff();
                        replaceBook.exerciseErrors++;
                    }
                }
            }
        }

        public void btnDeleteLast_Click(object sender, RoutedEventArgs e)
        {
            if (UserOrderLB.Items.Count > 0)
            {
                // Remove the last item from the UserOrderLB ListBox
                string removedItem = (string)UserOrderLB.Items[UserOrderLB.Items.Count - 1];
                UserOrderLB.Items.RemoveAt(UserOrderLB.Items.Count - 1);

                // Reset the background color for the removed item in randCallNumbersLB
                ListBoxItem listBoxItem = (ListBoxItem)randCallNumbersLB.ItemContainerGenerator.ContainerFromItem(removedItem);
                if (listBoxItem != null)
                {
                    listBoxItem.Background = Brushes.Transparent; // Reset the background color
                }

                // Check if the UserOrderLB is empty after removing
                if (UserOrderLB.Items.Count == 0)
                {
                    // If it's empty, hide the "Delete Last" button
                    btnDeleteLast.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ResetHighlight()
        {
            foreach (var item in randCallNumbersLB.Items)
            {
                ListBoxItem listBoxItem = (ListBoxItem)randCallNumbersLB.ItemContainerGenerator.ContainerFromItem(item);
                if (listBoxItem != null)
                {
                    listBoxItem.Background = Brushes.Transparent; // Set to the default background color
                }
            }
        }

       private void btnEraseAll_Click(object sender, RoutedEventArgs e)
        {
            // Clear items in UserOrderLB
            UserOrderLB.Items.Clear();

            // Hide the "Delete Last" button
            btnDeleteLast.Visibility = Visibility.Collapsed;

            // Unhighlight items in randCallNumbersLB
            foreach (var item in randCallNumbersLB.Items)
            {
                ListBoxItem listBoxItem = (ListBoxItem)randCallNumbersLB.ItemContainerGenerator.ContainerFromItem(item);
                if (listBoxItem != null)
                {
                    listBoxItem.Background = Brushes.Transparent; // Set to the default background color
                }
            }
        }

        private void btnQuitGame_Click(object sender, RoutedEventArgs e)
        {
            UserOrderLB.Items.Clear();

            // Unhighlight items in randCallNumbersLB
            foreach (var item in randCallNumbersLB.Items)
            {
                ListBoxItem listBoxItem = (ListBoxItem)randCallNumbersLB.ItemContainerGenerator.ContainerFromItem(item);
                if (listBoxItem != null)
                {
                    listBoxItem.Background = Brushes.Transparent; // Set to the default background color
                }
            }

            // Hide the "Delete Last" button
            btnDeleteLast.Visibility = Visibility.Collapsed;

            // Show the menu grid and hide the "Replace Books" grid
            MenuGrid.Visibility = Visibility.Visible;
            ReplaceBooksGrid.Visibility = Visibility.Collapsed;
        }

        public void StartNewReplaceBooksExercise()
        {
            replaceBook.exerciseErrors = 0;
            ReplaceBooksPB.Value = 0;
            RoundNumlbl.Content = "1/2";
                randCallNumbersLB.ItemsSource = null;
                UserOrderLB.Items.Clear();
                UserOrderLB.Background = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255));

                replaceBook.callNumbersList = replaceBook.GenerateRandomCallNumbers(10);

                randCallNumbersLB.ItemsSource = replaceBook.callNumbersList;

                randCallNumbersLB.Items.Refresh();
                UserOrderLB.ItemsSource = null;
        }

        public void StartAnotherReplaceBooksRound()
        {
            // Clear the ListBox and reset background color
            UserOrderLB.Items.Clear();
            UserOrderLB.Background = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255));
            replaceBook.callNumbersList = replaceBook.GenerateRandomCallNumbers(10);
            randCallNumbersLB.ItemsSource = replaceBook.callNumbersList;
            randCallNumbersLB.Items.Refresh();
        }

        private void OkButtonEndRoundPop_Click(object sender, RoutedEventArgs e)
        {
            PopupEndRound.Visibility = Visibility.Hidden;
            MenuGrid.Visibility = Visibility.Visible;
            ReplaceBooksGrid.Visibility = Visibility.Collapsed;
        }

        private void btnInfoReplaceBooks_Click(object sender, RoutedEventArgs e)
        {
            HowToPlayReplaceBooks.Visibility = Visibility.Visible;
        }

        private void InfoExitButton_Click(object sender, RoutedEventArgs e)
        {
            HowToPlayReplaceBooks.Visibility= Visibility.Collapsed;
        }



        public void btnIdentifyAreas_Click(object sender, RoutedEventArgs e)
        {
            identifyArea.correctCount = 0;
            MenuGrid.Visibility = Visibility.Hidden;
            IdentifyAreasGrid.Visibility = Visibility.Visible;
            HowToPlayIdentifyAreas.Visibility = Visibility.Visible;

            StartIdentifyAreaRound();
        }

        private void btnCheckAnswerIdentifyArea_Click(object sender, RoutedEventArgs e)
        {
            if (!isCallNumbersOnLeft)
            {
                if (CallNumbersListBox.SelectedItem != null && DescriptionsListBox.SelectedItem != null)
                {
                    string selectedCallNumber = CallNumbersListBox.SelectedItem.ToString();
                    string selectedDescription = DescriptionsListBox.SelectedItem.ToString();

                    if (deweyCategories.deweyDictionary.ContainsKey(selectedCallNumber) &&
                        deweyCategories.deweyDictionary[selectedCallNumber] == selectedDescription)
                    {
                        // Correct match
                        IAroundcount++;
                        MotivateCorrectIconIA.Visibility = Visibility.Visible;
                        MotivateLabelIA.Visibility = Visibility.Visible;
                        timer.Start(); 

                        identifyArea.correctCount++;
                        IdentifyAreasPB.Value += 25;

                        // Highlight the correct answer in green
                        HighlightCorrectAnswer(CallNumbersListBox, selectedCallNumber);
                        HighlightCorrectAnswer(DescriptionsListBox, selectedDescription);
                    }
                    else
                    {
                        // Incorrect match
                        IncorrectIconIA.Visibility = Visibility.Visible;
                        IncorrectLabelIA.Visibility = Visibility.Visible;
                        timer.Start();
                    }

                    CallNumbersListBox.SelectedIndex = -1;
                    DescriptionsListBox.SelectedIndex = -1;
                }
            }
            else
            {
                if (DescriptionsListBox.SelectedItem != null && CallNumbersListBox.SelectedItem != null)
                {
                    string selectedDescription = CallNumbersListBox.SelectedItem.ToString();
                    string selectedCallNumber = DescriptionsListBox.SelectedItem.ToString();

                    if (deweyCategories.deweyDictionary.ContainsKey(selectedCallNumber) &&
                        deweyCategories.deweyDictionary[selectedCallNumber] == selectedDescription)
                    {
                        // Correct match
                        IAroundcount++;
                        MotivateCorrectIconIA.Visibility = Visibility.Visible;
                        MotivateLabelIA.Visibility = Visibility.Visible;
                        timer.Start();

                        identifyArea.correctCount++;
                        IdentifyAreasPB.Value += 25;

                        // Highlight the correct answer in green
                        HighlightCorrectAnswer(DescriptionsListBox, selectedCallNumber);
                        HighlightCorrectAnswer(CallNumbersListBox, selectedDescription);
                    }
                    else
                    {
                        // Incorrect match
                        IncorrectIconIA.Visibility = Visibility.Visible;
                        IncorrectLabelIA.Visibility = Visibility.Visible;
                        timer.Start();
                    }

                    CallNumbersListBox.SelectedIndex = -1;
                    DescriptionsListBox.SelectedIndex = -1;
                }
            }

            //Check if its the end of the round
            if (identifyArea.correctCount == 4)
            {
                PopupEndRoundIA.Visibility = Visibility.Visible;
            }
        }

        private void HighlightCorrectAnswer(ListBox listBox, string correctAnswer)
        {
            // Find the ListBoxItem that contains the correct answer
            foreach (var item in listBox.Items)
            {
                ListBoxItem listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(item);
                if (listBoxItem != null && item.ToString() == correctAnswer)
                {
                    // Highlight the correct answer in green
                    listBoxItem.Background = Brushes.LightBlue;
                }
            }
        }

        public void StartIdentifyAreaRound()
        {
            identifyArea.correctCount = 0;
            IdentifyAreasPB.Value = 0;
            // Get all keys (call numbers) and values (descriptions) from the dictionary
            List<string> allKeys = deweyCategories.deweyDictionary.Keys.ToList();
            List<string> allValues = deweyCategories.deweyDictionary.Values.ToList();

            // Clear existing items in ListBox controls
            CallNumbersListBox.ItemsSource = null;
            DescriptionsListBox.ItemsSource = null;

            if (isCallNumbersOnLeft)
            {
                // Display 4 call numbers on the left and 7 descriptions on the right
                List<string> selectedCallNumbers = identifyArea.GetRandomItems(allKeys, 4);
                List<string> selectedDescriptions = identifyArea.GetRandomItems(allValues, 10);



                CallNumbersListBox.ItemsSource = identifyArea.ShuffleList(selectedCallNumbers);
                DescriptionsListBox.ItemsSource = identifyArea.ShuffleList(selectedDescriptions);
            }
            else
            {
                // Display 4 descriptions on the left and 7 call numbers on the right
                List<string> selectedCallNumbers = identifyArea.GetRandomItems(allKeys, 10);
                List<string> selectedDescriptions = identifyArea.GetRandomItems(allValues, 4);

                CallNumbersListBox.ItemsSource = identifyArea.ShuffleList(selectedDescriptions);
                DescriptionsListBox.ItemsSource = identifyArea.ShuffleList(selectedCallNumbers);
            }

            // Toggle the state for the next round
            isCallNumbersOnLeft = !isCallNumbersOnLeft;
        }

        private void btnFinishIdentBooks_Click(object sender, RoutedEventArgs e)
        {
            IdentifyAreasGrid.Visibility = Visibility.Collapsed;
            MenuGrid.Visibility = Visibility.Visible;

            if(IAroundcount > 0)
            {
                ThirdAchievePB.Value = 100;
                ThirdAchieveNumber.Content = "1/1";
            }
        }

        private void btnIdentifyPopup_Click(object sender, RoutedEventArgs e)
        {
            IdentifyAreaPopup.Visibility = Visibility.Hidden;
            if(identifyArea.correctCount == 4)
            {
                StartIdentifyAreaRound();
            }
        }

        private void btnInfoIdentifyAreas_Click(object sender, RoutedEventArgs e)
        {
            HowToPlayIdentifyAreas.Visibility = Visibility.Visible;
        }

        private void btnQuitGameIdentifyAreas_Click(object sender, RoutedEventArgs e)
        {
            IdentifyAreasGrid.Visibility = Visibility.Hidden;
            MenuGrid.Visibility= Visibility.Visible;
        }

        private void InfoExitButtonIdentifyAreas_Click(object sender, RoutedEventArgs e)
        {
            HowToPlayIdentifyAreas.Visibility = Visibility.Hidden;
        }

        private void OkButtonEndRoundPopAI_Click(object sender, RoutedEventArgs e)
        {
            PopupEndRoundIA.Visibility = Visibility.Hidden;
            StartIdentifyAreaRound();
        }

        public void btnFindCallNumbers_Click(object sender, RoutedEventArgs e)
        {
            FindCallNumPB.Value = 0;
   
            DisplayRandomQuestion();
            FindCallNumbersGrid.Visibility = Visibility.Visible;
        }

        private void btnInfoFindCall_Click(object sender, RoutedEventArgs e)
        {
            HowToPlayFindCall.Visibility = Visibility.Visible;
        }

        private void btnQuitGameFindCall_Click(object sender, RoutedEventArgs e)
        {
            FindCallNumbersGrid.Visibility = Visibility.Hidden;
            MenuGrid.Visibility = Visibility.Visible;
        }

        private void btnFinishFindCall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadDeweyDecimalHierarchy()
        {
            try
            {
                // File path for the json file
                string jsonFilePath = @"C:\Users\micha\Source\Repos\VCNMB\vcnmb-prog7312-2023-poe-MichaelKriel05\PROG_PART_1_LibraryApp\PROG_PART_1_LibraryApp\PROG_PART_1_LibraryApp\Data\hierarchy.json";
                string jsonContent = File.ReadAllText(jsonFilePath);
                deweyDecimalHierarchy = JsonConvert.DeserializeObject<List<Category>>(jsonContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Dewey Decimal hierarchy: {ex.Message}");
            }
        }

        private void DisplayRandomQuestion()
        {
            Random random = new Random();

            int topLevelIndex = random.Next(deweyDecimalHierarchy.Count);
            Category topLevelCategory = deweyDecimalHierarchy[topLevelIndex];

            if (topLevelCategory.Subcategories != null && topLevelCategory.Subcategories.Count > 0)
            {
                int subLevelIndex = random.Next(topLevelCategory.Subcategories.Count);
                Category subLevelCategory = topLevelCategory.Subcategories[subLevelIndex];

                if (subLevelCategory.Subcategories != null && subLevelCategory.Subcategories.Count > 0)
                {
                    int thirdLevelIndex = random.Next(subLevelCategory.Subcategories.Count);
                    currentThirdLevelCategory = subLevelCategory.Subcategories[thirdLevelIndex];

                    lblLowLevel.Content = currentThirdLevelCategory.Title;

                    var topLevelCategories = deweyDecimalHierarchy
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(4)
                        .ToList();

                    if (!topLevelCategories.Contains(topLevelCategory))
                    {
                        topLevelCategories[random.Next(4)] = topLevelCategory;
                    }

                    // Display both call number and description in the ListBox
                    TopLevelListBox.ItemsSource = topLevelCategories.Select(cat => $"{cat.Code} {cat.Title}");
                }
            }
        }

        private void TopLevelListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TopLevelListBox.SelectedItem != null)
            {
                string selectedCategory = TopLevelListBox.SelectedItem.ToString();
                string selectedCode = selectedCategory.Split(' ')[0].Trim();

                int currentFirstDigit = int.Parse(currentThirdLevelCategory.Code.Substring(0, 1));

                // Get the first digit of the selected code
                int selectedFirstDigit = int.Parse(selectedCode.Substring(0, 1));

                if (currentFirstDigit == selectedFirstDigit)
                {
                    MessageBox.Show("Correct! Move to the next question.");
                    FindCallNumPB.Value += 15;

                    // Get the selected category object
                    Category selectedCategoryObject = deweyDecimalHierarchy
                        .FirstOrDefault(cat => cat.Code == selectedCode);

                    if (selectedCategoryObject != null && selectedCategoryObject.Subcategories != null)
                    {
                        // Get second-level categories based on the selected category's second digit
                        int selectedSecondDigit = int.Parse(currentThirdLevelCategory.Code.Substring(1, 1));
                        List<Category> secondLevelCategories = selectedCategoryObject.Subcategories
                            .Where(subCat => subCat.Code.StartsWith($"{selectedFirstDigit}{selectedSecondDigit}"))
                            .ToList();

                        TopLevelListBox.ItemsSource = secondLevelCategories;
                    }

                    // Display a new question or perform other actions for the next round
                    DisplayRandomQuestion();
                }
                else
                {
                    MessageBox.Show("Incorrect. Try again or handle accordingly.");
                }
            }
        }

        private Category FindCategoryByCode(List<Category> categories, string code)
        {
            foreach (var category in categories)
            {
                if (category.Code == code)
                {
                    return category;
                }

                if (category.Subcategories != null)
                {
                    var result = FindCategoryByCode(category.Subcategories, code);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        private void InfoExitButtonFindCall_Click(object sender, RoutedEventArgs e)
        {
            HowToPlayFindCall.Visibility = Visibility.Collapsed;
        }
    }
}
