﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bti.Babble.Metro.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bti.Babble.Metro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StoryPage : Bti.Babble.Metro.Common.LayoutAwarePage
    {
        public StoryPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var story = (StoryEvent)e.Parameter;
            this.DataContext = story;
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            base.GoBack(sender, e);
        }

        private void ReadMore_Click(object sender, RoutedEventArgs e)
        {
            var button = (HyperlinkButton) e.OriginalSource;
            var link = button.Tag.ToString();
            if (link.Length > 0)
            {
                Windows.System.Launcher.LaunchUriAsync(new Uri(link));
            }
        }
    }
}
