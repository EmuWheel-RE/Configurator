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
        FlushControllerBuffers();
        controllerIndex = -1;
        while (true)
        {
            foreach (var joy in BindDetection.GameControllers)
            {
                foreach (var update in joy.GetBufferedData())
                {
                    if (!update.Offset.ToString().Contains("PointOfView"))
                    {
                        continue;
                    }
                    
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
        FlushControllerBuffers();
        while (true)
        {
            foreach (var joy in BindDetection.GameControllers)
            {
                foreach (var update in joy.GetBufferedData())
                {
                    JoystickOffset offset = update.Offset;
                    var controlName = offset.ToString();
                    if (update.Value == 0 || !controlName.StartsWith("Button"))
                    {
                        continue;
                    }

                    var controller = InputCollection.Find(
                        (Predicate<Controller>)(x => x.InstanceGuid == joy.Information.InstanceGuid));
                    controller.Buttons ??= new List<ButtonData>();
                    
                    int buttonIndex = Convert.ToInt32(controlName.Substring(7));
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
                    controller.Buttons.Add(buttonData);
                    ButtonsData.Rows.Add(row);
                    return (DataRow)null;
                }
            }

            Thread.Sleep(1);
        }
    }

    private static void FlushControllerBuffers()
    {
        foreach (var gameController in BindDetection.GameControllers)
        {
            gameController.GetBufferedData();
        }
    }

    public static AxisData GetAxis(string axisId, out int matchingControllerIndex)
    {
        // Flush buffers, so we don't bind to stuff that's moved since the last binding
        FlushControllerBuffers();

        // Keyed by (controllerIndex, axisIndex)
        // Values are (initialValue, range)
        var firstValues = new Dictionary<(int, int), (int, int)>();
        matchingControllerIndex = -1;

        while (true)
        {
            foreach (var gameController in GameControllers)
            {
                Joystick joy = gameController;
                foreach (var update in joy.GetBufferedData())
                {
                    JoystickOffset offset = update.Offset;
                    var controlName = offset.ToString();
                    if (controlName.Contains("Button"))
                    {
                        continue;
                    }

                    if (controlName.Contains("PointOfView"))
                    {
                        continue;
                    }

                    var axisData = new AxisData()
                    {
                        AxisIndex = AxesNames.IndexOf(controlName),
                        Id = axisId
                    };

                    int controllerIndex = BindDetection.InputCollection.FindIndex(
                        (Predicate<Controller>)(x => x.InstanceGuid == joy.Information.InstanceGuid));
                    var stateKey = (controllerIndex, axisData.AxisIndex);
                    if (!firstValues.ContainsKey(stateKey))
                    {
                        var range = joy.GetObjectPropertiesByName(controlName).LogicalRange;
                        firstValues.Add(stateKey, (update.Value, range.Maximum - range.Minimum));
                        continue;
                    }
                    else
                    {
                        var (firstValue, range) = firstValues[stateKey];
                        // Require a 5% change, so that we can reliably bind with noisy/high-sensitivity inputs
                        if (Math.Abs(update.Value - firstValue) < (range / 20))
                        {
                            continue;
                        }
                    }

                    var controller = InputCollection[controllerIndex];

                    if (controller.Axes == null)
                        controller.Axes = new List<AxisData>();
                    if (controller.Axes.FindIndex(
                            (Predicate<AxisData>)(x =>
                                x.Id == axisData.Id && x.AxisIndex == axisData.AxisIndex)) != -1)
                    {
                        int axisIndex = controller.Axes
                            .FindIndex((Predicate<AxisData>)(x => x.Id == axisData.Id));
                        controller.Axes[axisIndex] = axisData;
                        matchingControllerIndex = controllerIndex;
                        return axisData;
                    }

                    if (controller.Axes
                            .FindIndex((Predicate<AxisData>)(x => x.AxisIndex == axisData.AxisIndex)) != -1)
                    {
                        int currentBindingIndex = controller.Axes
                            .FindIndex((Predicate<AxisData>)(x => x.AxisIndex == axisData.AxisIndex));
                        string currentBindingName = controller.Axes[currentBindingIndex]
                            .Id.ToString();
                        MessageBox.Show(
                            $"The axis '{offset.ToString()}' on controller '{controller.InstanceName}' is already bound to '{currentBindingName}'. Clear the binding and try again.",
                            "Duplicate binding", MessageBoxButton.OK, MessageBoxImage.Exclamation,
                            MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                        matchingControllerIndex = controllerIndex;
                        return (AxisData)null;
                    }

                    controller.Axes.Add(axisData);
                    matchingControllerIndex = controllerIndex;
                    return axisData;
                }
            }

            Thread.Sleep(1);
        }
    }
}