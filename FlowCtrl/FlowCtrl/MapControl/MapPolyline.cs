// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © Clemens Fischer 2012-2013
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
#if NETFX_CORE
using Windows.Foundation;
using Windows.UI.Xaml;
#else
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
#endif

namespace MapControl
{
    /// <summary>
    /// A polyline or polygon created from a collection of Locations.
    /// </summary>
    public partial class MapPolyline : MapPath, ICommandSource
    {
#if NETFX_CORE
        // For WinRT, the Locations dependency property type is declared as object
        // instead of IEnumerable. See http://stackoverflow.com/q/10544084/1136211
        private static readonly Type LocationsPropertyType = typeof(object);
#else
        private static readonly Type LocationsPropertyType = typeof(IEnumerable<Location>);
#endif
        public static readonly DependencyProperty LocationsProperty = DependencyProperty.Register(
            "Locations", LocationsPropertyType, typeof(MapPolyline),
            new PropertyMetadata(null, LocationsPropertyChanged));

        public static readonly DependencyProperty IsClosedProperty = DependencyProperty.Register(
            "IsClosed", typeof(bool), typeof(MapPolyline),
            new PropertyMetadata(false, (o, e) => ((MapPolyline)o).UpdateData()));

        /// <summary>
        /// Gets or sets the locations that define the polyline points.
        /// </summary>
#if !NETFX_CORE
        [TypeConverter(typeof(LocationCollectionConverter))]
#endif
        public IEnumerable<Location> Locations
        {
            get { return (IEnumerable<Location>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the polyline is closed, i.e. is a polygon.
        /// </summary>
        public bool IsClosed
        {
            get { return (bool)GetValue(IsClosedProperty); }
            set { SetValue(IsClosedProperty, value); }
        }

        private void LocationCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateData();
        }

        private static void LocationsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var mapPolyline = (MapPolyline)obj;
            var oldCollection = e.OldValue as INotifyCollectionChanged;
            var newCollection = e.NewValue as INotifyCollectionChanged;

            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= mapPolyline.LocationCollectionChanged;
            }

            if (newCollection != null)
            {
                newCollection.CollectionChanged += mapPolyline.LocationCollectionChanged;
            }

            mapPolyline.UpdateData();
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        "Command",
        typeof(ICommand),
        typeof(MapPolyline),
        new PropertyMetadata((ICommand)null,
        new PropertyChangedCallback(CommandChanged)));

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapPolyline cs = (MapPolyline)d;
            cs.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        // Add a new command to the Command Property. 
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers. 
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }
            AddCommand(oldCommand, newCommand);
        }

        // Remove an old command from the Command Property. 
        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        // Add the command. 
        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = new EventHandler(CanExecuteChanged);
            canExecuteChangedHandler = handler;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += canExecuteChangedHandler;
            }
        }
        EventHandler canExecuteChangedHandler;

        private void CanExecuteChanged(object sender, EventArgs e)
        {

            if (this.Command != null)
            {
                RoutedCommand command = this.Command as RoutedCommand;

                // If a RoutedCommand. 
                if (command != null)
                {
                    if (command.CanExecute(CommandParameter, CommandTarget))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
                // If a not RoutedCommand. 
                else
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
            }
        }

        void ExecuteCommand()
        {
            if (this.Command != null)
            {
                RoutedCommand command = Command as RoutedCommand;
                if (command != null)
                {
                    command.Execute(CommandParameter, CommandTarget);
                }
                else
                {
                    ((ICommand)Command).Execute(CommandParameter);
                }
            }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(MapPolyline));

        public object CommandParameter
        {
            get
            {
                return (object)GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }



        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(MapPolyline));
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)GetValue(CommandTargetProperty);
            }
            set
            {
                SetValue(CommandTargetProperty, value);
            }
        }
    }
}
