using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UFCTeams.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UFCTeams
{
    public sealed partial class MainPage : Page
    {
        private List<Team> teams;

        public MainPage()
        {
            this.InitializeComponent();
            LoadTeams();
            LoadWeightClasses();
        }

        // Load Default teams
        private void LoadTeams()
        {
            teams = new List<Team>
            {
                Team.Create("The Eagles",
                    new Fighter("Khabib Nurmagomedov", "Lightweight", true, 29),
                    new Fighter("Islam Makhachev", "Lightweight", false, 27),
                    "Javier Mendez"
                ),
                Team.Create("Bones Brigade",
                    new Fighter("Jon Jones", "Heavyweight", true, 28),
                    new Fighter("Tom Aspinall", "HeavyWeight", false, 15),
                    "Greg Jackson"
                ),
                Team.Create("Stylebenders",
                    new Fighter("Israel Adesanya", "Middleweight", true, 24),
                    new Fighter("Max Holloway", "Featherweight", false, 26),
                    "Eugene Bareman"
                ),
                Team.Create("Hit Heavy",
                    new Fighter("Curtis Blaydes", "Heavyweight", false, 17),
                    new Fighter("Tom Aspinall", "Heavyweight", true, 15),
                    "Dwayne Ludwig"
                )
            };

            foreach (var team in teams)
            {
                TeamComboBox.Items.Add(new ComboBoxItem { Content = team.TeamName, DataContext = team });
            }

            TeamComboBox.Items.Add(new ComboBoxItem { Content = "New Team" });
        }

        // Load default weightclasses
        private void LoadWeightClasses()
        {
            List<string> weightClasses = new List<string>
            {
                "Strawweight", "Flyweight", "Bantamweight", "Featherweight",
                "Lightweight", "Welterweight", "Middleweight",
                "Light Heavyweight", "Heavyweight"
            };

            Fighter1WeightClassComboBox.ItemsSource = weightClasses;
            Fighter2WeightClassComboBox.ItemsSource = weightClasses;
        }

        // Select or type new team in Combobox and clear fields 
        private void TeamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TeamComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Content.ToString() == "New Team")
                {
                    ClearInputFields();
                }
                else
                {
                    var selectedTeam = selectedItem.DataContext as Team;
                    if (selectedTeam != null)
                    {
                        Fighter1TextBox.Text = selectedTeam.Fighters[0].Name;
                        Fighter2TextBox.Text = selectedTeam.Fighters[1].Name;
                        Fighter1WeightClassComboBox.SelectedItem = selectedTeam.Fighters[0].WeightClass;
                        Fighter2WeightClassComboBox.SelectedItem = selectedTeam.Fighters[1].WeightClass;
                        CoachTextBox.Text = selectedTeam.Coach;

                        Fighter1IsChampionCheckBox.IsChecked = selectedTeam.Fighters[0].IsChampion;
                        Fighter2IsChampionCheckBox.IsChecked = selectedTeam.Fighters[1].IsChampion;

                        Fighter1WinsTextBox.Text = selectedTeam.Fighters[0].Wins.ToString();
                        Fighter2WinsTextBox.Text = selectedTeam.Fighters[1].Wins.ToString();
                    }
                }
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");

        private void Fighter1WinsTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            sender.Text = _regex.Replace(sender.Text, "");
        }

        private void SaveTeamButton_Click(object sender, RoutedEventArgs e)
        {
            string teamName = TeamComboBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(teamName) || teamName == "New Team")
            {
                ShowDialog("Invalid Name", "Please enter a valid team name.");
                return;
            }

            try
            {
                var fighter1 = new Fighter(Fighter1TextBox.Text, Fighter1WeightClassComboBox.SelectedItem?.ToString(),
                    Fighter1IsChampionCheckBox.IsChecked ?? false, int.Parse(Fighter1WinsTextBox.Text));

                var fighter2 = new Fighter(Fighter2TextBox.Text, Fighter2WeightClassComboBox.SelectedItem?.ToString(),
                    Fighter2IsChampionCheckBox.IsChecked ?? false, int.Parse(Fighter2WinsTextBox.Text));

                // Check if team exists
                Team existingTeam = teams.Find(t => t.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase));

                if (existingTeam != null)
                {
                    existingTeam.Fighters[0] = fighter1;
                    existingTeam.Fighters[1] = fighter2;
                    existingTeam.Coach = CoachTextBox.Text;

                    ShowDialog("Team Updated", $"{teamName} Successfully updated");
                }
                else
                {
                    if (teams.Exists(t => t.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
                    {
                        ShowDialog("Duplicate Team", $"{teamName} already exists.");
                        return;
                    }

                    // Add new team
                    var newTeam = Team.Create(teamName, fighter1, fighter2, CoachTextBox.Text);
                    teams.Add(newTeam);
                    TeamComboBox.Items.Insert(TeamComboBox.Items.Count - 1, new ComboBoxItem { Content = newTeam.TeamName, DataContext = newTeam });

                    TeamComboBox.SelectedIndex = TeamComboBox.Items.Count - 2;
                    ShowDialog("Team Saved", $"{teamName} has been successfully added.");
                }
            }
            catch (ArgumentException ex)
            {
                ShowDialog("Missing Information", ex.Message);
            }
            catch (FormatException)
            {
                ShowDialog("Invalid Input", "Wins/Losses must be a valid number.");
            }
        }


        private void ClearInputFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearInputFields();
        }

        private void ClearInputFields()
        {
            Fighter1TextBox.Text = string.Empty;
            Fighter2TextBox.Text = string.Empty;
            Fighter1WeightClassComboBox.SelectedIndex = -1;
            Fighter2WeightClassComboBox.SelectedIndex = -1;
            CoachTextBox.Text = string.Empty;
            TeamComboBox.SelectedIndex = -1;
        }

        private void DeleteTeamButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeamComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string teamName = selectedItem.Content.ToString();

                // Search for the selected team in the list
                Team teamToDelete = teams.Find(t => t.TeamName == teamName);
                if (teamToDelete != null)
                {
                    teams.Remove(teamToDelete);
                    TeamComboBox.Items.Remove(selectedItem);

                    ShowDialog("Team Deleted", $"{teamName} has been successfully deleted.");
                }
            }
            else
            {
                ShowDialog("No Selection", "Select a team to delete.");
            }
        }

        // Too display dialogs on events
        private async void ShowDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }
    }
}