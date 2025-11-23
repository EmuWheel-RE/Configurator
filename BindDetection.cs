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

    public static DPad GetDPad(out int controllerIndex)
    {
        controllerIndex = -1;
        while (true)
        {
            foreach (Joystick gameController in BindDetection.GameControllers)
            {
                Joystick joy = gameController;
                JoystickUpdate[] bufferedData = joy.GetBufferedData();
                if (bufferedData.Length == 1 && bufferedData[0].Offset.ToString().IndexOf("PointOfView") != -1)
                {
                    int index = BindDetection.InputCollection.FindIndex(
                        (Predicate<Controller>)(x => x.InstanceGuid == joy.Information.InstanceGuid));
                    if (BindDetection.InputCollection[index].DPad == null)
                        BindDetection.InputCollection[index].DPad = new DPad();
                    DPad dpad = new DPad() { Index = index };
                    BindDetection.InputCollection[index].DPad = dpad;
                    controllerIndex = index;
                    return dpad;
                }
            }

            Thread.Sleep(1);
        }
    }

    public static DataRow GetButton(ButtonData.ButtonEnum bindButton)
    {
        while (true)
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
                        int index = BindDetection.InputCollection.FindIndex(
                            (Predicate<Controller>)(x => x.InstanceGuid == joy.Information.InstanceGuid));
                        if (BindDetection.InputCollection[index].Buttons == null)
                            BindDetection.InputCollection[index].Buttons = new List<ButtonData>();
                        offset = bufferedData[0].Offset;
                        int buttonIndex = Convert.ToInt32(offset.ToString().Substring(7));
                        DataRow row = BindDetection.ButtonsData.NewRow();
                        row[0] = (object)bindButton.ToString();
                        row[1] = (object)joy.Information.InstanceName;
                        row[2] = (object)buttonIndex;
                        row[3] = (object)joy.Information.InstanceGuid;
                        DataRow button = BindDetection.ButtonsData.AsEnumerable()
                            .Where<DataRow>((System.Func<DataRow, bool>)(x =>
                                Guid.Parse(x.ItemArray[3].ToString()) == joy.Information.InstanceGuid &&
                                (int)x.ItemArray[2] == buttonIndex)).FirstOrDefault<DataRow>();
                        if (button != null)
                            return button;
                        ButtonData buttonData = new ButtonData()
                        {
                            Id = bindButton,
                            Index = buttonIndex
                        };
                        BindDetection.InputCollection[index].Buttons.Add(buttonData);
                        BindDetection.ButtonsData.Rows.Add(row);
                        return (DataRow)null;
                    }
                }
            }

            Thread.Sleep(1);
        }
    }

    public static AxisData GetAxis(string axisId, out int matchingControllerIndex)
    {
        matchingControllerIndex = -1;
        while (true)
        {
            foreach (Joystick gameController in GameControllers)
            {
                Joystick joy = gameController;
                foreach (var update in joy.GetBufferedData())
                {
                    JoystickOffset offset = update.Offset;
                    if (offset.ToString().IndexOf("Button") == -1)
                    {
                        if (offset.ToString().IndexOf("PointOfView") == -1)
                        {
                            int controllerIndex = BindDetection.InputCollection.FindIndex(
                                (Predicate<Controller>)(x => x.InstanceGuid == joy.Information.InstanceGuid));
                            if (InputCollection[controllerIndex].Axes == null)
                                InputCollection[controllerIndex].Axes = new List<AxisData>();
                            AxisData axisData = new AxisData()
                            {
                                AxisIndex = AxesNames.IndexOf(update.Offset.ToString()),
                                Id = axisId
                            };
                            if (InputCollection[controllerIndex].Axes.FindIndex(
                                    (Predicate<AxisData>)(x =>
                                        x.Id == axisData.Id && x.AxisIndex == axisData.AxisIndex)) != -1)
                            {
                                int axisIndex = InputCollection[controllerIndex].Axes
                                    .FindIndex((Predicate<AxisData>)(x => x.Id == axisData.Id));
                                InputCollection[controllerIndex].Axes[axisIndex] = axisData;
                                matchingControllerIndex = controllerIndex;
                                return axisData;
                            }

                            if (InputCollection[controllerIndex].Axes
                                    .FindIndex((Predicate<AxisData>)(x => x.AxisIndex == axisData.AxisIndex)) != -1)
                            {
                                int currentBindingIndex = InputCollection[controllerIndex].Axes
                                    .FindIndex((Predicate<AxisData>)(x => x.AxisIndex == axisData.AxisIndex));
                                string currentBindingName = InputCollection[controllerIndex].Axes[currentBindingIndex]
                                    .Id.ToString();
                                MessageBox.Show(
                                    $"The axis '{offset.ToString()}' on controller '{InputCollection[controllerIndex].InstanceName}' is already bound to '{currentBindingName}'. Clear the binding and try again.",
                                    "Duplicate binding", MessageBoxButton.OK, MessageBoxImage.Exclamation,
                                    MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                matchingControllerIndex = controllerIndex;
                                return (AxisData)null;
                            }

                            InputCollection[controllerIndex].Axes.Add(axisData);
                            matchingControllerIndex = controllerIndex;
                            return axisData;
                        }
                    }
                }
            }

            Thread.Sleep(1);
        }
    }
}