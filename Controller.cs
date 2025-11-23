// Decompiled with JetBrains decompiler
// Type: Configurator.Controller
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

using System;
using System.Collections.Generic;

#nullable disable
namespace Configurator;

public class Controller
{
  public Guid ProductGuid { get; set; }

  public Guid InstanceGuid { get; set; }

  public string InstanceName { get; set; }

  public string ProductName { get; set; }

  public List<AxisData> Axes { get; set; }

  public List<ButtonData> Buttons { get; set; }

  public DPad DPad { get; set; }

  public FFBParams FFBParameters { get; set; }
}
