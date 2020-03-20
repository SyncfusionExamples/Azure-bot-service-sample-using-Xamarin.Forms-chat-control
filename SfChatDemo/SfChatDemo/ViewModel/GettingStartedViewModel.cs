using System;
using System.Collections.ObjectModel;
using Syncfusion.XForms.Chat;
using System.ComponentModel;

namespace SfChatDemo
{
    public class SfChatDemoViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<object> messages;

        /// <summary>
        /// current user of chat.
        /// </summary>
        private Author currentUser;

        public SfChatDemoViewModel()
        {
            this.messages = new ObservableCollection<object>();
            this.currentUser = new Author() { Name = "Nancy", Avatar = "People_Circle16.png" };
            this.GenerateMessages();
        }

        /// <summary>
        /// Gets or sets the group message conversation.
        /// </summary>
        public ObservableCollection<object> Messages
        {
            get
            {
                return this.messages;
            }

            set
            {
                this.messages = value;
            }
        }

        /// <summary>
        /// Gets or sets the current author.
        /// </summary>
        public Author CurrentUser
        {
            get
            {
                return this.currentUser;
            }
            set
            {
                this.currentUser = value;
                RaisePropertyChanged("CurrentUser");
            }
        }

        /// <summary>
        /// Property changed handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when property is changed.
        /// </summary>
        /// <param name="propName">changed property name</param>
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void GenerateMessages()
        {
            this.messages.Add(new TextMessage()
            {
                Author = new Author() { Name = "Andrea", Avatar = "People_Circle2.png" },
                Text = "Oh! That's great.",
                ShowAvatar = true,
            });


            this.messages.Add(new TextMessage()
            {
                Author = new Author() { Name = "Margaret", Avatar = "People_Circle7.png" },
                Text = "What kind of application is it and when are we going to launch?",
                ShowAvatar = true,
            });
        }
    }
}
