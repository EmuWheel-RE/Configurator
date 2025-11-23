// Decompiled with JetBrains decompiler
// Type: Configurator.ControlsBindPanel
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable disable
namespace Configurator;

internal class ControlsBindPanel : INotifyPropertyChanged
{
  private string _Steering = "None";
  private string _Combined = "None";
  private string _Throttle = "None";
  private string _Brake = "None";
  private string _Clutch = "None";
  private string _Handbrake = "None";

  public event PropertyChangedEventHandler PropertyChanged;

  private void OnPropertyChanged([CallerMemberName] string caller = null)
  {
    PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
    if (propertyChanged == null)
      return;
    propertyChanged((object) this, new PropertyChangedEventArgs(caller));
  }

  public string Steering
  {
    get => this._Steering;
    set
    {
      this._Steering = value;
      this.OnPropertyChanged(nameof (Steering));
    }
  }

  public string Combined
  {
    get => this._Combined;
    set
    {
      this._Combined = value;
      this.OnPropertyChanged(nameof (Combined));
    }
  }

  public string Throttle
  {
    get => this._Throttle;
    set
    {
      this._Throttle = value;
      this.OnPropertyChanged(nameof (Throttle));
    }
  }

  public string Brake
  {
    get => this._Brake;
    set
    {
      this._Brake = value;
      this.OnPropertyChanged(nameof (Brake));
    }
  }

  public string Clutch
  {
    get => this._Clutch;
    set
    {
      this._Clutch = value;
      this.OnPropertyChanged(nameof (Clutch));
    }
  }

  public string Handbrake
  {
    get => this._Handbrake;
    set
    {
      this._Handbrake = value;
      this.OnPropertyChanged(nameof (Handbrake));
    }
  }
}
