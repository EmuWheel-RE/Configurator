// Decompiled with JetBrains decompiler
// Type: Configurator.ButtonData
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

#nullable disable
namespace Configurator;

public class ButtonData
{
  public ButtonData.ButtonEnum Id { get; set; }

  public int Index { get; set; }

  public enum ButtonEnum
  {
    None = -1, // 0xFFFFFFFF
    XboxView = 0,
    XboxMenu = 1,
    XBoxA = 2,
    XBoxB = 3,
    XBoxX = 4,
    XBoxY = 5,
    Button7 = 6,
    Button8 = 7,
    Button9 = 8,
    Button10 = 9,
    Button11 = 10, // 0x0000000A
    Button12 = 11, // 0x0000000B
    Button13 = 12, // 0x0000000C
    Button14 = 13, // 0x0000000D
    Button15 = 14, // 0x0000000E
    Button16 = 15, // 0x0000000F
    PreviousGear = 16, // 0x00000010
    NextGear = 17, // 0x00000011
    ReverseGear = 18, // 0x00000012
    ForwardGear1 = 19, // 0x00000013
    ForwardGear2 = 20, // 0x00000014
    ForwardGear3 = 21, // 0x00000015
    ForwardGear4 = 22, // 0x00000016
    ForwardGear5 = 23, // 0x00000017
    ForwardGear6 = 24, // 0x00000018
    ForwardGear7 = 25, // 0x00000019
    Button27 = 26, // 0x0000001A
    Button28 = 27, // 0x0000001B
    Button29 = 28, // 0x0000001C
    Button30 = 29, // 0x0000001D
    Button31 = 30, // 0x0000001E
    Button32 = 31, // 0x0000001F
  }
}
