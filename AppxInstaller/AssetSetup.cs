﻿using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AppxInstaller
{
    public class AssetSetup : INotifyPropertyChanged
    {
        /// <summary>
        /// The UI thread marshalling delegate. It should be set for the environments where cross-thread calls
        /// must be marshalled (e.g. WPF, WinForms). Not needed otherwise (e.g. Console application).
        /// </summary>
        public Action<Action> InUiThread = (action) => action();

        public AssetSetup(string productName, string productVersion)
        {
            IsCurrentlyInstalled = false;
            ProductName = productName;
            ProductVersion = productVersion;

            ProductStatus = string.Format("The product is {0}INSTALLED\n\n", IsCurrentlyInstalled ? "" : "NOT ");
        }

        CancellationTokenSource cancelTransfer = null;
        public async void Install()
        {
            Progress<AppxProgress> progress = new Progress<AppxProgress>(p =>
            {
                if (p.Percentage == ProgressTotal)
                {
                    ProgressCurrentPosition = (int)p.Percentage;
                }
                else
                {
                    ProgressCurrentPosition = (int)p.Percentage;
                }
            });

            IsRunning = true;
            ProgressTotal = 100;
            cancelTransfer = new CancellationTokenSource();
            bool result = await AppxBundle.InstallAppx(progress, cancelTransfer.Token);
            IsRunning = false;
            cancelTransfer = null;
        }

        /// <summary>
        /// Cancel started install.
        /// </summary>
        public virtual void StartCancel()
        {
            if (IsRunning && cancelTransfer != null)
            {
                cancelTransfer.Cancel();
            }
        }

        /// <summary>
        /// Starts the fresh installation.
        /// </summary>
        public virtual void StartInstall()
        {
            if (!IsCurrentlyInstalled)
            {
                Install();
            }
            else
                ErrorStatus = "Product is already installed";
        }

        /// <summary>
        /// Starts the repair installation for the already installed product.
        /// </summary>
        public virtual void StartRepair()
        {
            if (IsCurrentlyInstalled)
            {
                IsRunning = true;
            }
            else
                ErrorStatus = "Product is not installed";
        }

        /// <summary>
        /// Starts the uninstallation of the already installed product.
        /// </summary>
        public virtual void StartUninstall()
        {
            if (IsCurrentlyInstalled)
            {
                IsRunning = true;
            }
            else
                ErrorStatus = "Product is not installed";
        }

        /// <summary>
        /// Gets or sets the error status.
        /// </summary>
        /// <value>
        /// The error status.
        /// </value>
        string errorStatus;
        public string ErrorStatus
        {
            get => errorStatus;
            set => SetValue(ref errorStatus, value);
        }

        string installDirectory;
        public string InstallDirectory
        {
            get => installDirectory;
            set => SetValue(ref installDirectory, value);
        }

        /// <summary>
        /// Gets or sets the product status (installed or not installed).
        /// </summary>
        /// <value>
        /// The product status.
        /// </value>
        string productStatus;
        public string ProductStatus
        {
            get => productStatus;
            set => SetValue(ref productStatus, value);
        }

        /// <summary>
        /// Gets or sets the product version.
        /// </summary>
        /// <value>
        /// The product version.
        /// </value>
        string productVersion;
        public string ProductVersion
        {
            get => productVersion;
            set => SetValue(ref productVersion, value);
        }

        /// <summary>
        /// Gets or sets the MSI ProductName.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        string productName;
        public string ProductName
        {
            get => productName;
            set => SetValue(ref productName, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the setup is in progress.
        /// </summary>
        /// <value>
        /// <c>true</c> if this setup is in progress; otherwise, <c>false</c>.
        /// </value>
        bool isRunning;
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;

                if (isRunning)
                    NotStarted = false;

                OnPropertyChanged(nameof(IsRunning));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether setup was not started yet. This information
        /// can be useful for implementing "not started" UI state in the setup GUI.  
        /// </summary>
        /// <value>
        ///   <c>true</c> if setup was not started; otherwise, <c>false</c>.
        /// </value>
        bool notStarted = true;
        public bool NotStarted
        {
            get => notStarted;
            set => SetValue(ref notStarted, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the product is currently installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product is currently installed; otherwise, <c>false</c>.
        /// </value>
        bool isCurrentlyInstalled;
        public bool IsCurrentlyInstalled
        {
            get => isCurrentlyInstalled;
            set
            {
                isCurrentlyInstalled = value;
                SetValue(ref isCurrentlyInstalled, value);

                OnPropertyChanged(nameof(CanInstall));
                OnPropertyChanged(nameof(CanUnInstall));
                OnPropertyChanged(nameof(CanRepair));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the product can be installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product can install; otherwise, <c>false</c>.
        /// </value>
        public bool CanInstall { get => !IsCurrentlyInstalled; }

        /// <summary>
        /// Gets a value indicating whether the product can be uninstalled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product can uninstall; otherwise, <c>false</c>.
        /// </value>
        public bool CanUnInstall { get => IsCurrentlyInstalled; }

        /// <summary>
        /// Gets a value indicating whether the product can be repaired.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product can repaired; otherwise, <c>false</c>.
        /// </value>
        public bool CanRepair { get => IsCurrentlyInstalled; }

        /// <summary>
        /// Gets or sets the progress total.
        /// </summary>
        /// <value>The progress total.</value>
        int progressTotal = 100;
        public int ProgressTotal
        {
            get => progressTotal;
            protected set
            {
                progressTotal = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the progress current position.
        /// </summary>
        /// <value>The progress current position.</value>
        int progressCurrentPosition = 0;
        public int ProgressCurrentPosition
        {
            get => progressCurrentPosition;
            protected set
            {
                if (ProgressStepDelay > 0)
                    Thread.Sleep(ProgressStepDelay);

                progressCurrentPosition = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The progress step delay. It is a "for-testing" feature. Set it to positive value (number of milliseconds)
        /// to artificially slow down the installation process. The default value is 0.
        /// </summary>
        public static int ProgressStepDelay = 0;

        /// <summary>
        /// Gets or sets the name of the current action.
        /// </summary>
        /// <value>
        /// The name of the current action.
        /// </value>
        string currentActionName = null;
        public string CurrentActionName
        {
            get => currentActionName;
            protected set
            {
                var newValue = (value ?? "").Trim();

                currentActionName = newValue;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the product language.
        /// </summary>
        /// <value>The language.</value>
        int language;
        public int Language
        {
            get => language;
            protected set => SetValue(ref language, value);
        }

        /// <summary>
        /// Gets or sets the product CodePage.
        /// </summary>
        /// <value>The product CodePage.</value>
        int codePage;
        public int CodePage
        {
            get => codePage;
            protected set => SetValue(ref codePage, value);
        }

        /// <summary>
        /// Gets or sets the flag indication the the user can cancel the setup in progress.
        /// </summary>
        /// <value>The can cancel.</value>
        bool canCancel;
        public bool CanCancel
        {
            get => canCancel;
            protected set => SetValue(ref canCancel, value);
        }

        /// <summary>
        /// Gets or sets the setup window caption.
        /// </summary>
        /// <value>The caption.</value>
        string caption;
        public string Caption
        {
            get => caption;
            protected set => SetValue(ref caption, value);
        }

        int ticksPerActionDataMessage;
        protected int TicksPerActionDataMessage
        {
            get => ticksPerActionDataMessage;
            set => SetValue(ref ticksPerActionDataMessage, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the progress steps are changing in the forward direction.
        /// </summary>
        /// <value>
        /// <c>true</c> if the progress changes are in forward direction; otherwise, <c>false</c>.
        /// </value>
        bool isProgressForwardDirection;
        public bool IsProgressForwardDirection
        {
            get => isProgressForwardDirection;
            set => SetValue(ref isProgressForwardDirection, value);
        }

        bool isProgressTimeEstimationAccurate;
        protected bool IsProgressTimeEstimationAccurate
        {
            get => isProgressTimeEstimationAccurate;
            set => SetValue(ref isProgressTimeEstimationAccurate, value);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                InUiThread(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;
            backingField = value;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Occurs when some of the current instance property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
