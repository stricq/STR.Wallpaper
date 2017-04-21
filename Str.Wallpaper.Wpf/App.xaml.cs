using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper;

using CefSharp;

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
      //
      // Add Custom assembly resolver
      //
      AppDomain.CurrentDomain.AssemblyResolve += Resolver;
      //
      // Any CefSharp references have to be in another method with NonInlining
      // attribute so the assembly rolver has time to do it's thing.
      //
      InitializeCefSharp();
    }

    #endregion Constructor

    #region Overrides

    protected override void OnStartup(StartupEventArgs e) {
      base.OnStartup(e);

      try {
        IEnumerable<IAutoMapperConfiguration> configurations = container.GetAll<IAutoMapperConfiguration>();

        MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => configurations.ForEach(configuration => configuration.RegisterMappings(cfg)));

        try {
          mapperConfiguration.AssertConfigurationIsValid();
        }
        catch(Exception ex) {
          MessageBox.Show(ex.Message, "Mapping Validation Error");
        }

        container.RegisterInstance(mapperConfiguration.CreateMapper());

        IEnumerable<IController> controllers = container.GetAll<IController>();

        IOrderedEnumerable<IGrouping<int, IController>> groups = controllers.GroupBy(c => c.InitializePriority).OrderBy(g => g.Key);

        foreach(IGrouping<int, IController> group in groups) {
          Task.Run(() => group.ForEachAsync(controller => controller.InitializeAsync())).Wait();
        }
      }
      catch(Exception ex) {
        while(ex.InnerException != null) ex = ex.InnerException;

        MessageBox.Show(ex.Message, "MEF Error");
      }
    }

    #endregion Overrides

    #region Private Methods

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InitializeCefSharp() {
      //
      // Set BrowserSubProcessPath based on app bitness at runtime
      //
      CefSettings settings = new CefSettings {
        BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Environment.Is64BitProcess ? "x64" : "x86", "CefSharp.BrowserSubprocess.exe")
      };
      //
      // Make sure you set performDependencyCheck false
      //
      Cef.Initialize(settings, false, null);
    }
    //
    // Will attempt to load missing assembly from either x86 or x64 subdir
    // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
    //
    private static Assembly Resolver(object sender, ResolveEventArgs args) {
      if (args.Name.StartsWith("CefSharp")) {
        string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";

        string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, Environment.Is64BitProcess ? "x64" : "x86", assemblyName);

        return File.Exists(archSpecificPath) ? Assembly.LoadFile(archSpecificPath) : null;
      }

      return null;
    }

    #endregion Private Methods

  }

}
