# Azure-bot-service-sample-using-Xamarin.Forms-chat-control
This sample explains how to connect to and configure your Azure bot service in a Xamarin.Forms application using SfChat control

## Sample

```xaml
    public class BotService
    {
        private HttpClient httpClient;

        private Conversation conversation;

        private string BotBaseAddress = "https://directline.botframework.com/v3/directline/conversations/";

        private string directLineKey = "XYeLq1aytPw.4wbtMs2r7XEzdkG2_wyxGpP676wpfFS_hSaSJW8IjQg";

        private string watermark = string.Empty;

        public BotService(FlightBookingViewModel viewModel)
        {
            this.ViewModel = viewModel;
            InitializeHttpConnection();
        }

        internal FlightBookingViewModel ViewModel { get; set; }

        internal bool CheckInternetConnection()
        {
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                this.ViewModel.IsConnectionNotEstablished = false;
                return true;
            }
            else
            {
                this.ViewModel.ShowBusyIndicator = false;
                this.ViewModel.IsConnectionNotEstablished = true;
                return false;
            }
        }

        internal async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                this.ViewModel.IsConnectionNotEstablished = false;
                if (this.conversation == null || (this.conversation != null && string.IsNullOrEmpty(this.conversation.ConversationId)))
                {
                    this.ViewModel.ShowBusyIndicator = true;
                    SetupConversation();
                }
                else
                {
                    await this.ReadMessageFromBot();
                    this.ViewModel.ShowBusyIndicator = false;
                }
            }
            else
            {
                this.ViewModel.ShowBusyIndicator = false;
                this.ViewModel.IsConnectionNotEstablished = true;
            }
        }
        
        internal void SendMessageToBot(string text)
        {
            Activity activity = new Activity()
            {
                From = new ChannelAccount()
                {
                    Id = this.ViewModel.CurrentUser.Name
                },

                Text = text,
                Type = "message"
            };

            PostActvityToBot(activity);
        }

        internal async Task ReadMessageFromBot()
        {
            try
            {
                string conversationUrl = this.BotBaseAddress + this.conversation.ConversationId + "/activities?watermark=" + this.watermark;
                using (HttpResponseMessage messagesReceived = await this.httpClient.GetAsync(conversationUrl, HttpCompletionOption.ResponseContentRead))
                {
                    string messagesReceivedData = await messagesReceived.Content.ReadAsStringAsync();
                    ActivitySet messagesRoot = JsonConvert.DeserializeObject<ActivitySet>(messagesReceivedData);

                    if (messagesRoot != null)
                    {
                        this.watermark = messagesRoot.Watermark;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            foreach (Activity activity in messagesRoot.Activities)
                            {
                                if (activity.From.Id == "ChatBot_Testing_Syncfusion" && activity.Type == "message")
                                {
                                    this.ProcessBotReplyAndAddMessage(activity);
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception while reading Bot activity. exception message - {0}", ex.Message);
            }

            this.ViewModel.ShowTypingIndicator = false;
            this.ViewModel.ShowBusyIndicator = false;
        }

        private void InitializeHttpConnection()
        {
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(this.BotBaseAddress);
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.directLineKey);
            this.httpClient.Timeout = Timeout.InfiniteTimeSpan;

            if (CheckInternetConnection())
            {
                SetupConversation();
            }
        }

        private async void SetupConversation()
        {
            HttpContent contentPost = new StringContent(JsonConvert.SerializeObject(new Conversation()), Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await this.httpClient.PostAsync("/v3/directline/conversations", contentPost);
                if (response.IsSuccessStatusCode)
                {
                    string conversationInfo = await response.Content.ReadAsStringAsync();
                    this.conversation = JsonConvert.DeserializeObject<Conversation>(conversationInfo);
                    await Task.Delay(2000);

                    Activity activity = new Activity();
                    activity.From = new ChannelAccount()
                    {
                        Id = ViewModel.CurrentUser.Name,
                        Name = ViewModel.CurrentUser.Name,
                    };

                    activity.Type = "add";
                    activity.Action = "add";
                    this.PostActvityToBot(activity);
                }
            }
            catch { }
        }

        private async void PostActvityToBot(Activity activity)
        {
            StringContent contentPost = new StringContent(JsonConvert.SerializeObject(activity), Encoding.UTF8, "application/json");
            string conversationUrl = this.BotBaseAddress + this.conversation.ConversationId + "/activities";

            try
            {
                await this.httpClient.PostAsync(conversationUrl, contentPost);
                await this.ReadMessageFromBot();
            }
            catch { }
        }

        private void ProcessBotReplyAndAddMessage(Activity activity)
        {
            if (!string.IsNullOrEmpty(activity.Text))
            {
                if (activity.Text == "What else can I do for you?")
                {
                    return;
                }

                if (activity.Text == "When are you planning to travel?" || activity.Text == "Oops ! This doesn’t seem to be a valid date. Please select a valid date.")
                {
                    this.AddCalendarMessage(activity.Text);
                }
                else
                {
                    this.AddTextMessage(activity);
                }
            }
        }

        private void AddTextMessage(Activity activity)
        {
            TextMessage message = new TextMessage();
            message.Text = activity.Text;
            message.Author = this.ViewModel.Bot;
            message.DateTime = DateTime.Now;

            if (activity.SuggestedActions != null && activity.SuggestedActions.Actions.Count > 0)
            {
                ChatSuggestions suggestions = new ChatSuggestions();
                var suggestionItems = new ObservableCollection<ISuggestion>();
                foreach (CardAction action in activity.SuggestedActions.Actions)
                {
                    var suggestion = new Suggestion();
                    suggestion.Text = action.Title;
                    suggestionItems.Add(suggestion);
                }

                suggestions.Items = suggestionItems;
                message.Suggestions = suggestions;
            }

            ViewModel.Messages.Add(message);
        }

    }

```

## Requirements to run the demo

To run the demo, refer to [System Requirements for Xamarin](https://help.syncfusion.com/xamarin/system-requirements)

## Troubleshooting

### Path too long exception

If you are facing path too long exception when building this example project, close Visual Studio and rename the repository to short and build the project.
