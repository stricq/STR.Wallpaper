﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;

using AutoMapper;

using Str.Wallpaper.Domain.Contracts;

using STR.Common.Extensions;

using STR.MvvmCommon.Contracts;
using STR.MvvmCommon.Mef;


namespace Str.Wallpaper.Wpf {

  internal sealed partial class App : Application {

    #region Private Fields

    private readonly IMvvmContainer container;

    #endregion Private Fields

    #region Constructor

    public App() {
      container = new MvvmContainer();

      container.Initialize(() => new AggregateCatalog(new DirectoryCatalog(Directory.GetCurrentDirectory(), "Str.Wallpaper.Wpf.exe"),
                                                      new DirectoryCatalog(Directory.GetCurrentDirectory(), "Str.*.dll")));
    }

    #endregion Constructor

    #region Overrides

    protected override void OnStartup(StartupEventArgs e) {
      base.OnStartup(e);

      try {
        IEnumerable<IAutoMapperConfiguration> configurations = container.GetAll<IAutoMapperConfiguration>();

        MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => configurations.ForEach(configuration => configuration.RegisterMappings(cfg)));

        mapperConfiguration.AssertConfigurationIsValid();

        container.RegisterInstance(mapperConfiguration.CreateMapper());

        container.GetAll<IController>();
      }
      catch(Exception ex) {
        MessageBox.Show(ex.Message, "MEF or Mapping Error");
      }
    }

    #endregion Overrides

  }

}
