﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Bti.Babble.Dashboard.Model;
using System.Net.Http;
using System.Xml;
using System.Threading.Tasks;
using Windows.Storage;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace Bti.Babble.Dashboard.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : Bti.Babble.Dashboard.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        private ObservableCollection<BabbleEvent> events;
        
        public ObservableCollection<BabbleEvent> BabbleEvents
        {
            get { return this.events; }
            set
            {
                this.events = value;
                OnPropertyChanged("BabbleEvents");
            }
        }

        public async void LoadBabbleEvents()
        {
            var query = this.Subtitle;
            this.BabbleEvents = new ObservableCollection<BabbleEvent>();
            var client = new HttpClient();
            var response = await client.GetAsync("http://prod.bti.tv/babble/service.svc/xml/twitter?rpp=48&q=" + System.Net.WebUtility.UrlEncode(query));
            if (response.IsSuccessStatusCode)
            {
                using (var stream = response.Content.ReadAsStreamAsync().Result)
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "event")
                            {
                                var evt = BabbleEvent.CreateFromXmlReader(reader);
                                LoadBabbleEventImage(evt);
                                BabbleEvents.Add(evt);
                            }
                        }
                    }
                }
            }
        }

        private async void LoadBabbleEventImage(BabbleEvent evt)
        {
            var uri = new Uri(evt.Image);
            evt.ImageSource = new BitmapImage(uri);
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class SampleDataSource
    {
        private ObservableCollection<SampleDataGroup> _itemGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> ItemGroups
        {
            get { return this._itemGroups; }
        }

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                    "@NBCTheVoice",
                    "Group Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group1.Items.Add(new SampleDataItem("Group-1-Item-1",
                    "Tweets from @NBCTheVoice",
                    "from:NBCTheVoice",
                    "Assets/TheVoice.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-2",
                    "Tweets to @NBCTheVoice",
                    "to:NBCTheVoice",
                    "Assets/TheVoice.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-3",
                    "Tweets that mention @NBCTheVoice",
                    "NBCTheVoice -to:NBCTheVoice -from:NBCTheVoice -@NBCTheVoice",
                    "Assets/TheVoice.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-4",
                    "Tweets that mention @NBCTheVoice without links",
                    "NBCTheVoice -to:NBCTheVoice -from:NBCTheVoice -@NBCTheVoice -filter:links",
                    "Assets/TheVoice.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group1));
            group1.Items.Add(new SampleDataItem("Group-1-Item-5",
                    "Shared picutres about @NBCTheVoice",
                    "“NBCTheVoice” twitpic OR yfrog OR post.ly OR twitgoo OR pikchur filter:links",
                    "Assets/TheVoice.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group1));
            this.ItemGroups.Add(group1);

            var group2 = new SampleDataGroup("Group-2",
                    "Influencers",
                    "Group Subtitle: 2",
                    "Assets/LightGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group2.Items.Add(new SampleDataItem("Group-2-Item-1",
                    "Tweets from Adam Levine",
                    "from:adamlevine",
                    "Assets/adam-levine.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-2",
                    "Tweets from Blake Shelton",
                    "from:blakeshelton",
                    "Assets/blake-shelton.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-3",
                    "Tweets from Cell Lo Green",
                    "from:ceelogreen",
                    "Assets/cee-lo-green.png",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group2));
            group2.Items.Add(new SampleDataItem("Group-2-Item-4",
                    "Tweets from Christina Aguilera",
                    "from:TheRealXtina",
                    "Assets/christina-aguilera.png",
                    "Item Description: Pellentsesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group2));
            this.ItemGroups.Add(group2);

            var group3 = new SampleDataGroup("Group-3",
                    "Facebook Wall",
                    "Group Subtitle: 3",
                    "Assets/MediumGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group3.Items.Add(new SampleDataItem("Group-3-Item-1",
                    "Get a glimpse behind the scenes as #TeamAdam and #TeamCeeLo prepared for their big night this week. This is THE VOICE (ahem, backstage): http://bit.ly/IGfRes",
                    "148 comments",
                    "Assets/TheVoice_Facebook.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group3));
            group3.Items.Add(new SampleDataItem("Group-3-Item-1",
                    "#TeamCeeLo and #TeamAdam blew us away this week, and we couldn't be more proud of each and every artist! Their live performance songs are now available on iTunes, and they are amazing. Check them out, and let us know your favorites! http://itunes.com/thevoice",
                    "392 comments",
                    "Assets/TheVoice_Facebook.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group3));
            group3.Items.Add(new SampleDataItem("Group-3-Item-2",
                    "Results are in! Did America get it right? Did the coaches? Discuss.",
                    "3149 comments",
                    "Assets/TheVoice_Facebook.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group3));
            this.ItemGroups.Add(group3);

            var group4 = new SampleDataGroup("Group-4",
                    "#TheVoice",
                    "Group Subtitle: 3",
                    "Assets/MediumGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group4.Items.Add(new SampleDataItem("Group-4-Item-1",
                    "#TheVoice Tweets",
                    "#TheVoice",
                    "Assets/TheVoice_Twitter.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group4));
            group4.Items.Add(new SampleDataItem("Group-4-Item-2",
                    "#TheVoice Filtered Tweets",
                    "#TheVoice filter:links -RT -via -job -jobs",
                    "Assets/TheVoice_Twitter.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group4));
            group4.Items.Add(new SampleDataItem("Group-4-Item-3",
                    "#TheVoice Filtered Near New York",
                    "#TheVoice filter:links -RT -via -job -jobs near:ny",
                    "Assets/TheVoice_Twitter.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group4));
            group4.Items.Add(new SampleDataItem("Group-4-Item-4",
                    "#TheVoice Filtered Near Los Angeles",
                    "#TheVoice filter:links -RT -via -job -jobs near:la",
                    "Assets/TheVoice_Twitter.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group4));
            group4.Items.Add(new SampleDataItem("Group-4-Item-5",
                    "#TheVoice Filtered Near San Francisco",
                    "#TheVoice filter:links -RT -via -job -jobs near:sf",
                    "Assets/TheVoice_Twitter.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group4));
            group4.Items.Add(new SampleDataItem("Group-4-Item-6",
                    "#TheVoice Links Only",
                    "#TheVoice filter:links",
                    "Assets/TheVoice_Twitter.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group4));
            group4.Items.Add(new SampleDataItem("Group-4-Item-7",
                    "Shared picutres about #TheVoice",
                    "“NBCTheVoice” twitpic OR yfrog OR post.ly OR twitgoo OR pikchur filter:links",
                    "Assets/TheVoice_Twitter.jpg",
                    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                    ITEM_CONTENT,
                    group4));
            this.ItemGroups.Add(group4);
        }
    }
}
