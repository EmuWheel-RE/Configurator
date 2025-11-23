// Decompiled with JetBrains decompiler
// Type: Configurator.AxisData
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

#nullable disable
namespace Configurator;

public class AxisData
{
  public string Id { get; set; }

  public int AxisIndex { get; set; }

  public int Inverted { get; set; }

  public float Deadzone { get; set; }

  public enum AxisEnum
  {
    X,
    Y,
    Z,
    RotationX,
    RotationY,
    RotationZ,
    Sliders0,
    Sliders1,
  }
}
