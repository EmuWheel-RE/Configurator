// Decompiled with JetBrains decompiler
// Type: Configurator.MainWindow
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

using Microsoft.Win32;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

#nullable disable
namespace Configurator;

public partial class MainWindow : Window, IComponentConnector
{
  private const string VJoyProductGuid = "bead1234000000000000504944564944";
  private static SharpDX.DirectInput.DirectInput DI { get; set; }

  public MainWindow()
  {
    try
    {
      MainWindow.GenerateDataTableColumns();
      this.DataContext = (object) BindDetection.ButtonsData;
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }
    catch (Exception ex)
    {
      int num = (int) MessageBox.Show(ex.ToString());
    }
  }

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      MainWindow.DI = new SharpDX.DirectInput.DirectInput();
      this.GetDevices();
      this.CreateAxesNameArray();
      this.CreateButtonNameList();
    }
    catch (Exception ex)
    {
      int num = (int) MessageBox.Show(ex.ToString());
    }
  }

  private ButtonList ButtonList { get; set; }

  private void CreateButtonNameList()
  {
    this.ButtonList = new ButtonList()
    {
      ButtonNameList = Enum.GetValues(typeof (ButtonData.ButtonEnum)).Cast<ButtonData.ButtonEnum>().ToList<ButtonData.ButtonEnum>()
    };
    this.ButtonList.ButtonNameList.Remove(ButtonData.ButtonEnum.None);
    this.ButtonList.ButtonNameList.Insert(0, ButtonData.ButtonEnum.None);
    this.MainControlsGrid.DataContext = (object) this.ButtonList;
    this.ButtonTable.DataContext = (object) this.ButtonList;
    this.ButtonListPop.SelectedItem = (object) ButtonData.ButtonEnum.None;
  }

  private void GetDevices()
  {
    Guid guid = Guid.Parse("bead1234000000000000504944564944");
    BindDetection.GameControllers = new List<Joystick>();
    BindDetection.InputCollection = new List<Controller>();
    foreach (DeviceInstance device in (IEnumerable<DeviceInstance>) MainWindow.DI.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
    {
      if (!(device.ProductGuid == guid))
      {
        Joystick joystick = new Joystick(MainWindow.DI, device.InstanceGuid);
        joystick.Properties.BufferSize = 128 /*0x80*/;
        joystick.Acquire();
        BindDetection.GameControllers.Add(joystick);
        BindDetection.InputCollection.Add(new Controller()
        {
          InstanceGuid = joystick.Information.InstanceGuid,
          ProductGuid = joystick.Information.ProductGuid,
          InstanceName = joystick.Information.InstanceName,
          ProductName = joystick.Information.ProductName
        });
      }
    }
  }

  private void CreateAxesNameArray()
  {
    Type enumType = typeof (AxisData.AxisEnum);
    Enum.GetUnderlyingType(enumType);
    Array values = Enum.GetValues(enumType);
    BindDetection.AxesNames = new List<string>();
    BindDetection.AxesNames = values.OfType<object>().Select<object, string>((System.Func<object, string>) (o => o.ToString())).ToList<string>();
  }

  private static void GenerateDataTableColumns()
  {
    BindDetection.ButtonsData = new DataTable();
    BindDetection.ButtonsData.Columns.Add("Ingame Button");
    BindDetection.ButtonsData.Columns.Add("Device");
    BindDetection.ButtonsData.Columns.Add("Button", typeof (int));
    BindDetection.ButtonsData.Columns.Add("InstanceGuid", typeof (Guid));
  }

  private async void Set_Axis_Click(object sender, RoutedEventArgs e)
  {
    string id = ((FrameworkElement) sender).Name.Split('_')[1];
    int ctrIndex = -1;
    AxisData ax = (AxisData) null;
    this.ButtonListPop.IsEnabled = true;
    this.ButtonListPop.Visibility = Visibility.Hidden;
    this.ButtonList.PopupEnabled = true;
    this.MainControlsGrid.IsEnabled = false;
    this.ButtonText.Text = $"Interact with any axis on your controller \nto bind it to {id} axis...";
    await Task.Factory.StartNew((System.Action) (() => ax = BindDetection.GetAxis(id, out ctrIndex)), CancelToken.tokenSource.Token);
    if (ax != null && ctrIndex != -1)
    {
      List<TextBox> list = this.AxesGrid.Children.OfType<TextBox>().ToList<TextBox>();
      list.Where<TextBox>((System.Func<TextBox, bool>) (x => x.Name == ax.Id)).FirstOrDefault<TextBox>().Text = $"[{(object) ctrIndex}]{((AxisData.AxisEnum) ax.AxisIndex).ToString()}";
      this.AxesGrid.Children.OfType<CheckBox>().ToList<CheckBox>().Where<CheckBox>((System.Func<CheckBox, bool>) (x => x.Name == "Invert_" + ax.Id)).FirstOrDefault<CheckBox>().IsEnabled = true;
      list.Where<TextBox>((System.Func<TextBox, bool>) (x => x.Name == "Deadzone_" + id)).FirstOrDefault<TextBox>().IsEnabled = true;
    }
    this.ButtonList.PopupEnabled = false;
    this.MainControlsGrid.IsEnabled = true;
  }

  private async void Set_DPad_Click(object sender, RoutedEventArgs e)
  {
    int ctrIndex = -1;
    Configurator.DPad dPad = (Configurator.DPad) null;
    this.ButtonListPop.IsEnabled = true;
    this.ButtonListPop.Visibility = Visibility.Hidden;
    this.ButtonList.PopupEnabled = true;
    this.MainControlsGrid.IsEnabled = false;
    this.ButtonText.Text = "Press DPad (any direction) on your controller...";
    await Task.Factory.StartNew((System.Action) (() => dPad = BindDetection.GetDPad(out ctrIndex)), CancelToken.tokenSource.Token);
    if (dPad != null && ctrIndex != -1)
      this.DPad.Text = $"[{(object) ctrIndex}]DPad";
    this.ButtonList.PopupEnabled = false;
    this.MainControlsGrid.IsEnabled = true;
  }

  private void Set_Button_Click(object sender, RoutedEventArgs e)
  {
    this.ButtonListPop.SelectedItem = (object) ButtonData.ButtonEnum.None;
    this.ButtonListPop.IsEnabled = true;
    this.ButtonListPop.Visibility = Visibility.Visible;
    this.ButtonList.PopupEnabled = true;
    this.ButtonText.Text = "Choose a bind from drop-down list and \npress any button on your controller...";
    this.MainControlsGrid.IsEnabled = false;
  }

  private async void ButtonListPop_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    DataRow dupRow = (DataRow) null;
    if ((int) this.ButtonListPop.SelectedItem == -1)
      return;
    ButtonData.ButtonEnum buttonBind = (ButtonData.ButtonEnum) this.ButtonListPop.SelectedItem;
    this.ButtonListPop.IsEnabled = false;
    await Task.Factory.StartNew((System.Action) (() => dupRow = BindDetection.GetButton(buttonBind)), CancelToken.tokenSource.Token);
    this.ButtonList.PopupEnabled = false;
    this.ButtonBindDescription.Visibility = Visibility.Hidden;
    this.ButtonList.DataTableValues = BindDetection.ButtonsData.AsDataView();
    this.ButtonGridView.IsReadOnly = true;
    this.ButtonGridView.Columns[3].Visibility = Visibility.Hidden;
    this.ButtonGridView.Columns[0].Width = (DataGridLength) 100.0;
    this.ButtonGridView.Columns[1].Width = (DataGridLength) 200.0;
    this.MainControlsGrid.IsEnabled = true;
    if (dupRow != null)
    {
      int num = (int) MessageBox.Show($"Button '{dupRow.ItemArray[2].ToString()}' on controller '{dupRow.ItemArray[1].ToString()}' is already bound to '{dupRow.ItemArray[0].ToString()}'. Clear the binding and try again.");
    }
  }

  private void Grid_KeyDown(object sender, KeyEventArgs e)
  {
    if (e.Key != System.Windows.Input.Key.Escape)
      return;
    CancelToken.tokenSource.Cancel();
    CancelToken.tokenSource = new CancellationTokenSource();
    this.ButtonList.PopupEnabled = false;
    this.MainControlsGrid.IsEnabled = true;
  }

  private void Clear_Button_Click(object sender, RoutedEventArgs e)
  {
    List<DataRow> dataRowList = new List<DataRow>();
    foreach (DataRowView dataRowView in (IEnumerable) this.ButtonGridView.SelectedItems.AsQueryable())
      dataRowList.Add(dataRowView.Row);
    foreach (DataRow dataRow in dataRowList)
    {
      DataRow row = dataRow;
      int index1 = BindDetection.InputCollection.FindIndex((Predicate<Controller>) (x => x.InstanceGuid == (Guid) row.ItemArray[3]));
      int index2 = BindDetection.InputCollection[index1].Buttons.FindIndex((Predicate<ButtonData>) (x => x.Id.ToString() == row.ItemArray[0].ToString() && x.Index == (int) row.ItemArray[2]));
      BindDetection.InputCollection[index1].Buttons.RemoveAt(index2);
      BindDetection.ButtonsData.Rows.Remove(row);
    }
  }

  private void Clear_Axis_Click(object sender, RoutedEventArgs e)
  {
    string id = ((FrameworkElement) sender).Name.Split('_')[1];
    List<TextBox> list1 = this.AxesGrid.Children.OfType<TextBox>().ToList<TextBox>();
    TextBox textBox1 = list1.Where<TextBox>((System.Func<TextBox, bool>) (x => x.Name == id)).FirstOrDefault<TextBox>();
    TextBox textBox2 = list1.Where<TextBox>((System.Func<TextBox, bool>) (x => x.Name == "Deadzone_" + id)).FirstOrDefault<TextBox>();
    textBox2.Text = "0.000";
    textBox2.IsEnabled = false;
    textBox1.Text = "None";
    List<CheckBox> list2 = this.AxesGrid.Children.OfType<CheckBox>().ToList<CheckBox>();
    CheckBox checkBox1 = list2.Where<CheckBox>((System.Func<CheckBox, bool>) (x => x.Name == "Invert_" + id)).FirstOrDefault<CheckBox>();
    checkBox1.IsChecked = new bool?(false);
    checkBox1.IsEnabled = false;
    CheckBox checkBox2 = list2.Where<CheckBox>((System.Func<CheckBox, bool>) (x => x.Name == "Invert_" + id)).FirstOrDefault<CheckBox>();
    if (checkBox2 != null)
    {
      checkBox2.IsChecked = new bool?(false);
      checkBox2.IsEnabled = false;
    }
    List<List<AxisData>> list3 = BindDetection.InputCollection.Select<Controller, List<AxisData>>((System.Func<Controller, List<AxisData>>) (x => x.Axes)).ToList<List<AxisData>>();
    int index1 = -1;
    for (int index2 = 0; index2 < list3.Count; ++index2)
    {
      if (list3[index2] != null)
      {
        foreach (AxisData axisData in list3[index2])
        {
          if (axisData.Id == id)
          {
            index1 = index2;
            break;
          }
        }
      }
    }
    if (index1 == -1)
      return;
    BindDetection.InputCollection[index1].Axes.RemoveAll((Predicate<AxisData>) (y => y.Id == id));
    this.AxesGrid.Children.OfType<CheckBox>().ToList<CheckBox>().Where<CheckBox>((System.Func<CheckBox, bool>) (x => x.Name == "Invert_" + id)).FirstOrDefault<CheckBox>().IsEnabled = false;
  }

  private void Clear_DPad_Click(object sender, RoutedEventArgs e)
  {
    this.DPad.Text = "None";
    List<Configurator.DPad> list = BindDetection.InputCollection.Select<Controller, Configurator.DPad>((System.Func<Controller, Configurator.DPad>) (x => x.DPad)).ToList<Configurator.DPad>();
    int index1 = -1;
    for (int index2 = 0; index2 < list.Count; ++index2)
    {
      if (list[index2] != null)
        index1 = index2;
    }
    if (index1 == -1)
      return;
    BindDetection.InputCollection[index1].DPad = (Configurator.DPad) null;
  }

  private void Invert_Checked(object sender, RoutedEventArgs e)
  {
    string id = ((FrameworkElement) sender).Name.Split('_')[1];
    List<List<AxisData>> list = BindDetection.InputCollection.Select<Controller, List<AxisData>>((System.Func<Controller, List<AxisData>>) (x => x.Axes)).ToList<List<AxisData>>();
    int index1 = -1;
    for (int index2 = 0; index2 < list.Count; ++index2)
    {
      if (list[index2] != null)
      {
        foreach (AxisData axisData in list[index2])
        {
          if (axisData.Id == id)
          {
            index1 = index2;
            break;
          }
        }
      }
    }
    if (index1 == -1)
      return;
    BindDetection.InputCollection[index1].Axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == id)).Select<AxisData, AxisData>((System.Func<AxisData, AxisData>) (x => x)).FirstOrDefault<AxisData>().Inverted = 1;
    if (!(id == "Handbrake"))
      return;
    this.Gamepad_Handbrake.IsEnabled = true;
  }

  private void Invert_Unchecked(object sender, RoutedEventArgs e)
  {
    string id = ((FrameworkElement) sender).Name.Split('_')[1];
    List<List<AxisData>> list = BindDetection.InputCollection.Select<Controller, List<AxisData>>((System.Func<Controller, List<AxisData>>) (x => x.Axes)).ToList<List<AxisData>>();
    int index1 = -1;
    for (int index2 = 0; index2 < list.Count; ++index2)
    {
      if (list[index2] != null)
      {
        foreach (AxisData axisData in list[index2])
        {
          if (axisData.Id == id)
          {
            index1 = index2;
            break;
          }
        }
      }
    }
    if (index1 == -1)
      return;
    BindDetection.InputCollection[index1].Axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == id)).Select<AxisData, AxisData>((System.Func<AxisData, AxisData>) (x => x)).FirstOrDefault<AxisData>().Inverted = 0;
    if (!(id == "Handbrake"))
      return;
    this.Gamepad_Handbrake.IsChecked = new bool?(false);
    this.Gamepad_Handbrake.IsEnabled = false;
  }

  private void Gamepad_Handbrake_Checked(object sender, RoutedEventArgs e)
  {
    string id = "Handbrake";
    List<List<AxisData>> list = BindDetection.InputCollection.Select<Controller, List<AxisData>>((System.Func<Controller, List<AxisData>>) (x => x.Axes)).ToList<List<AxisData>>();
    int index1 = -1;
    for (int index2 = 0; index2 < list.Count; ++index2)
    {
      if (list[index2] != null)
      {
        foreach (AxisData axisData in list[index2])
        {
          if (axisData.Id == id)
          {
            index1 = index2;
            break;
          }
        }
      }
    }
    if (index1 == -1)
      return;
    BindDetection.InputCollection[index1].Axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == id)).Select<AxisData, AxisData>((System.Func<AxisData, AxisData>) (x => x)).FirstOrDefault<AxisData>().Inverted = -1;
  }

  private void Gamepad_Handbrake_Unchecked(object sender, RoutedEventArgs e)
  {
    string id = "Handbrake";
    List<List<AxisData>> list = BindDetection.InputCollection.Select<Controller, List<AxisData>>((System.Func<Controller, List<AxisData>>) (x => x.Axes)).ToList<List<AxisData>>();
    int index1 = -1;
    for (int index2 = 0; index2 < list.Count; ++index2)
    {
      if (list[index2] != null)
      {
        foreach (AxisData axisData in list[index2])
        {
          if (axisData.Id == id)
          {
            index1 = index2;
            break;
          }
        }
      }
    }
    if (index1 == -1)
      return;
    BindDetection.InputCollection[index1].Axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == id)).Select<AxisData, AxisData>((System.Func<AxisData, AxisData>) (x => x)).FirstOrDefault<AxisData>().Inverted = 1;
  }

  private int FFBAxisCheck()
  {
    List<List<AxisData>> list = BindDetection.InputCollection.Select<Controller, List<AxisData>>((System.Func<Controller, List<AxisData>>) (x => x.Axes)).ToList<List<AxisData>>();
    int num = -1;
    for (int index = 0; index < list.Count; ++index)
    {
      if (list[index] != null)
      {
        foreach (AxisData axisData in list[index])
        {
          if (axisData.Id == "Steering")
          {
            num = index;
            return num;
          }
        }
      }
    }
    return num;
  }

  private void ForceFeedback_Check_TextBox(object sender, RoutedEventArgs e)
  {
    if (BindDetection.InputCollection == null)
      return;
    TextBox parameter = (TextBox) sender;
    string name1 = parameter.Name.Split('_')[0];
    string name2 = parameter.Name.Split('_')[1];
    TextBox textBox = this.FFBGrid.Children.OfType<TextBox>().ToList<TextBox>().Where<TextBox>((System.Func<TextBox, bool>) (x => x.Name == parameter.Name)).FirstOrDefault<TextBox>();
    float result;
    float.TryParse(textBox.Text, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result);
    BindDetection.InputCollection.Select<Controller, List<AxisData>>((System.Func<Controller, List<AxisData>>) (x => x.Axes)).ToList<List<AxisData>>();
    int index = this.FFBAxisCheck();
    if (index == -1)
      return;
    if (BindDetection.InputCollection[index].FFBParameters == null)
      BindDetection.InputCollection[index].FFBParameters = new FFBParams();
    if (BindDetection.InputCollection[index].FFBParameters.GetType().GetProperty(name1).GetValue((object) BindDetection.InputCollection[index].FFBParameters) == null)
      BindDetection.InputCollection[index].FFBParameters.Const = new FFBParams.Constant();
    if (name2 != "Magnitude" && (double) result > 1.0)
    {
      result = 1f;
      textBox.Text = "1.000";
    }
    if ((double) result <= 0.0)
    {
      result = 0.0f;
      textBox.Text = "0.000";
    }
    BindDetection.InputCollection[index].FFBParameters.GetType().GetProperty(name1).GetValue((object) BindDetection.InputCollection[index].FFBParameters).GetType().GetProperty(name2).SetValue(BindDetection.InputCollection[index].FFBParameters.GetType().GetProperty(name1).GetValue((object) BindDetection.InputCollection[index].FFBParameters), (object) result);
  }

  private void Sine_Check_TextBox(object sender, RoutedEventArgs e)
  {
    if (BindDetection.InputCollection == null)
      return;
    TextBox parameter = (TextBox) sender;
    string name1 = parameter.Name.Split('_')[0];
    string str = parameter.Name.Split('_')[1];
    string name2 = parameter.Name.Split('_')[2];
    TextBox textBox = this.VibrationsGrid.Children.OfType<TextBox>().ToList<TextBox>().Where<TextBox>((System.Func<TextBox, bool>) (x => x.Name == parameter.Name)).FirstOrDefault<TextBox>();
    float result;
    float.TryParse(textBox.Text, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result);
    int index = this.FFBAxisCheck();
    if (index == -1)
      return;
    if (BindDetection.InputCollection[index].FFBParameters == null)
      BindDetection.InputCollection[index].FFBParameters = new FFBParams();
    if (BindDetection.InputCollection[index].FFBParameters.GetType().GetProperty(name1).GetValue((object) BindDetection.InputCollection[index].FFBParameters) == null)
      BindDetection.InputCollection[index].FFBParameters.Sine = new FFBParams.Periodic()
      {
        GearShiftVibrations = new FFBParams.Periodic.GearShiftVibration(),
        EngineVibrations = new FFBParams.Periodic.EngineVibration()
      };
    if (name2 != "Magnitude" && name2 != "Frequency" && (double) result > 1.0)
    {
      result = 1f;
      textBox.Text = "1.000";
    }
    if ((double) result <= 0.0)
    {
      result = 0.0f;
      textBox.Text = "0.000";
    }
    switch (str)
    {
      case "Sine":
        BindDetection.InputCollection[index].FFBParameters.GetType().GetProperty(name1).GetValue((object) BindDetection.InputCollection[index].FFBParameters).GetType().GetProperty(name2).SetValue(BindDetection.InputCollection[index].FFBParameters.GetType().GetProperty(name1).GetValue((object) BindDetection.InputCollection[index].FFBParameters), (object) result);
        break;
      case "Engine":
        switch (name2)
        {
          case "Frequency":
            BindDetection.InputCollection[index].FFBParameters.Sine.EngineVibrations.Frequency = result;
            return;
          case "Magnitude":
            BindDetection.InputCollection[index].FFBParameters.Sine.EngineVibrations.Strength = result;
            return;
          default:
            return;
        }
      case "GearShift":
        switch (name2)
        {
          case "Frequency":
            BindDetection.InputCollection[index].FFBParameters.Sine.GearShiftVibrations.Frequency = result;
            return;
          case "Magnitude":
            BindDetection.InputCollection[index].FFBParameters.Sine.GearShiftVibrations.Strength = result;
            return;
          default:
            return;
        }
    }
  }

  private void Condition_Check_TextBox(object sender, RoutedEventArgs e)
  {
    if (BindDetection.InputCollection == null)
      return;
    TextBox parameter = (TextBox) sender;
    string name1 = parameter.Name.Split('_')[0];
    string name2 = parameter.Name.Split('_')[1];
    TextBox textBox = this.ConditionGrid.Children.OfType<TextBox>().ToList<TextBox>().Where<TextBox>((System.Func<TextBox, bool>) (x => x.Name == parameter.Name)).FirstOrDefault<TextBox>();
    float result;
    float.TryParse(textBox.Text, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result);
    BindDetection.InputCollection.Select<Controller, List<AxisData>>((System.Func<Controller, List<AxisData>>) (x => x.Axes)).ToList<List<AxisData>>();
    int index = this.FFBAxisCheck();
    if (index == -1)
      return;
    if (BindDetection.InputCollection[index].FFBParameters == null)
      BindDetection.InputCollection[index].FFBParameters = new FFBParams();
    if (BindDetection.InputCollection[index].FFBParameters.GetType().GetProperty(name1).GetValue((object) BindDetection.InputCollection[index].FFBParameters) == null)
    {
      switch (name1)
      {
        case "Spring":
          BindDetection.InputCollection[index].FFBParameters.Spring = new FFBParams.Condition();
          break;
        case "Damper":
          BindDetection.InputCollection[index].FFBParameters.Damper = new FFBParams.Condition();
          break;
      }
    }
    if (name2 != "Coefficient" && (double) result > 1.0)
    {
      result = 1f;
      textBox.Text = "1.000";
    }
    if ((double) result <= 0.0)
    {
      result = 0.0f;
      textBox.Text = "0.000";
    }
    BindDetection.InputCollection[index].FFBParameters.GetType().GetProperty(name1).GetValue((object) BindDetection.InputCollection[index].FFBParameters).GetType().GetProperty(name2).SetValue(BindDetection.InputCollection[index].FFBParameters.GetType().GetProperty(name1).GetValue((object) BindDetection.InputCollection[index].FFBParameters), (object) result);
  }

  private void Deadzone_Changed(object sender, RoutedEventArgs e)
  {
    if (BindDetection.InputCollection == null)
      return;
    TextBox axis = (TextBox) sender;
    string id = axis.Name.Split('_')[1];
    TextBox textBox = this.AxesGrid.Children.OfType<TextBox>().ToList<TextBox>().Where<TextBox>((System.Func<TextBox, bool>) (x => x.Name == axis.Name)).FirstOrDefault<TextBox>();
    float result;
    float.TryParse(textBox.Text, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result);
    if ((double) result > 1.0)
    {
      result = 1f;
      textBox.Text = "1.000";
    }
    else if ((double) result <= 0.0)
    {
      result = 0.0f;
      textBox.Text = "0.000";
    }
    List<List<AxisData>> list = BindDetection.InputCollection.Select<Controller, List<AxisData>>((System.Func<Controller, List<AxisData>>) (x => x.Axes)).ToList<List<AxisData>>();
    int index1 = -1;
    for (int index2 = 0; index2 < list.Count; ++index2)
    {
      if (list[index2] != null)
      {
        foreach (AxisData axisData in list[index2])
        {
          if (axisData.Id == id)
          {
            index1 = index2;
            break;
          }
        }
      }
    }
    if (index1 == -1)
      return;
    BindDetection.InputCollection[index1].Axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == id)).Select<AxisData, AxisData>((System.Func<AxisData, AxisData>) (x => x)).FirstOrDefault<AxisData>().Deadzone = result;
  }

  private void LoadDefaultFFBValues(int ctrlIndex)
  {
    if (BindDetection.InputCollection[ctrlIndex].FFBParameters != null)
      return;
    BindDetection.InputCollection[ctrlIndex].FFBParameters = new FFBParams()
    {
      Const = new FFBParams.Constant(),
      Sine = new FFBParams.Periodic()
      {
        EngineVibrations = new FFBParams.Periodic.EngineVibration(),
        GearShiftVibrations = new FFBParams.Periodic.GearShiftVibration()
      },
      Spring = new FFBParams.Condition(),
      Damper = new FFBParams.Condition()
    };
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.Magnitude = 1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.MaximumForce = 1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.MinimumForce = 0.0f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.FilterThreshold = 1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.MinimumCoefficient = 1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.Magnitude = 1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.MaximumForce = 1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.MinimumForce = 0.0f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.Phase = 0.375f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.Frequency = 1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.EngineVibrations.Frequency = 1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.EngineVibrations.Strength = 0.03f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.GearShiftVibrations.Frequency = 1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.GearShiftVibrations.Strength = 0.1f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Spring.Coefficient = 0.0f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Spring.Saturation = 0.0f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Damper.Coefficient = 0.0f;
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Damper.Saturation = 0.0f;
    Controller input = BindDetection.InputCollection[ctrlIndex];
    this.Const_Magnitude.Text = input.FFBParameters.Const.Magnitude.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Const_MinimumForce.Text = input.FFBParameters.Const.MinimumForce.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Const_MaximumForce.Text = input.FFBParameters.Const.MaximumForce.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Const_FilterThreshold.Text = input.FFBParameters.Const.FilterThreshold.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Const_MinimumCoefficient.Text = input.FFBParameters.Const.MinimumCoefficient.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_Magnitude.Text = input.FFBParameters.Sine.Magnitude.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_MinimumForce.Text = input.FFBParameters.Sine.MinimumForce.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_MaximumForce.Text = input.FFBParameters.Sine.MaximumForce.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_Frequency.Text = input.FFBParameters.Sine.Frequency.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_Phase.Text = input.FFBParameters.Sine.Phase.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Engine_Magnitude.Text = input.FFBParameters.Sine.EngineVibrations.Strength.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Engine_Frequency.Text = input.FFBParameters.Sine.EngineVibrations.Frequency.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_GearShift_Magnitude.Text = input.FFBParameters.Sine.GearShiftVibrations.Strength.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_GearShift_Frequency.Text = input.FFBParameters.Sine.GearShiftVibrations.Frequency.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Spring_Coefficient.Text = input.FFBParameters.Spring.Coefficient.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Spring_Saturation.Text = input.FFBParameters.Spring.Saturation.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Damper_Coefficient.Text = input.FFBParameters.Damper.Coefficient.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Damper_Saturation.Text = input.FFBParameters.Damper.Saturation.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
  }

  private void Steering_TextChanged(object sender, TextChangedEventArgs e)
  {
    TextBox textBox = (TextBox) sender;
    if (BindDetection.InputCollection == null)
      return;
    int num = this.FFBAxisCheck();
    if (textBox.Text != "None" && BindDetection.InputCollection[num].FFBParameters == null)
    {
      this.FFBMainGrid.IsEnabled = true;
      this.FFBMessage.Visibility = Visibility.Hidden;
      this.LoadDefaultFFBValues(num);
    }
    else if (textBox.Text != "None" && BindDetection.InputCollection[num].FFBParameters != null)
    {
      this.FFBMainGrid.IsEnabled = true;
      this.FFBMessage.Visibility = Visibility.Hidden;
    }
    else
    {
      BindDetection.InputCollection[num].FFBParameters = (FFBParams) null;
      this.FFBMainGrid.IsEnabled = false;
      this.FFBMessage.Visibility = Visibility.Visible;
    }
  }

  private void Save_Config_Click(object sender, RoutedEventArgs e)
  {
    try
    {
      bool flag = true;
      foreach (Controller input in BindDetection.InputCollection)
      {
        if (input.Axes != null || input.Buttons != null || input.DPad != null)
          flag = false;
      }
      if (flag)
      {
        int num1 = (int) MessageBox.Show("At least one controller needs to be bound for creating configuration file.");
      }
      else if (this.Steering.Text == "None")
      {
        int num2 = (int) MessageBox.Show("Steering axis must be bound to save configuration");
      }
      else
      {
        List<Controller> inputCollection = BindDetection.InputCollection;
        List<int> intList = new List<int>();
        int num3 = 0;
        foreach (Controller controller in inputCollection)
        {
          if (controller.Axes == null && controller.Buttons == null && controller.DPad == null)
            intList.Add(num3);
          ++num3;
        }
        foreach (int index in intList)
          inputCollection.RemoveAt(index);
        JsonSerializer jsonSerializer = new JsonSerializer();
        using (StreamWriter streamWriter = new StreamWriter("configuration.json"))
        {
          using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) streamWriter))
          {
            jsonWriter.Formatting = Formatting.Indented;
            jsonSerializer.Serialize(jsonWriter, (object) inputCollection);
          }
        }
        int num4 = (int) MessageBox.Show("Configuration saved!");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.ToString());
    }
  }

  private void GetAxisIndices(List<TextBox> textBoxes, List<AxisData> axes)
  {
    foreach (TextBox textBox in textBoxes)
    {
      TextBox tb = textBox;
      AxisData ax = axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == tb.Name)).FirstOrDefault<AxisData>();
      int num1 = -1;
      if (ax == null)
      {
        tb.Text = "None";
      }
      else
      {
        int num2 = 0;
        foreach (Controller input in BindDetection.InputCollection)
        {
          if (input.Axes != null && input.Axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == ax.Id)).FirstOrDefault<AxisData>() != null)
          {
            num1 = num2;
            ++num2;
          }
        }
        tb.Text = $"[{(object) num1}]{((AxisData.AxisEnum) ax.AxisIndex).ToString()}";
      }
    }
  }

  private void GetDeadzones(List<TextBox> textBoxes, List<AxisData> axes)
  {
    foreach (TextBox textBox in textBoxes)
    {
      TextBox tb = textBox;
      AxisData ax = axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == tb.Name.Split('_')[1])).FirstOrDefault<AxisData>();
      if (ax == null)
      {
        tb.Text = "0.00";
      }
      else
      {
        int num = 0;
        foreach (Controller input in BindDetection.InputCollection)
        {
          if (input.Axes != null && input.Axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == ax.Id)).FirstOrDefault<AxisData>() != null)
            ++num;
        }
        tb.IsEnabled = true;
        tb.Text = ax.Deadzone.ToString("F3");
      }
    }
  }

  private void GetInverts(List<CheckBox> checkBoxes, List<AxisData> axes)
  {
    foreach (CheckBox checkBox in checkBoxes)
    {
      CheckBox cb = checkBox;
      AxisData axis = axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == cb.Name.Split('_')[1])).FirstOrDefault<AxisData>();
      if (axis != null)
      {
        cb.IsEnabled = true;
        int inverted = axis.Inverted;
        if (axis.Inverted != 0)
          cb.IsChecked = new bool?(true);
        if (axis.Id == "Handbrake")
          this.GetGamepadStick(axis, inverted);
      }
    }
  }

  private void GetGamepadStick(AxisData axis, int gamepadInvert)
  {
    if (axis.Inverted != 0)
      this.Gamepad_Handbrake.IsEnabled = true;
    if (gamepadInvert != -1)
      return;
    this.Gamepad_Handbrake.IsChecked = new bool?(true);
  }

  private void GetButtons()
  {
    BindDetection.ButtonsData.Clear();
    foreach (Controller input in BindDetection.InputCollection)
    {
      if (input.Buttons != null)
      {
        foreach (ButtonData button in input.Buttons)
        {
          DataRow row = BindDetection.ButtonsData.NewRow();
          row[0] = (object) button.Id;
          row[1] = (object) input.InstanceName;
          row[2] = (object) button.Index;
          row[3] = (object) input.InstanceGuid;
          BindDetection.ButtonsData.Rows.Add(row);
        }
      }
    }
    this.ButtonList.DataTableValues = BindDetection.ButtonsData.AsDataView();
    this.ButtonGridView.IsReadOnly = true;
    this.ButtonGridView.Columns[3].Visibility = Visibility.Hidden;
    this.ButtonGridView.Columns[0].Width = (DataGridLength) 100.0;
    this.ButtonGridView.Columns[1].Width = (DataGridLength) 200.0;
    this.ButtonBindDescription.Visibility = Visibility.Hidden;
  }

  private void SetFFBParameters(int ctrlIndex)
  {
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.Magnitude = float.Parse(this.Const_Magnitude.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.MinimumForce = float.Parse(this.Const_MinimumForce.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.MaximumForce = float.Parse(this.Const_MaximumForce.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.FilterThreshold = float.Parse(this.Const_FilterThreshold.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Const.MinimumCoefficient = float.Parse(this.Const_MinimumCoefficient.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.Magnitude = float.Parse(this.Sine_Sine_Magnitude.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.MinimumForce = float.Parse(this.Sine_Sine_MinimumForce.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.MaximumForce = float.Parse(this.Sine_Sine_MaximumForce.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.Frequency = float.Parse(this.Sine_Sine_Frequency.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.Phase = float.Parse(this.Sine_Sine_Phase.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.EngineVibrations.Frequency = float.Parse(this.Sine_Engine_Frequency.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.EngineVibrations.Strength = float.Parse(this.Sine_Engine_Magnitude.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.GearShiftVibrations.Frequency = float.Parse(this.Sine_GearShift_Frequency.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Sine.GearShiftVibrations.Strength = float.Parse(this.Sine_GearShift_Magnitude.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Spring.Coefficient = float.Parse(this.Spring_Coefficient.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Spring.Saturation = float.Parse(this.Spring_Saturation.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Damper.Coefficient = float.Parse(this.Damper_Coefficient.Text, (IFormatProvider) CultureInfo.InvariantCulture);
    BindDetection.InputCollection[ctrlIndex].FFBParameters.Damper.Saturation = float.Parse(this.Damper_Saturation.Text, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  private void GetFFBParams(Controller ctrl)
  {
    if (ctrl == null || !ctrl.Axes.Where<AxisData>((System.Func<AxisData, bool>) (x => x.Id == "Steering")).Select<AxisData, AxisData>((System.Func<AxisData, AxisData>) (x => x)).Any<AxisData>())
      return;
    this.Const_Magnitude.Text = ctrl.FFBParameters.Const.Magnitude.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Const_MinimumForce.Text = ctrl.FFBParameters.Const.MinimumForce.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Const_MaximumForce.Text = ctrl.FFBParameters.Const.MaximumForce.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Const_FilterThreshold.Text = ctrl.FFBParameters.Const.FilterThreshold.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Const_MinimumCoefficient.Text = ctrl.FFBParameters.Const.MinimumCoefficient.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_Magnitude.Text = ctrl.FFBParameters.Sine.Magnitude.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_MinimumForce.Text = ctrl.FFBParameters.Sine.MinimumForce.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_MaximumForce.Text = ctrl.FFBParameters.Sine.MaximumForce.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_Frequency.Text = ctrl.FFBParameters.Sine.Frequency.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Sine_Phase.Text = ctrl.FFBParameters.Sine.Phase.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Engine_Magnitude.Text = ctrl.FFBParameters.Sine.EngineVibrations.Strength.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_Engine_Frequency.Text = ctrl.FFBParameters.Sine.EngineVibrations.Frequency.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_GearShift_Magnitude.Text = ctrl.FFBParameters.Sine.GearShiftVibrations.Strength.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Sine_GearShift_Frequency.Text = ctrl.FFBParameters.Sine.GearShiftVibrations.Frequency.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Spring_Coefficient.Text = ctrl.FFBParameters.Spring.Coefficient.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Spring_Saturation.Text = ctrl.FFBParameters.Spring.Saturation.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Damper_Coefficient.Text = ctrl.FFBParameters.Damper.Coefficient.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
    this.Damper_Saturation.Text = ctrl.FFBParameters.Damper.Saturation.ToString("F3", (IFormatProvider) CultureInfo.InvariantCulture);
  }

  private static List<Controller> LoadedControllers { get; set; }

  private void Load_Config_Click(object sender, RoutedEventArgs e)
  {
    OpenFileDialog openFileDialog1 = new OpenFileDialog();
    openFileDialog1.DefaultExt = ".json";
    openFileDialog1.Filter = "Configuration (.json)|*.json";
    OpenFileDialog openFileDialog2 = openFileDialog1;
    bool? nullable = openFileDialog2.ShowDialog();
    bool flag = false;
    if ((nullable.GetValueOrDefault() == flag ? (nullable.HasValue ? 1 : 0) : 0) != 0)
      return;
    MainWindow.LoadedControllers = JsonConvert.DeserializeObject<List<Controller>>(File.ReadAllText(openFileDialog2.FileName));
    foreach (Controller loadedController in MainWindow.LoadedControllers)
    {
      Controller controller = loadedController;
      int index = BindDetection.InputCollection.FindIndex((Predicate<Controller>) (x => x.InstanceGuid == controller.InstanceGuid));
      if (index != -1)
      {
        BindDetection.InputCollection[index] = controller;
      }
      else
      {
        int num = (int) MessageBox.Show($"Controller '{controller.InstanceName}' is not connected. Connect it and try loading configuration again.");
        return;
      }
    }
    List<TextBox> list = this.AxesGrid.Children.OfType<TextBox>().ToList<TextBox>().Where<TextBox>((System.Func<TextBox, bool>) (x => !x.Name.Contains("Deadzone"))).Select<TextBox, TextBox>((System.Func<TextBox, TextBox>) (x => x)).ToList<TextBox>();
    List<AxisData> axes = new List<AxisData>();
    foreach (Controller input in BindDetection.InputCollection)
    {
      if (input.Axes != null)
        axes.AddRange((IEnumerable<AxisData>) input.Axes);
    }
    if (axes != null)
    {
      this.GetAxisIndices(list, axes);
      this.GetDeadzones(this.AxesGrid.Children.OfType<TextBox>().ToList<TextBox>().Where<TextBox>((System.Func<TextBox, bool>) (x => x.Name.Contains("Deadzone_"))).Select<TextBox, TextBox>((System.Func<TextBox, TextBox>) (x => x)).ToList<TextBox>(), axes);
      this.GetInverts(this.AxesGrid.Children.OfType<CheckBox>().ToList<CheckBox>().Where<CheckBox>((System.Func<CheckBox, bool>) (x => x.Name.Contains("Invert_"))).Select<CheckBox, CheckBox>((System.Func<CheckBox, CheckBox>) (x => x)).ToList<CheckBox>(), axes);
    }
    if (BindDetection.InputCollection.Where<Controller>((System.Func<Controller, bool>) (x => x.DPad != null)).Select<Controller, Controller>((System.Func<Controller, Controller>) (x => x)).FirstOrDefault<Controller>() != null)
      this.DPad.Text = $"[{(object) BindDetection.InputCollection.FindIndex((Predicate<Controller>) (x => x.DPad != null))}]DPad";
    this.GetButtons();
    this.GetFFBParams(MainWindow.LoadedControllers.Where<Controller>((System.Func<Controller, bool>) (x => x.FFBParameters != null)).FirstOrDefault<Controller>());
  }
}
