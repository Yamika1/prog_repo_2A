using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Media;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;


namespace POEWPF_Programming
{
    public partial class MainWindow : Window
    {
        private readonly SpeechSynthesizer synth = new();
        private readonly List<string> chatHistory = new();
        private readonly Random random = new();
        private readonly List<TaskItem> tasks = new();
        private readonly List<string> activityLog = new();

        private string username = string.Empty;
        private bool nameCaptured = false;
        private int nextId = 1;

        // Static content
        private readonly string[] whyCybersecurityAnswers =
        {
    "Cybersecurity safeguards personal data, including financial info and health records.",
    "Cybersecurity ensures that online transactions are safe from interception and manipulation.",
    "Implementing cybersecurity measures helps ensure business productivity despite threats.",
    "Cybersecurity protects systems from attacks that could impact public safety and well-being."
};

        private readonly Dictionary<string, string> definitions = new()
{
    { "cybersecurity", "The practice of protecting digital information, networks, and systems from unauthorized access." },
    { "phishing", "Phishing is online fraud where attackers steal data using fake emails or links." },
    { "password", "Password safety involves using strong, unique credentials and enabling two-factor authentication." },
    { "scam", "A dishonest scheme to trick or defraud people." },
    { "privacy", "The right to control what personal data is collected and how it's used." },
    { "safe browsing", "Practices that minimize risk when using the internet." }
};

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            synth.Volume = 100;
            synth.Rate = 0;
            PlayGreeting();
            BotSpeak("Hello! Welcome to your cybersecurity awareness chatbot. What is your name?");
        }

        // Button Handler for Sending Messages
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInputTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
                return;

            AddMessage($"You: {input}");
            chatHistory.Add($"You: {input}");

            input = input.ToLowerInvariant();

            if (!nameCaptured)
            {
                try
                {
                    ValidateName(input);
                    username = input;
                    nameCaptured = true;

                    BotSpeak($"Hi {username}, I'm here to help you stay safe online.");
                    ShowAsciiArtInBox();
                    ShowWelcomeAsciiArt();
                    DisplayTipOfTheDay();
                }
                catch (Exception ex)
                {
                    BotSpeak(ex.Message);
                }
            }
            else
            {
                HandleUserInput(input);
            
            }

