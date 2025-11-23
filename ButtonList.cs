// Decompiled with JetBrains decompiler
// Type: Configurator.ButtonList
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

#nullable disable
namespace Configurator;

internal class ButtonList : INotifyPropertyChanged
{
  private List<ButtonData.ButtonEnum> buttonNameList;
  private bool popupEnabled;
  private DataView dataTableValues;

  public event PropertyChangedEventHandler PropertyChanged;

  protected void OnPropertyChanged(string value)
  {
    PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
    if (propertyChanged == null)
      return;
    propertyChanged((object) this, new PropertyChangedEventArgs(value));
  }

  public bool PopupEnabled
  {
    get => this.popupEnabled;
    set
    {
      this.popupEnabled = value;
      this.OnPropertyChanged(nameof (PopupEnabled));
    }
  }

  public DataView DataTableValues
  {
    get => this.dataTableValues;
    set
    {
      this.dataTableValues = value;
      this.OnPropertyChanged(nameof (DataTableValues));
    }
  }

  public List<ButtonData.ButtonEnum> ButtonNameList
  {
    get => this.buttonNameList;
    set
    {
      this.buttonNameList = value;
      this.OnPropertyChanged(nameof (ButtonNameList));
    }
  }
}
