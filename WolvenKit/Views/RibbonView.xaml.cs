﻿using System.ComponentModel;
using Catel.IoC;
using Fluent;
using Orchestra;
using Orchestra.Services;
using System.Windows;
using Catel;
using Catel.Data;
using WolvenKit.Views.AssetBrowser;

namespace WolvenKit.Views
{
    public partial class RibbonView
    {
        public RibbonView()
        {
            InitializeComponent();

            ribbon.AddAboutButton();
            


        }

        protected override void OnViewModelChanged()
        {
            base.OnViewModelChanged();
#pragma warning disable WPF0041
            backstageTabControl.DataContext = ViewModel;
#pragma warning restore WPF0041
        }

        protected override void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnViewModelPropertyChanged(e);

            if (!(e is AdvancedPropertyChangedEventArgs property))
                return;

            switch (property.PropertyName)
            {
                case "SelectedTheme":
                    if (property.NewValue is string themename)
                        ControlzEx.Theming.ThemeManager.Current.ChangeTheme(Application.Current, themename);
                    break;
                default:
                    break;
            }
        }

        private void ShowStartScreen_OnClick(object sender, RoutedEventArgs e) // Convert me to MVVM
        {
            
 
            this.startScreen.SetCurrentValue(StartScreen.ShownProperty, false);
                this.startScreen.SetCurrentValue(Backstage.IsOpenProperty, true);
        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e) // Convert me to MVVM
        {
            AssetBrowserView assetBrowser = new AssetBrowserView();
            assetBrowser.Show(); 
        }
    }
}