            UserInputTextBox.Clear();
        }

        // Core Input Handler
        private void HandleUserInput(string input)
        {
            if (HandleExcitementConversation(input)) return;

            if (input.Contains("why is cybersecurity important"))
            {
                RespondToCybersecurityImportance();
            }
            else if (definitions.ContainsKey(input))
            {
                BotSpeak(definitions[input]);
            }
            else if (input.StartsWith("tell me more on"))
            {
                string topic = input.Replace("tell me more on", "").Trim();
                ProvideMoreInfo(topic);
            }
            else if (input.Contains("i'm worried about"))
            {
                HandleWorries(input);
            }
            else if (input.Contains("save chat"))
            {
                SaveChatHistory();
            }
            else if (new[] { "1", "2", "3", "4" }.Contains(input))
            {
                ShowCybersecurityDefinition(input);
            }
            else
            {
                RespondToUser(input);
            }
        }
        private void TakeQuizButton_Click(object sender, RoutedEventArgs e)
        {
            RunCyberQuiz(); 
        }
        private void RunCyberQuiz()
        {
            string fileName = "Quiz game prog6211.txt";
            if (!File.Exists(fileName))
            {
                BotSpeak("Quiz file not found.");
                return;
            }

            string[] lines = File.ReadAllLines(fileName);
            var questions = new List<string>();
            var optionsList = new List<List<string>>();
            var correctAnswersList = new List<List<int>>();

            for (int i = 0; i < lines.Length;)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) { i++; continue; }

                if (char.IsDigit(line[0]) && line.Contains("."))
                {
                    questions.Add(line.Substring(line.IndexOf('.') + 1).Trim());
                    var options = new List<string>();
                    var correct = new List<int>();
                    int j = 1;

                    while (i + j < lines.Length && !string.IsNullOrWhiteSpace(lines[i + j]))
                    {
                        string optionLine = lines[i + j].Trim();
                        if (optionLine.StartsWith(">"))
                        {
                            correct.Add(options.Count + 1);
                            optionLine = optionLine.Substring(1).Trim();
                        }
                        options.Add(optionLine);
                        j++;
                    }

                    optionsList.Add(options);
                    correctAnswersList.Add(correct);
                    i += j;
                }
                else { i++; }
            }

            int score = 0;

            for (int q = 0; q < questions.Count; q++)
            {
                string questionBlock = $"\nQuestion {q + 1}: {questions[q]}\n";
                for (int i = 0; i < optionsList[q].Count; i++)
                {
                    questionBlock += $"{i + 1}. {optionsList[q][i]}\n";
                }

                string input = Microsoft.VisualBasic.Interaction.InputBox(questionBlock + "\nEnter your answer (number):", "Cybersecurity Quiz", "1");

                if (int.TryParse(input, out int selected))
                {
                    if (correctAnswersList[q].Contains(selected))
                    {
                        BotSpeak("Correct!");
                        LogActivity($"Answered quiz question {q + 1} correctly.");
                        score++;
                    }
                    else
                    {
                        BotSpeak("Incorrect.");
                        LogActivity($"Answered quiz question {q + 1} incorrectly.");
                    }
                }
                else
                {
                    BotSpeak("Invalid input.");
                }
            }

            BotSpeak($"Quiz complete! Your score: {score} out of {questions.Count}");
        }
        // Respond to General User Input
        private void RespondToUser(string input)
        {
            if (input.Contains("hello") || input.Contains("hi"))
            {
                BotSpeak($"Hello {username}!");
            }
            else if (input.Contains("how are you"))
            {
                BotSpeak("I'm good, thank you! How can I help you today?");
            }
            else if (input.Contains("assistance") || input.Contains("what can you provide assistance on"))
            {
                BotSpeak("I can assist with information on password safety, phishing, and safe internet browsing.");
            }
            else if (input.Contains("safe browsing"))
            {
                BotSpeak("Safe browsing helps protect you from malicious websites and online threats.");
            }
            else if (input.Contains("phishing"))
            {
                BotSpeak("Phishing tricks users into revealing personal info via fake emails or websites.");
            }
            else if (input.Contains("password safety"))
            {
                BotSpeak("Use strong, unique passwords and enable two-factor authentication.");
            }
            else if (input.Contains("exit") || input.Contains("quit"))
            {
                BotSpeak("You typed 'exit', but I’ll stay right here if you still need me!");
            }
            else
            {
                BotSpeak("Sorry, I didn't understand that. Please try asking something else.");
            }
        }

        // Handles Excitement, Curiosity, and Specific Passionate Statements
        private bool HandleExcitementConversation(string input)
        {
            if (input.Contains("exciting topics") || input.Contains("interests me"))
            {
                BotSpeak("There are so many exciting topics related to cybersecurity!");
                return true;
            }

            if (input.Contains("what cybersecurity topics are there") || input.Contains("topics that interest me"))
            {
                BotSpeak("Cybersecurity, phishing, scams, malware, and more. Which one excites you the most?");
                return true;
            }

            if (input.Contains("i am passionate about learning about phishing attacks"))
            {
                BotSpeak("Great, I'll remember that as a topic for later.");
                return true;
            }

            if (input.Contains("what is cybersecurity"))
            {
                BotSpeak("Cybersecurity involves protecting systems, networks, and programs from digital attacks.");
                return true;
            }

            if (input.Contains("dive into the topic of phishing attacks"))
            {
                BotSpeak("Yes sure, let's get right into it. Tell me one fact.");
                return true;
            }

            if (input.Contains("phishing attacks accounted for 36%"))
            {
                BotSpeak("That is quite an astonishing fact. Thanks for sharing!");
                return true;
            }

            if (input.Contains("exit") || input.Contains("quit"))
            {
                BotSpeak("You typed 'exit', but I’ll stay right here if you still need me!");
                return true;
            }

            return false;
        }

        private void ShowAsciiArtInBox()
        {
            string asciiArt = @"
   ____      _                                        _ _              
  / ___|   _| |__   ___ _ __ ___  ___  ___ _   _ _ __(_) |_ _   _      
 | |  | | | | '_ \ / _ \ '__/ __|/ _ \/ __| | | | '__| | __| | | |     
 | |__| |_| | |_) |  __/ |  \__ \  __/ (__| |_| | |  | | |_| |_| |     
  \____\__, |_.__/ \___|_|  |___/\___|\___|\__,_|_|  |_|\__|\__, |     
   __ _|___/   ____ _ _ __ ___ _ __   ___  ___ ___  | __ )  |___/ |_   
  / _` \ \ /\ / / _` | '__/ _ \ '_ \ / _ \/ __/ __| |  _ \ / _ \| __|  
 | (_| |\ V  V / (_| | | |  __/ | | |  __/\__ \__ \ | |_) | (_) | |_   
  \__,_| \_/\_/ \__,_|_|  \___|_| |_|\___||___/___/ |____/ \___/ \__|  
";
            AsciiArtBox.Text = asciiArt;
        }

        private void ShowWelcomeAsciiArt()
        {
            string asciiArt = @"
 __          __  _                            _          _   _                 
 \ \        / / | |                          | |        | | | |                
  \ \  /\  / /__| | ___ ___  _ __ ___   ___  | |_ ___   | |_| |__   ___        
   \ \/  \/ / _ \ |/ __/ _ \| '_ ` _ \ / _ \ | __/ _ \  | __| '_ \ / _ \       
    \  /\  /  __/ | (_| (_) | | | | | |  __/ | || (_) | | |_| | | |  __/       
     \/  \/ \___|_|\___\___/|_| |_| |_|\___|  \__\___/   \__|_| |_|\___|       
";
            AddMessage(asciiArt);
        }
      
      
        private void ShowCybersecurityDefinition(string choice)
        {
            string definition = choice switch
            {
                "1" => "Cybersecurity: Cybersecurity refers to the measures taken to defend digital devices, networks, and confidential information against cyber threats...",
                "2" => "Phishing: Phishing is a type of online scam where cybercriminals seek to obtain your sensitive information...",
                "3" => "Password protection serves as an access control method designed to safeguard sensitive information...",
                "4" => "Safe Browsing: The practice of navigating the internet in a way that minimizes the risk of malware, phishing, and other online threats.",
                _ => "Invalid choice. Please select a valid number (1–4)."
            };
            BotSpeak(definition);
        }

        private void RespondToCybersecurityImportance()
        {
            string response = whyCybersecurityAnswers[random.Next(whyCybersecurityAnswers.Length)];
            BotSpeak(response);
        }

        private void ProvideMoreInfo(string topic)
        {
            topic = topic.ToLower();
            string response = topic switch
            {
                "cybersecurity" => "Cybersecurity protects systems, networks, and programs from digital attacks...",
                "phishing" => "Phishing is a form of online fraud in which hackers attempt to steal your private information...",
                "password safety" => "Password safety involves creating strong, unique passwords and enabling two-factor authentication...",
                _ => "Invalid topic. Please try 'cybersecurity', 'phishing', or 'password safety'."
            };
            BotSpeak(response);
        }

        private void HandleWorries(string concern)
        {
            if (concern.Contains("phishing"))
                BotSpeak("Use strong passwords, enable two-factor auth, and avoid clicking suspicious links.");
            else if (concern.Contains("cybersecurity attacks"))
                BotSpeak("Keep your software updated and use a reliable antivirus tool.");
            else if (concern.Contains("scams"))
                BotSpeak("Always verify emails or messages before clicking links or providing information.");
            else
                BotSpeak("Thanks for sharing your concern. Be cautious online and let me know how I can help.");
        }

        private void BotSpeak(string message)
        {
            AddMessage($"Bot: {message}");
            synth.SpeakAsync(message);
        }

        private void AddMessage(string message)
        {
            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5)
            };
            ChatPanel.Children.Add(textBlock);
        }

        private void DisplayTipOfTheDay()
        {
            string[] tips =
            {
                "Use multi-factor authentication.",
                "Verify the sender’s email before clicking any links.",
                "Avoid using public Wi-Fi for sensitive tasks."
            };

            string tip = tips[random.Next(tips.Length)];
            BotSpeak($"Security Tip of the Day: {tip}");
        }

        private void ValidateName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Name cannot be empty.");
            if (input.Length < 2)
                throw new ArgumentException("Name must be at least 2 characters long.");
            foreach (char c in input)
            {
                if (!char.IsLetter(c) && c != ' ')
                    throw new ArgumentException("Name can only contain letters and spaces.");
            }
        }

        private void SaveChatHistory()
        {
            try
            {
                File.WriteAllText("chat_history.txt", string.Join(Environment.NewLine, chatHistory));
                BotSpeak("Chat history saved.");
            }
            catch (Exception ex)
            {
                BotSpeak($"Failed to save chat history: {ex.Message}");
            }
        }

        private void PlayGreeting()
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Greeting.wav");
                if (File.Exists(path))
                {
                    var player = new SoundPlayer(path);
                    player.Play();
                }
                else
                {
                    BotSpeak("Greeting audio not found.");
                }
            }
            catch
            {
                BotSpeak("Error playing greeting audio.");
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            DateTime? reminder = null;

            if (DateTime.TryParseExact(ReminderTextBox.Text.Trim(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedReminder))
            {
                reminder = parsedReminder;
            }

            var task = new TaskItem
            {
                Id = nextId++,
                Title = title,
                Description = description,
                ReminderDateTime = reminder,
                IsCompleted = false
            };

            tasks.Add(task);
            LogActivity($"Added task: {title}");
            RefreshTaskList();
        }

        private void RefreshTaskList()
        {
            TasksPanel.Children.Clear();

            foreach (var task in tasks)
            {
                var stack = new StackPanel
                {
                    Margin = new Thickness(5),
                    Orientation = Orientation.Vertical
                };

                stack.Children.Add(new TextBlock
                {
                    Text = $"[{task.Id}] {task.Title} - {(task.IsCompleted ? "Completed" : "Pending")}"
                });

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

                var completeBtn = new Button { Content = "Complete", Tag = task.Id, Margin = new Thickness(0, 0, 5, 0) };
                completeBtn.Click += CompleteButton_Click;

                var deleteBtn = new Button { Content = "Delete", Tag = task.Id, Margin = new Thickness(0, 0, 5, 0) };
                deleteBtn.Click += DeleteButton_Click;

                var viewDetailsBtn = new Button { Content = "View Details", Tag = task.Id };
                viewDetailsBtn.Click += ViewDetailsButton_Click;

                buttonPanel.Children.Add(completeBtn);
                buttonPanel.Children.Add(deleteBtn);
                buttonPanel.Children.Add(viewDetailsBtn);

                stack.Children.Add(buttonPanel);
                TasksPanel.Children.Add(stack);
            }
        }
    

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var task = tasks.Find(t => t.Id == id);
            if (task != null && !task.IsCompleted)
            {
                task.IsCompleted = true;
                LogActivity($"Completed task: {task.Title}");
                RefreshTaskList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var task = tasks.Find(t => t.Id == id);
            if (task != null)
            {
                tasks.Remove(task);
                LogActivity($"Deleted task: {task.Title}");
                RefreshTaskList();
            }
        }
        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var task = tasks.Find(t => t.Id == id);

            if (task != null)
            {
                string details = $"Task ID: {task.Id}\n" +
                                 $"Title: {task.Title}\n" +
                                 $"Description: {task.Description}\n" +
                                 $"Status: {(task.IsCompleted ? "Completed" : "Pending")}\n" +
                                 $"Reminder: {(task.ReminderDateTime.HasValue ? task.ReminderDateTime.Value.ToString("yyyy-MM-dd HH:mm") : "None")}";

                MessageBox.Show(details, "Task Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
   

        private void LogActivity(string action)
        {
            string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {action}";
            activityLog.Add(log);
            ActivityLogTextBox.Text += log + Environment.NewLine;
        }
    }

}

public class TaskItem
    {
        public int Id;
        public string Title;
        public string Description;
        public bool IsCompleted;
        public DateTime? ReminderDateTime;

        public override string ToString()
        {
            return $"[{Id}] {Title} - {(IsCompleted ? "Completed" : "Pending")}" +
                   (ReminderDateTime.HasValue ? $" (Reminder: {ReminderDateTime.Value})" : "") +
                   $"\n    {Description}";
        }
    }

