// Decompiled with JetBrains decompiler
// Type: Configurator.App
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

#nullable disable
namespace Configurator;

public class App : Application
{
  public App()
  {
    FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
  }

  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public void InitializeComponent()
  {
    this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
  }

  [STAThread]
  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public static void Main()
  {
    App app = new App();
    app.InitializeComponent();
    app.Run();
  }
}
