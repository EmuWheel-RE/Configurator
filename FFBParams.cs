// Decompiled with JetBrains decompiler
// Type: Configurator.FFBParams
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

#nullable disable
namespace Configurator;

public class FFBParams
{
  public FFBParams.Constant Const { get; set; }

  public FFBParams.Periodic Sine { get; set; }

  public FFBParams.Condition Spring { get; set; }

  public FFBParams.Condition Damper { get; set; }

  public class Constant
  {
    public float Magnitude { get; set; }

    public float MaximumForce { get; set; }

    public float MinimumForce { get; set; }

    public float FilterThreshold { get; set; }

    public float MinimumCoefficient { get; set; }
  }

  public class Periodic
  {
    public float Magnitude { get; set; }

    public float Frequency { get; set; }

    public float MaximumForce { get; set; }

    public float MinimumForce { get; set; }

    public float Phase { get; set; }

    public FFBParams.Periodic.EngineVibration EngineVibrations { get; set; }

    public FFBParams.Periodic.GearShiftVibration GearShiftVibrations { get; set; }

    public class EngineVibration
    {
      public float Frequency { get; set; }

      public float Strength { get; set; }
    }

    public class GearShiftVibration
    {
      public float Frequency { get; set; }

      public float Strength { get; set; }
    }
  }

  public class Condition
  {
    public float Coefficient { get; set; }

    public float Saturation { get; set; }
  }
}
