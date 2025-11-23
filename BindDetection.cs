// Decompiled with JetBrains decompiler
// Type: Configurator.BindDetection
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

#nullable disable
namespace Configurator;

internal class BindDetection
{
  public static List<Joystick> GameControllers { get; set; }

  public static List<Controller> InputCollection { get; set; }

  public static List<string> AxesNames { get; set; }

  public static DataTable ButtonsData { get; set; }

  public static DPad GetDPad(out int ctrindex)
  {
    bool flag = true;
    while (flag)
    {
      foreach (Joystick gameController in BindDetection.GameControllers)
      {
        Joystick joy = gameController;
        JoystickUpdate[] bufferedData = joy.GetBufferedData();
        if (bufferedData.Length == 1 && bufferedData[0].Offset.ToString().IndexOf("PointOfView") != -1)
        {
          int index = BindDetection.InputCollection.FindIndex((Predicate<Controller>) (x => x.InstanceGuid == joy.Information.InstanceGuid));
          if (BindDetection.InputCollection[index].DPad == null)
            BindDetection.InputCollection[index].DPad = new DPad();
          DPad dpad = new DPad() { Index = index };
          BindDetection.InputCollection[index].DPad = dpad;
          ctrindex = index;
          flag = false;
          return dpad;
        }
      }
      Thread.Sleep(1);
    }
    ctrindex = -1;
    return (DPad) null;
  }

  public static DataRow GetButton(ButtonData.ButtonEnum bindButton)
  {
    bool flag = true;
    while (flag)
    {
      foreach (Joystick gameController in BindDetection.GameControllers)
      {
        Joystick joy = gameController;
        JoystickUpdate[] bufferedData = joy.GetBufferedData();
        if (bufferedData.Length == 1)
        {
          JoystickOffset offset = bufferedData[0].Offset;
          if (offset.ToString().IndexOf("Button") != -1 && bufferedData[0].Value != 0)
          {
            int index = BindDetection.InputCollection.FindIndex((Predicate<Controller>) (x => x.InstanceGuid == joy.Information.InstanceGuid));
            if (BindDetection.InputCollection[index].Buttons == null)
              BindDetection.InputCollection[index].Buttons = new List<ButtonData>();
            offset = bufferedData[0].Offset;
            int buttonIndex = Convert.ToInt32(offset.ToString().Substring(7));
            DataRow row = BindDetection.ButtonsData.NewRow();
            row[0] = (object) bindButton.ToString();
            row[1] = (object) joy.Information.InstanceName;
            row[2] = (object) buttonIndex;
            row[3] = (object) joy.Information.InstanceGuid;
            DataRow button = BindDetection.ButtonsData.AsEnumerable().Where<DataRow>((System.Func<DataRow, bool>) (x => Guid.Parse(x.ItemArray[3].ToString()) == joy.Information.InstanceGuid && (int) x.ItemArray[2] == buttonIndex)).FirstOrDefault<DataRow>();
            if (button != null)
              return button;
            ButtonData buttonData = new ButtonData()
            {
              Id = bindButton,
              Index = buttonIndex
            };
            BindDetection.InputCollection[index].Buttons.Add(buttonData);
            BindDetection.ButtonsData.Rows.Add(row);
            return (DataRow) null;
          }
        }
      }
      Thread.Sleep(1);
    }
    return (DataRow) null;
  }

  public static AxisData GetAxis(string axisId, out int ctrindex)
  {
    bool flag = true;
    while (flag)
    {
      foreach (Joystick gameController in BindDetection.GameControllers)
      {
        Joystick joy = gameController;
        JoystickUpdate[] data = joy.GetBufferedData();
        if (data.Length == 1)
        {
          JoystickOffset offset = data[0].Offset;
          if (offset.ToString().IndexOf("Button") == -1)
          {
            offset = data[0].Offset;
            if (offset.ToString().IndexOf("PointOfView") == -1)
            {
              int index1 = BindDetection.InputCollection.FindIndex((Predicate<Controller>) (x => x.InstanceGuid == joy.Information.InstanceGuid));
              if (BindDetection.InputCollection[index1].Axes == null)
                BindDetection.InputCollection[index1].Axes = new List<AxisData>();
              int index2 = BindDetection.AxesNames.FindIndex((Predicate<string>) (x => x.ToString() == data[0].Offset.ToString()));
              AxisData ax = new AxisData()
              {
                AxisIndex = index2,
                Id = axisId
              };
              if (BindDetection.InputCollection[index1].Axes.FindIndex((Predicate<AxisData>) (x => x.Id == ax.Id && x.AxisIndex == ax.AxisIndex)) != -1)
              {
                int index3 = BindDetection.InputCollection[index1].Axes.FindIndex((Predicate<AxisData>) (x => x.Id == ax.Id));
                BindDetection.InputCollection[index1].Axes[index3] = ax;
                flag = false;
                ctrindex = index1;
                return ax;
              }
              if (BindDetection.InputCollection[index1].Axes.FindIndex((Predicate<AxisData>) (x => x.AxisIndex == ax.AxisIndex)) != -1)
              {
                int index4 = BindDetection.InputCollection[index1].Axes.FindIndex((Predicate<AxisData>) (x => x.AxisIndex == ax.AxisIndex));
                string str = BindDetection.InputCollection[index1].Axes[index4].Id.ToString();
                string[] strArray = new string[7];
                strArray[0] = "The axis '";
                offset = data[0].Offset;
                strArray[1] = offset.ToString();
                strArray[2] = "' on controller '";
                strArray[3] = BindDetection.InputCollection[index1].InstanceName;
                strArray[4] = "' is already bound to '";
                strArray[5] = str;
                strArray[6] = "'. Clear the binding and try again.";
                int num = (int) MessageBox.Show(string.Concat(strArray), "Duplicate binding", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                flag = false;
                ctrindex = index1;
                return (AxisData) null;
              }
              BindDetection.InputCollection[index1].Axes.Add(ax);
              flag = false;
              ctrindex = index1;
              return ax;
            }
          }
        }
      }
      Thread.Sleep(1);
    }
    ctrindex = -1;
    return (AxisData) null;
  }
}
