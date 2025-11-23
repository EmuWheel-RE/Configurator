// Decompiled with JetBrains decompiler
// Type: Configurator.CancelToken
// Assembly: Configurator, Version=1.3.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B139A836-6C09-4F6B-8B20-4ECB31EE51FA
// Assembly location: C:\Users\fred\Downloads\forza_emuwheel_v1.4a\Configurator.exe

using System.Threading;

#nullable disable
namespace Configurator;

public class CancelToken
{
  public static CancellationTokenSource tokenSource = new CancellationTokenSource();
}
