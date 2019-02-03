﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using InteractiveAlert;
using MvvmCross;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.ViewModels;

namespace Android
{
    public class Setup : MvxAppCompatSetup<Core.App>
    {
        protected override void InitializePlatformServices()
        {
        }
    }
}