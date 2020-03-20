using Syncfusion.XForms.Chat;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SfChatDemo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            InitializeComponent();
        }

        private void SfChat_SendMessage(object sender, SendMessageEventArgs e)
        {
            e.Handled = true;
            e.Message.ShowAuthorName = true;
            e.Message.ShowAvatar = true;
            this.sfChat.Messages.Add(e.Message);
            TextMessage textMessage = new TextMessage() { Author = sfChat.CurrentUser, ShowAvatar = true };
            textMessage.Text = "How are you?";
            this.sfChat.Messages.Add(textMessage);
        }
    }
}